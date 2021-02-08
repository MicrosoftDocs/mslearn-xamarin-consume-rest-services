using BookClient.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BookClient
{
    public partial class MainPage : ContentPage
    {
        readonly IList<Book> books = new ObservableCollection<Book>();
        readonly BookManager manager = new BookManager();

        public MainPage()
        {
            BindingContext = books;
            InitializeComponent();
        }

        async void OnRefresh(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var bookCollection = await manager.GetAll();
                foreach (Book book in bookCollection)
                {
                    if (books.All(b => b.ISBN != book.ISBN))
                        books.Add(book);
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error",
                        ex.Message,
                        "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void OnAddNewBook(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
                new AddEditBookPage(manager, books));
        }

        async void OnEditBook(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushModalAsync(
                new AddEditBookPage(manager, books, (Book)e.Item));
        }

        async void OnDeleteBook(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var book = item.CommandParameter as Book;
            if (book == null || IsBusy)
                return;

            if (await this.DisplayAlert("Delete Book?",
                "Are you sure you want to delete the book '"
                    + book.Title + "'?", "Yes", "Cancel") == true)
            {
                try
                {
                    IsBusy = true;
                    await manager.Delete(book.ISBN);
                    books.Remove(book);
                }
                catch (Exception ex)
                {
                    await this.DisplayAlert("Error",
                            ex.Message,
                            "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}

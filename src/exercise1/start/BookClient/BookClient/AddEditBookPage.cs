using Xamarin.Forms;
using System;
using System.Linq;
using BookClient.Data;
using System.Collections.Generic;

namespace BookClient
{
    public class AddEditBookPage : ContentPage
    {
        readonly Book existingBook;
        readonly EntryCell titleCell, genreCell, authorCell;
        readonly IList<Book> books;
        readonly BookManager manager;

        public AddEditBookPage(BookManager manager, IList<Book> books, Book existingBook = null)
        {
            this.manager = manager;
            this.books = books;
            this.existingBook = existingBook;

            var tableView = new TableView {
                Intent = TableIntent.Form,
                Root = new TableRoot(existingBook != null ? "Edit Book" : "New Book") {  
                    new TableSection("Details") {
                        new TextCell {
                            Text = "ISBN",
                            Detail = (existingBook != null) ? existingBook.ISBN : "Will be generated"
                        },
                        (titleCell = new EntryCell {
                            Label = "Title",
                            Placeholder = "add title",
                            Text = (existingBook != null) ? existingBook.Title : null,
                        }),
                        (genreCell = new EntryCell {
                            Label = "Genre",
                            Placeholder = "add genre",
                            Text = (existingBook != null) ? existingBook.Genre : null,
                        }),
                        (authorCell = new EntryCell {
                            Label = "Author",
                            Placeholder = "add author",
                            Text = (existingBook != null) ? existingBook.Authors.FirstOrDefault() : null,
                        }),
                    },
                }
            };

            Button button = new Button() {
                BackgroundColor = existingBook != null ? Color.Gray : Color.Green,
                TextColor = Color.White,
                Text = existingBook != null ? "Finished" : "Add Book",
                BorderRadius = 0,
            };
            button.Clicked += OnDismiss;

            Content = new StackLayout
            {
                Spacing = 0,
                Children = { tableView, button },
            };
        }

        async void OnDismiss(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            button.IsEnabled = false;
            this.IsBusy = true;
            try
            {
                string title = titleCell.Text;
                string author = authorCell.Text;
                string genre = genreCell.Text;

                if (string.IsNullOrWhiteSpace(title)
                    || string.IsNullOrWhiteSpace(author)
                    || string.IsNullOrWhiteSpace(genre))
                {
                    this.IsBusy = false;
                    await this.DisplayAlert("Missing Information",
                        "You must enter values for the Title, Author, and Genre.",
                        "OK");
                }
                else
                {
                    if (existingBook != null)
                    {
                        existingBook.Title = title;
                        existingBook.Genre = genre;
                        existingBook.Authors[0] = author;

                        await manager.Update(existingBook);
                        int pos = books.IndexOf(existingBook);
                        books.RemoveAt(pos);
                        books.Insert(pos, existingBook);
                    }
                    else
                    {
                        Book book = await manager.Add(title, author, genre);
                        books.Add(book);
                    }

                    await Navigation.PopModalAsync();
                }

            }
            finally
            {
                this.IsBusy = false;
                button.IsEnabled = true;
            }
        }
    }
}


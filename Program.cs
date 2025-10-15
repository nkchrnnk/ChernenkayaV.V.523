//Задание
//Создайте консольное приложение C# для учёта книг в библиотеке.
//У книги должны быть следующие параметры: 
//Уникальный идентификатор (генерируется автоматически при добавлении). 
//Название. 
//Автор. 
//Жанр (можно выбрать из заданных в коде вариантов, не менее трёх). 
//Год издания. 
//Цена.
//Мы можем работать с книгами через команды:
//Добавить книгу (запросить все параметры у пользователя, идентификатор назначается автоматически).
//Удалить книгу по идентификатору.
//Найти книги (по названию, автору, жанру, должны быть все варианты поиска книги) и выводить полную информацию.
//Отсортировать книги по названию или году (должны быть обе команды). 
//Вывести самую дорогую и самую дешёвую книгу.
//Сгруппировать книги по авторам и вывести количество книг каждого автора.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Genre
{
    Фэнтези,
    НаучнаяФантастика,
    Детектив,
    Роман,
    Триллер,
    Биография,
    История
}
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public Genre Genre { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; } 
    
    public override string ToString()
    {
        return $"ID: {Id}, Название: {Title}, Автор: {Author}, Жанр: {Genre}, Год: {Year}, Цена: {Price:C}";
    }
}

class Program
{
    static List<Book> books = new List<Book>();
    static int nextId = 1;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        InitializeTestData();

        Console.WriteLine("=== СИСТЕМА УЧЁТА БИБЛИОТЕКИ ==="); 
        
        while (true)
        {
            ShowMenu();

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Ошибка ввода! Введите число от 1 до 8.");
                continue;
            }
            switch (choice)
            {
                case 1:
                    AddBook();
                    break;
                case 2:
                    RemoveBook();
                    break;
                case 3:
                    SearchBooks();
                    break;
                case 4:
                    SortBooks();
                    break;
                case 5:
                    ShowPriceExtremes();
                    break;
                case 6:
                    ShowBooksByAuthors();
                    break;
                case 7:
                    ShowAllBooks();
                    break;
                case 8:
                    Console.WriteLine("До свидания!");
                    return;
                default:
                    Console.WriteLine("Неверный выбор! Введите число от 1 до 8.");
                    break;
            }
        }
    }
    static void InitializeTestData()
    {
        books.AddRange(new[]
        {
            new Book { Id = nextId++, Title = "Властелин Колец", Author = "Дж. Р. Р. Толкин", Genre = Genre.Фэнтези, Year = 1954, Price = 1500 },
            new Book { Id = nextId++, Title = "1984", Author = "Джордж Оруэлл", Genre = Genre.НаучнаяФантастика, Year = 1949, Price = 800 },
            new Book { Id = nextId++, Title = "Убийство в Восточном экспрессе", Author = "Агата Кристи", Genre = Genre.Детектив, Year = 1934, Price = 700 },
            new Book { Id = nextId++, Title = "Гордость и предубеждение", Author = "Джейн Остин", Genre = Genre.Роман, Year = 1813, Price = 600 },
            new Book { Id = nextId++, Title = "Шерлок Холмс", Author = "Артур Конан Дойл", Genre = Genre.Детектив, Year = 1887, Price = 900 }
        });

        Console.WriteLine("Добавлено 5 тестовых книг.");
    }
    static void ShowMenu()
    {
        Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
        Console.WriteLine("1. Добавить книгу");
        Console.WriteLine("2. Удалить книгу по ID");
        Console.WriteLine("3. Найти книги");
        Console.WriteLine("4. Отсортировать книги");
        Console.WriteLine("5. Самая дорогая и дешёвая книга");
        Console.WriteLine("6. Книги по авторам");
        Console.WriteLine("7. Показать все книги");
        Console.WriteLine("8. Выход");
        Console.Write("Выберите действие: ");
    }
    
    static void AddBook()
    {
        Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОЙ КНИГИ ===");

        Console.Write("Название: ");
        string title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Ошибка: название не может быть пустым!");
            return;
        }

        Console.Write("Автор: ");
        string author = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(author))
        {
            Console.WriteLine("Ошибка: автор не может быть пустым!");
            return;
        }

        Console.WriteLine("Выберите жанр:");
        var genres = Enum.GetValues(typeof(Genre));
        for (int i = 0; i < genres.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {genres.GetValue(i)}");
        }

        Console.Write("Жанр (номер): ");
        if (!int.TryParse(Console.ReadLine(), out int genreChoice) || genreChoice < 1 || genreChoice > genres.Length)
        {
            Console.WriteLine($"Ошибка: выберите жанр от 1 до {genres.Length}!");
            return;
        }

        Console.Write("Год издания: ");
        if (!int.TryParse(Console.ReadLine(), out int year) || year < 1000 || year > DateTime.Now.Year)
        {
            Console.WriteLine($"Ошибка: год должен быть от 1000 до {DateTime.Now.Year}!");
            return;
        }

        Console.Write("Цена: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
        {
            Console.WriteLine("Ошибка: цена должна быть положительным числом!");
            return;
        }

        var newBook = new Book
        {
            Id = nextId++,
            Title = title.Trim(),
            Author = author.Trim(),
            Genre = (Genre)(genreChoice - 1),
            Year = year,
            Price = price
        };

        books.Add(newBook);
        Console.WriteLine($"Книга '{title}' успешно добавлена с ID {newBook.Id}!");
    }
    

    
    
    

    
    
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
    

    
    
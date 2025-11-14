using System;
using System.Linq;
using System.Threading.Tasks;
using ChernenkayaV.V._523.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChernenkayaV.V._523
{
    public static class MarketplaceApp
    {
        private static User _currentUser;

        public static void Run()
        {
            Console.WriteLine(" GMWOG Маркетплейс ");
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            while (true)
            {
                if (_currentUser == null)
                    await ShowMainMenu();
                else
                    await ShowUserMenu();
            }
        }

        private static async Task ShowMainMenu()
        {
            Console.WriteLine("\n1. Товары");
            Console.WriteLine("2. Регистрация");
            Console.WriteLine("3. Вход");
            Console.WriteLine("4. Выход");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1": await ShowProducts(); break;
                case "2": await RegisterUser(); break;
                case "3": await LoginUser(); break;
                case "4": Environment.Exit(0); break;
            }
        }

        private static async Task ShowUserMenu()
        {
            Console.WriteLine($"\nПользователь: {_currentUser.Username}");
            Console.WriteLine("1. Товары");
            Console.WriteLine("2. Корзина");
            Console.WriteLine("3. Заказы");
            Console.WriteLine("4. Выйти");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1": await ShowProducts(); break;
                case "2": await ShowCart(); break;
                case "3": await ShowOrders(); break;
                case "4": _currentUser = null; break;
            }
        }

        private static async Task ShowProducts()
        {
            var products = await Core.Context.Products.ToListAsync();

            Console.WriteLine("\n=== Товары ===");
            foreach (var p in products)
                Console.WriteLine($"{p.Id}. {p.Name} - {p.Price} руб. ({p.Quantity} шт.)");

            if (_currentUser != null)
            {
                Console.Write("\nДобавить в корзину? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                    await AddToCart();
            }
        }

        private static async Task AddToCart()
        {
            Console.Write("ID товара: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) return;

            var product = await Core.Context.Products.FindAsync(id);
            if (product == null || product.Quantity < qty)
            {
                Console.WriteLine("Ошибка!");
                return;
            }

            var cartItem = await Core.Context.Carts
                .FirstOrDefaultAsync(c => c.UserId == _currentUser.Id && c.ProductId == id);

            if (cartItem != null)
                cartItem.Quantity += qty;
            else
                Core.Context.Carts.Add(new Cart { UserId = _currentUser.Id, ProductId = id, Quantity = qty });

            await Core.Context.SaveChangesAsync();
            Console.WriteLine("Добавлено в корзину!");
        }

        private static async Task ShowCart()
        {
            var cart = await Core.Context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == _currentUser.Id)
                .ToListAsync();

            if (cart.Count == 0)
            {
                Console.WriteLine("\nКорзина пуста!");
                return;
            }

            Console.WriteLine("\n=== Корзина ===");
            double total = 0;
            
            foreach (var item in cart)
            {
                double price = (double)item.Product.Price;
                double sum = price * item.Quantity;
                total += sum;
                Console.WriteLine($"{item.Id}. {item.Product.Name} - {item.Quantity} x {price} руб. = {sum} руб.");
            }
            
            Console.WriteLine($"Итого: {total} руб.");

            Console.WriteLine("\n1. Удалить\n2. Купить\n3. Назад");
            var choice = Console.ReadLine();
            
            if (choice == "1") 
                await RemoveFromCart();
            else if (choice == "2") 
                await CreateOrder((decimal)total);
        }

        private static async Task RemoveFromCart()
        {
            Console.Write("ID позиции: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var item = await Core.Context.Carts.FindAsync(id);
                if (item != null)
                {
                    Core.Context.Carts.Remove(item);
                    await Core.Context.SaveChangesAsync();
                    Console.WriteLine("Удалено!");
                }
            }
        }

        private static async Task CreateOrder(decimal total)
        {
            var points = await Core.Context.Points.ToListAsync();
            
            Console.WriteLine("\n=== ПВЗ ===");
            foreach (var p in points)
                Console.WriteLine($"{p.Id}. {p.Name} - {p.Address}");

            Console.Write("Выберите ПВЗ: ");
            if (!int.TryParse(Console.ReadLine(), out int pointId)) return;

            var order = new Order 
            { 
                UserId = _currentUser.Id, 
                PointId = pointId, 
                Total = total,
                Created = DateTime.Now
            };

            Core.Context.Orders.Add(order);
            
            // Очищаем корзину
            var cartItems = Core.Context.Carts.Where(c => c.UserId == _currentUser.Id);
            Core.Context.Carts.RemoveRange(cartItems);
            
            await Core.Context.SaveChangesAsync();
            Console.WriteLine($"Заказ №{order.Id} оформлен!");
        }

        private static async Task ShowOrders()
        {
            var orders = await Core.Context.Orders
                .Include(o => o.Point)
                .Where(o => o.UserId == _currentUser.Id)
                .OrderByDescending(o => o.Created)
                .ToListAsync();

            if (orders.Count == 0)
            {
                Console.WriteLine("\nЗаказов нет!");
                return;
            }

            Console.WriteLine("\n=== Заказы ===");
            foreach (var o in orders)
                Console.WriteLine($"№{o.Id} - {o.Total} руб. - {o.Point.Name} - {o.Created:dd.MM.yyyy}");
        }

        private static async Task RegisterUser()
        {
            Console.Write("Логин: ");
            var login = Console.ReadLine();

            Console.Write("Пароль: ");
            var pass = Console.ReadLine();

            Console.Write("Повтор пароля: ");
            var pass2 = Console.ReadLine();

            if (pass != pass2)
            {
                Console.WriteLine("Пароли не совпадают!");
                return;
            }

            if (await Core.Context.Users.AnyAsync(u => u.Username == login))
            {
                Console.WriteLine("Логин занят!");
                return;
            }

            Core.Context.Users.Add(new User { Username = login, Password = pass });
            await Core.Context.SaveChangesAsync();
            Console.WriteLine("Успешная регистрация!");
        }

        private static async Task LoginUser()
        {
            Console.Write("Логин: ");
            var login = Console.ReadLine();

            Console.Write("Пароль: ");
            var pass = Console.ReadLine();

            _currentUser = await Core.Context.Users.FirstOrDefaultAsync(u => u.Username == login && u.Password == pass);

            if (_currentUser != null)
                Console.WriteLine($"Добро пожаловать, {_currentUser.Username}!");
            else
                Console.WriteLine("Ошибка входа!");
        }
    }
}
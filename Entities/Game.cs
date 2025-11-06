using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ChernenkayaV.V._523.Entities;
using ChernenkayaV.V._523.Context;

namespace ChernenkayaV.V._523
{
    public class Game
    {
        private Random random = new Random();
        
        public void Start()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            Console.WriteLine("АВТОСЕРВИС");
            Console.WriteLine("============");
            
            try
            {
                var garages = Core.Context.Garages.ToList();
                var details = Core.Context.Details.ToList();
                var warehouse = Core.Context.DetailsGarages.ToList();
                
                Console.WriteLine("Данные загружены");
                Console.WriteLine($"Баланс: {garages.First().Balance.First():C}");
                Console.WriteLine($"Деталей: {details.Count}");
                Console.WriteLine($"На складе: {warehouse.Count} записей");
                
                RunGame(garages.First(), details, warehouse);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Детали: {ex.InnerException.Message}");
                }
            }
        }

        private void RunGame(Garage garage, List<Detail> details, List<DetailsGarage> warehouse)
        {
            var balance = garage.Balance.First();
            var carsProcessed = 0;

            Console.WriteLine($"Начальный баланс: {balance:C}");
            Console.WriteLine("Начинаем работу с клиентами...");

            while (balance > -1000 && carsProcessed < 20)
            {
                var brokenPart = details[random.Next(details.Count)];
                var repairCost = brokenPart.Price.First() * 1.3m;
                
                Console.WriteLine($"КЛИЕНТ {carsProcessed + 1}");
                Console.WriteLine($"Сломалось: {brokenPart.NameDetail}");
                Console.WriteLine($"Стоимость ремонта: {repairCost:C}");
                
                var inStock = warehouse.FirstOrDefault(w => w.DetailsId == brokenPart.Id);
                var hasPart = inStock != null && inStock.Count > 0;
                
                if (hasPart)
                {
                    Console.WriteLine($"На складе: {inStock.Count} шт.");
                    Console.WriteLine("1 - Починить");
                    Console.WriteLine("2 - Отказать");
                }
                else
                {
                    Console.WriteLine("На складе: нет");
                    Console.WriteLine("1 - Попробовать починить другой деталью");
                    Console.WriteLine("2 - Отказать");
                }

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    if (hasPart)
                    {
                        inStock.Count--;
                        balance += repairCost;
                        garage.Balance[0] = balance;
                        
                        Core.Context.SaveChanges();
                        Console.WriteLine($"Починили! +{repairCost:C}");
                    }
                    else
                    {
                        Console.WriteLine("Нужной детали нет! Ищем замену...");
                        
                        var availableParts = warehouse.Where(w => w.Count > 0).ToList();
                        
                        if (availableParts.Any())
                        {
                            var randomPart = availableParts[random.Next(availableParts.Count)];
                            var usedPart = details.First(p => p.Id == randomPart.DetailsId);
                            
                            randomPart.Count--;
                            
                            var penalty = repairCost * 2;
                            balance -= penalty;
                            garage.Balance[0] = balance;
                            
                            Core.Context.SaveChanges();
                            
                            Console.WriteLine("Использована неправильная деталь!");
                            Console.WriteLine($"Вместо {brokenPart.NameDetail} поставили {usedPart.NameDetail}");
                            Console.WriteLine($"Клиент вернулся недовольным!");
                            Console.WriteLine($"Штраф: -{penalty:C}");
                        }
                        else
                        {
                            var penalty = repairCost * 0.3m;
                            balance -= penalty;
                            garage.Balance[0] = balance;
                            
                            Core.Context.SaveChanges();
                            Console.WriteLine($"Не удалось выполнить ремонт. Штраф: -{penalty:C}");
                        }
                    }
                }
                else if (choice == "2")
                {
                    var penalty = repairCost * 0.3m;
                    balance -= penalty;
                    garage.Balance[0] = balance;
                    
                    Core.Context.SaveChanges();
                    Console.WriteLine($"Отказали клиенту. Штраф: -{penalty:C}");
                }

                carsProcessed++;
                Console.WriteLine($"Баланс: {balance:C}");

                if (carsProcessed % 2 == 0)
                {
                    ShowMenu(garage, details, warehouse);
                    warehouse = Core.Context.DetailsGarages.ToList();
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Игра окончена! Обработано клиентов: {carsProcessed}");
            Console.WriteLine($"Финальный баланс: {balance:C}");
        }

        private void ShowMenu(Garage garage, List<Detail> details, List<DetailsGarage> warehouse)
        {
            while (true)
            {
                Console.WriteLine("МЕНЮ:");
                Console.WriteLine("1 - Показать склад");
                Console.WriteLine("2 - Купить детали");
                Console.WriteLine("3 - Продолжить");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowWarehouse(warehouse, details);
                        break;
                    case "2":
                        BuyParts(garage, details, warehouse);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
        }

        private void BuyParts(Garage garage, List<Detail> details, List<DetailsGarage> warehouse)
        {
            Console.WriteLine("МАГАЗИН:");
            for (int i = 0; i < details.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {details[i].NameDetail} - {details[i].Price.First():C}");
            }

            Console.Write("Выбери деталь: ");
            if (int.TryParse(Console.ReadLine(), out int partIndex) && partIndex >= 1 && partIndex <= details.Count)
            {
                var part = details[partIndex - 1];
                Console.Write("Сколько: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var cost = part.Price.First() * quantity;
                    if (garage.Balance.First() >= cost)
                    {
                        garage.Balance[0] -= cost;
                        
                        var stockItem = warehouse.FirstOrDefault(w => w.DetailsId == part.Id);
                        if (stockItem != null)
                        {
                            stockItem.Count += quantity;
                        }
                        else
                        {
                            var newStock = new DetailsGarage
                            {
                                DetailsId = part.Id,
                                GarageId = garage.Id,
                                Count = quantity
                            };
                            Core.Context.DetailsGarages.Add(newStock);
                        }
                        
                        Core.Context.SaveChanges();
                        
                        Console.WriteLine($"Куплено {quantity} шт. {part.NameDetail}");
                        Console.WriteLine($"Списано: {cost:C}");
                        Console.WriteLine($"Остаток: {garage.Balance.First():C}");
                    }
                    else
                    {
                        Console.WriteLine($"Не хватает денег! Нужно: {cost:C}, есть: {garage.Balance.First():C}");
                    }
                }
                else
                {
                    Console.WriteLine("Неверное количество");
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор детали");
            }
        }

        private void ShowWarehouse(List<DetailsGarage> warehouse, List<Detail> details)
        {
            Console.WriteLine("СКЛАД:");
            var items = warehouse.Where(w => w.Count > 0).ToList();
            
            if (!items.Any())
            {
                Console.WriteLine("Склад пуст");
                return;
            }

            foreach (var item in items)
            {
                var part = details.First(p => p.Id == item.DetailsId);
                Console.WriteLine($"{part.NameDetail}: {item.Count} шт.");
            }
            
            var totalItems = items.Sum(i => i.Count);
            Console.WriteLine($"Всего деталей: {totalItems} шт.");
        }
    }
}
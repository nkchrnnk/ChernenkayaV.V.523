//Текстовая консольная пошаговая мини-игра-рогалик
//Сделайте текстовую игру для консоли. Игра пошаговая: на каждом ходу случается одно из двух — игрок находит сундук
    //или сталкивается со случайным врагом.
//Сущности
//Игрок
//Характеристики: здоровье (HP).
//Экипировка: одно оружие и одни доспехи одновременно.
//Враги
//Общие характеристики: здоровье, атака, защита.
//Особенности типов:
//Гоблин — имеет шанс нанести критический урон.
//Скелет — игнорирует защиту игрока.
//Маг — имеет шанс наложить «заморозку» (игрок пропускает следующий ход).
//Боссы
//ВВГ (раса Гоблин)Сохраняет: шанс критического удара.Особые характеристики:
//Здоровье ×2.0 от базового гоблина.Атака ×1.5.Защита ×1.2.Шанс крита +10%. к значению обычного гоблина.
//Ковальский (раса Скелет)Сохраняет: полностью игнорирует защиту игрока.
//Особые характеристики:Здоровье ×2.5.Атака ×1.3.Защита ×1.4.
//Архимаг C++ (раса Маг)Сохраняет: шанс наложить заморозку (пропуск хода).
//Особые характеристики:Здоровье ×1.8.Атака ×1.6.Защита ×1.1.Шанс заморозки +10%. к значению обычного мага.
//Пестов С-- (раса Скелет)Сохраняет: полностью игнорирует защиту игрока.
//Особые характеристики:Здоровье ×1.3.Атака ×1.8.Защита ×0.6.Шанс заморозки +15%. к значению обычного мага.
//Ход игры
//В начале каждого хода случайно определяется событие: сундук или враг (тип врага выбирается случайно из перечисленных)
//с шансом 50 на 50.
//Если выпал враг, начинается бой.
//Если выпал сундук, игрок получает случайный предмет.
//Каждые 10 ходов игроку попадается случайный босс.
//Бой
//Игрок всегда ходит первым.
//Ход игрока: выбрать Атаку или Защиту.
//Защита даёт 40% шанс полностью уклониться от следующей атаки врага.Если уклониться не удалось, срабатывает блок:
//уменьшение получаемого урона на 70–100% от характеристики защиты.
//После хода игрока враг всегда совершает атаку по игроку, применяя свои особенности (крит. шанс, игнор брони, заморозка).
//Сундук и предметы
//Из сундука может выпасть лечебное зелье, оружие или доспех (случайно).
//Лечебное зелье мгновенно полностью лечит игрока.
//При выпадении оружия или доспеха нужно:
//Показать характеристики нового предмета и текущей экипировки.
//Дать выбор: взять новый предмет (заменив текущий) или выбросить его.
//Сделайте так, чтобы все шансы и случайные величины (встреча сундука/врага, тип врага, крит.
//шанс/заморозка, величина блока 70–100%) определялись генератором случайных чисел.

    using System;
    using System.Collections.Generic;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
 
            Game game = new Game();
            game.Start();
        }
    }

    class Game
    {
        private Hero player;

        private Random random = new Random();

        private int turnCount = 0;

        private bool gameRunning = true;

        private EnemyFactory enemyFactory;
        private BattleSystem battleSystem;
        private ChestSystem chestSystem;
        private UIManager uiManager;

        public Game()
        {
            // старт предметы для игрока
            Armor startingArmor = new Armor(50, 10);
            Weapon startingWeapon = new Weapon(50, 15);

            // создание игрока
            player = new Hero(startingArmor, startingWeapon);
            player.HP = 100; 
            player.Defense = 5; 
            player.Damage = 10; 

            // создание всех систем игры
            enemyFactory = new EnemyFactory(random);
            battleSystem = new BattleSystem(random);
            chestSystem = new ChestSystem(random);
            uiManager = new UIManager();
        }
        public void Start()
        {
            Console.WriteLine("<<< ТЕКСТОВАЯ ПОШАГОВАЯ РОГАЛИК-ИГРА >>>");
            Console.WriteLine("Нажмите любую клавишу для начала...");
            Console.ReadKey();

            // пока игра не закончится
            while (gameRunning && player.HP > 0)
            {
                turnCount++;  
                Console.WriteLine($"\n--- Ход {turnCount} ---");

                uiManager.ShowPlayerStats(player);

                // каждый 10-й ход босс
                if (turnCount % 10 == 0)
                {
                    Enemy boss = enemyFactory.CreateBoss(turnCount);
                    Console.WriteLine($"\nПОЯВИЛСЯ БОСС: {boss.Name}!");
                    battleSystem.StartBattle(player, boss);
                }
                else
                {
                    // в обычный ход: 50% шанс врага, 50% шанс сундука
                    if (random.Next(2) == 0)
                    {
                        Enemy enemy = enemyFactory.CreateRandomEnemy();
                        Console.WriteLine($"\nВСТРЕЧА С ВРАГОМ: {enemy.Name}");
                        battleSystem.StartBattle(player, enemy);
                    }
                    else
                    {
                        Console.WriteLine($"\nВЫ НАШЛИ СУНДУК!");
                        chestSystem.OpenChest(player);
                    }
                }

                if (player.HP <= 0)
                {
                    Console.WriteLine("\nВЫ ПРОИГРАЛИ! Игра окончена.");
                    gameRunning = false;
                }
                else
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine($"\nИгра завершена. Пройдено ходов: {turnCount}");
        }
    }
    
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

    //класс врага
    class Enemy : Abstract
    {
        public string Name { get; set; }
        public List<string> Types { get; set; }
        public bool IsBoss { get; set; }
        public int CritChance { get; set; }
        public int FreezeChance { get; set; }
        public bool IgnoresArmor { get; set; }

        // конструктор врага
        public Enemy(string name, int hp, int defense, int damage, List<string> types,
            int critChance = 0, int freezeChance = 0, bool ignoresArmor = false)
        {
            Name = name;
            HP = hp;
            Defense = defense;
            Damage = damage;
            Types = types;
            CritChance = critChance;
            FreezeChance = freezeChance;
            IgnoresArmor = ignoresArmor;
        }

        // критический удар?
        public bool TryCriticalHit(Random random)
        {
            return random.Next(100) < CritChance;
        }

        // заморозка?
        public bool TryFreeze(Random random)
        {
            return random.Next(100) < FreezeChance;
        }
    }
class EnemyFactory
{
    private Random random;

    public EnemyFactory(Random rand)
    {
        random = rand;
    }

    //случайный обычный враг
    public Enemy CreateRandomEnemy()
    {
        int enemyType = random.Next(3);
        
        return enemyType switch
        {
            0 => CreateGoblin(),
            1 => CreateSkeleton(),
            2 => CreateMage(),
            _ => CreateGoblin()
        };
    }
      
    // босс
    public Enemy CreateBoss(int turnCount)
    {
        int bossType = random.Next(4);
        return bossType switch
        {
            0 => CreateGoblin(true),
            1 => CreateSkeleton(true),
            2 => CreateMage(true),
            3 => CreateSpecialSkeleton(true),
            _ => CreateGoblin(true)
        };
    }

    //гоблин
    private Enemy CreateGoblin(bool isBoss = false)
    {
        if (!isBoss)
        {
            return new Enemy("Гоблин", 30, 5, 8, new List<string> { "гоблин" }, critChance: 15);
        }
        else
        {
            return new Enemy("ВВГ (Босс Гоблин)", 60, 6, 12, new List<string> { "гоблин", "босс" }, critChance: 25);
        }
    }

    //скелет
    private Enemy CreateSkeleton(bool isBoss = false)
    {
        if (!isBoss)
        {
            return new Enemy("Скелет", 25, 3, 10, new List<string> { "скелет" }, ignoresArmor: true);
        }
        else
        {
            return new Enemy("Ковальский (Босс Скелет)", 63, 4, 13, new List<string> { "скелет", "босс" }, ignoresArmor: true);
        }
    }

    //маг
    private Enemy CreateMage(bool isBoss = false)
    {
        if (!isBoss)
        {
            return new Enemy("Маг", 20, 2, 12, new List<string> { "маг" }, freezeChance: 20);
        }
        else
        {
            return new Enemy("Архимаг C++ (Босс Маг)", 36, 2, 19, new List<string> { "маг", "босс" }, freezeChance: 30);
        }
    }

    //особый скелет
    private Enemy CreateSpecialSkeleton(bool isBoss = false)
    {
        return new Enemy("Пестов С-- (Особый Скелет)", 33, 3, 18, new List<string> { "скелет", "босs" },
                        freezeChance: 35, ignoresArmor: true);
    }
}
class BattleSystem
{
    private Random random;
    private bool playerFrozen = false;

    public BattleSystem(Random rand)
    {
        random = rand;
    }

    // начало
    public void StartBattle(Hero player, Enemy enemy)
    {
        Console.WriteLine($"Бой с {enemy.Name} (HP: {enemy.HP}, Урон: {enemy.Damage})");

        // бой пока кто-то не умрет
        while (enemy.HP > 0 && player.HP > 0)
        {
            // ход игрока, если не заморожен
            if (!playerFrozen)
            {
                PlayerTurn(player, enemy);
            }
            else
            {
                Console.WriteLine("Вы заморожены и пропускаете ход!");
                playerFrozen = false;
            }

            // проверка на смерть врага
            if (enemy.HP <= 0) break;

            EnemyTurn(player, enemy);
        }

        if (enemy.HP <= 0)
        {
            Console.WriteLine($"Вы победили {enemy.Name}!");
        }
    }

    // ход игрока
    private void PlayerTurn(Hero player, Enemy enemy)
    {
        Console.WriteLine("\nВаш ход:");
        Console.WriteLine("1 - Атака");
        Console.WriteLine("2 - Защита");
        Console.Write("Выберите действие: ");

        string input = Console.ReadLine();

        if (input == "1")
        {
            //базовый урон + урон оружия
            int damage = player.Damage + player.Weapon_.Damage;
            enemy.HP -= damage;
            player.Weapon_.Durability -= 1; // износ 
            Console.WriteLine($"Вы атаковали и нанесли {damage} урона!");
        }
        else if (input == "2")
        {
            // защита: 40% шанс уклониться
            if (random.Next(100) < 40)
            {
                Console.WriteLine("Вы успешно уклонились от атаки!");
                return;
            }
            else
            {
                // блок урона
                int blockPercent = random.Next(70, 101);
                int damageReduction = (int)(player.Defense * blockPercent / 100.0);
                Console.WriteLine($"Вы блокируете {blockPercent}% защиты ({damageReduction} урона)");
            }
        }
    }

    // ход врага
    private void EnemyTurn(Hero player, Enemy enemy)
    {
        Console.WriteLine($"\nХод {enemy.Name}:");

        int baseDamage = enemy.Damage;
        int finalDamage = baseDamage;
        
        // гоблин и крит урон
        if (enemy.Types.Contains("гоблин") && enemy.TryCriticalHit(random))
        {
            finalDamage = (int)(baseDamage * 1.5);
            Console.WriteLine($"Критический урон! Урон увеличен до {finalDamage}!");
        }

        // скелетам пофиг на броню
        if (enemy.Types.Contains("скелет") && enemy.IgnoresArmor)
        {
            Console.WriteLine($"{enemy.Name} игнорирует вашу защиту!");
        }
        else
        {
            // обычный враг: урон уменьшается на защиту
            int damageReduction = player.Defense + (int)player.Armor_.ArmorDefense;
            finalDamage = Math.Max(1, finalDamage - damageReduction);
        }

        // маги могут замораживать
        if (enemy.Types.Contains("маг") && enemy.TryFreeze(random))
        {
            playerFrozen = true;
            Console.WriteLine($"{enemy.Name} замораживает вас! Вы пропустите следующий ход.");
        }

        //урон игроку
        player.HP -= finalDamage;
        player.Armor_.Durability -= 1;  // броня износ

        Console.WriteLine($"{enemy.Name} атакует и наносит {finalDamage} урона!");
        Console.WriteLine($"Ваше HP: {player.HP}");
        
        CheckEquipmentDurability(player);
    }

    // проверка прочности экипировки
    private void CheckEquipmentDurability(Hero player)
    {
        if (player.Weapon_.Durability <= 0)
        {
            Console.WriteLine("Ваше оружие сломалось!");
            player.Weapon_ = new Weapon(0, 0);
        }

        if (player.Armor_.Durability <= 0)
        {
            Console.WriteLine("Ваши доспехи сломались!");
            player.Armor_ = new Armor(0, 0);
        }
    }
}
class ChestSystem
{
    private Random random;

    public ChestSystem(Random rand)
    {
        random = rand;
    }

    // открыть
    public void OpenChest(Hero player)
    {
        int itemType = random.Next(3);

        switch (itemType)
        {
            case 0:
                Console.WriteLine("Вы нашли зелье здоровья!");
                player.HP = 100;
                Console.WriteLine("HP полностью восстановлено!");
                break;

            case 1:
                Weapon newWeapon = GenerateRandomWeapon();
                Console.WriteLine($"Вы нашли новое оружие:");
                Console.WriteLine($"   Урон: {newWeapon.Damage}, Прочность: {newWeapon.Durability}");
                ShowWeaponComparison(player, newWeapon);
                break;

            case 2:
                Armor newArmor = GenerateRandomArmor();
                Console.WriteLine($"Вы нашли новые доспехи:");
                Console.WriteLine($"   Защита: {newArmor.ArmorDefense}, Прочность: {newArmor.Durability}");
                ShowArmorComparison(player, newArmor);
                break;
        }
    }

    //случайное оружие
    private Weapon GenerateRandomWeapon()
    {
        int damage = random.Next(10, 21);
        int durability = random.Next(30, 61);
        return new Weapon(durability, damage);
    }

    //случайная броня
    private Armor GenerateRandomArmor()
    {
        int defense = random.Next(8, 16); 
        int durability = random.Next(30, 61);
        return new Armor(durability, defense);
    }

    //сравнение оружия
    private void ShowWeaponComparison(Hero player, Weapon newWeapon)
    {
        Console.WriteLine($"Ваше текущее оружие - Урон: {player.Weapon_.Damage}, Прочность: {player.Weapon_.Durability}");
        Console.Write("Заменить оружие? (y/n): ");

        if (Console.ReadLine().ToLower() == "y")
        {
            player.Weapon_ = newWeapon;
            Console.WriteLine("Оружие заменено!");
        }
        else
        {
            Console.WriteLine("Оставили старое оружие.");
        }
    }

    //сравнение брони
    private void ShowArmorComparison(Hero player, Armor newArmor)
    {
        Console.WriteLine($"Ваши текущие доспехи - Защита: {player.Armor_.ArmorDefense}, Прочность: {player.Armor_.Durability}");
        Console.Write("Заменить доспехи? (y/n): ");

        if (Console.ReadLine().ToLower() == "y")
        {
            player.Armor_ = newArmor;
            Console.WriteLine("Доспехи заменены!");
        }
        else
        {
            Console.WriteLine("Оставили старые доспехи.");
        }
    }
}
class UIManager
{
    // статистика игрока
    public void ShowPlayerStats(Hero player)
    {
        Console.WriteLine($"\n<<< ИГРОК >>>");
        Console.WriteLine($"HP: {player.HP}");
        Console.WriteLine($"Атака: {player.Damage} + {player.Weapon_.Damage} (оружие)");
        Console.WriteLine($"Защита: {player.Defense} + {player.Armor_.ArmorDefense} (доспехи)");
        Console.WriteLine($"Прочность оружия: {player.Weapon_.Durability}");
        Console.WriteLine($"Прочность доспехов: {player.Armor_.Durability}");
    }
}

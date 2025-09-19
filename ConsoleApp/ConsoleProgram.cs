using System;
using ConfigManagement.Core;

namespace ConfigManagementConsole
{
    class Program
    {
        private static ConfigurationManager? _configManager;

        static void Main(string[] args)
        {
            _configManager = new ConfigurationManager();
            
            Console.WriteLine("=== Приложение управления конфигурацией ===");
            Console.WriteLine("Добро пожаловать!");
            Console.WriteLine();

            // Показать последнее сохраненное имя пользователя
            string lastUser = _configManager.GetSetting<string>("LastUser", "");
            if (!string.IsNullOrEmpty(lastUser))
            {
                Console.WriteLine($"Последний пользователь: {lastUser}");
                Console.WriteLine();
            }

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Поздороваться");
                Console.WriteLine("2. Показать информацию о системе");
                Console.WriteLine("3. Управление настройками");
                Console.WriteLine("4. Выход");
                Console.Write("Введите номер действия (1-4): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GreetUser();
                        break;
                    case "2":
                        ShowSystemInfo();
                        break;
                    case "3":
                        ManageSettings();
                        break;
                    case "4":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void GreetUser()
        {
            Console.Write("Введите ваше имя: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: Пожалуйста, введите ваше имя!");
                return;
            }

            // Сохранить имя пользователя в конфигурации
            _configManager?.SetSetting("LastUser", name);
            _configManager?.SetSetting("LastLoginTime", DateTime.Now.ToString());
            _configManager?.SaveConfiguration();

            Console.WriteLine($"Привет, {name}! Добро пожаловать в приложение!");
        }

        static void ShowSystemInfo()
        {
            Console.WriteLine("=== Информация о системе ===");
            Console.WriteLine($"Операционная система: {Environment.OSVersion}");
            Console.WriteLine($"Версия .NET: {Environment.Version}");
            Console.WriteLine($"Имя компьютера: {Environment.MachineName}");
            Console.WriteLine($"Имя пользователя: {Environment.UserName}");
            Console.WriteLine($"Рабочая директория: {Environment.CurrentDirectory}");
            Console.WriteLine($"Время запуска: {DateTime.Now}");
            
            // Показать сохраненную информацию
            string lastUser = _configManager?.GetSetting<string>("LastUser", "") ?? "";
            string lastLoginTime = _configManager?.GetSetting<string>("LastLoginTime", "") ?? "";
            
            if (!string.IsNullOrEmpty(lastUser))
            {
                Console.WriteLine($"Последний сохраненный пользователь: {lastUser}");
            }
            if (!string.IsNullOrEmpty(lastLoginTime))
            {
                Console.WriteLine($"Время последнего входа: {lastLoginTime}");
            }
        }

        static void ManageSettings()
        {
            while (true)
            {
                Console.WriteLine("=== Управление настройками ===");
                Console.WriteLine("1. Показать все настройки");
                Console.WriteLine("2. Добавить/изменить настройку");
                Console.WriteLine("3. Удалить настройку");
                Console.WriteLine("4. Очистить все настройки");
                Console.WriteLine("5. Вернуться в главное меню");
                Console.Write("Выберите действие (1-5): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllSettings();
                        break;
                    case "2":
                        AddOrUpdateSetting();
                        break;
                    case "3":
                        RemoveSetting();
                        break;
                    case "4":
                        ClearAllSettings();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void ShowAllSettings()
        {
            var settings = _configManager?.GetAllSettings();
            if (settings == null || settings.Count == 0)
            {
                Console.WriteLine("Настройки не найдены.");
                return;
            }

            Console.WriteLine("=== Все настройки ===");
            foreach (var setting in settings)
            {
                Console.WriteLine($"{setting.Key}: {setting.Value}");
            }
        }

        static void AddOrUpdateSetting()
        {
            Console.Write("Введите ключ настройки: ");
            string? key = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("Ошибка: Ключ не может быть пустым!");
                return;
            }

            Console.Write("Введите значение: ");
            string? value = Console.ReadLine();

            _configManager?.SetSetting(key, value ?? "");
            _configManager?.SaveConfiguration();
            
            Console.WriteLine("Настройка сохранена!");
        }

        static void RemoveSetting()
        {
            Console.Write("Введите ключ настройки для удаления: ");
            string? key = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("Ошибка: Ключ не может быть пустым!");
                return;
            }

            if (_configManager?.HasSetting(key) == true)
            {
                var settings = _configManager.GetAllSettings();
                settings.Remove(key);
                _configManager.ClearSettings();
                foreach (var setting in settings)
                {
                    _configManager.SetSetting(setting.Key, setting.Value);
                }
                _configManager.SaveConfiguration();
                Console.WriteLine("Настройка удалена!");
            }
            else
            {
                Console.WriteLine("Настройка не найдена!");
            }
        }

        static void ClearAllSettings()
        {
            Console.Write("Вы уверены, что хотите удалить все настройки? (y/n): ");
            string? confirmation = Console.ReadLine();
            
            if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
            {
                _configManager?.ClearSettings();
                _configManager?.SaveConfiguration();
                Console.WriteLine("Все настройки удалены!");
            }
            else
            {
                Console.WriteLine("Операция отменена.");
            }
        }
    }
}
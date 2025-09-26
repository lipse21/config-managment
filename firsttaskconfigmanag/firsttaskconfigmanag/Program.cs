using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace FirstTaskConfManag
{
    class Program
    {
        private static string _vfsRootPath = "";
        private static string _startupScriptPath = "";
        private static bool _isInteractiveMode = true;

        internal static void Main(string[] args)
        {
            Console.WriteLine("=== Конфигурационный менеджер (REPL прототип) ===\n");

            Console.WriteLine("=== Параметры командной строки ===");
            Console.WriteLine($"Количество параметров: {args.Length}");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"[{i}] {args[i]}");
            }
            Console.WriteLine();

            try
            {
                ParseCommandLineArgs(args);

                if (!string.IsNullOrEmpty(_startupScriptPath))
                {
                    _isInteractiveMode = false;
                    ExecuteStartupScript(_startupScriptPath);
                }
                else
                {
                    RunInteractiveMode();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        private static void ParseCommandLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--vfs-root":
                    case "-v":
                        if (i + 1 < args.Length)
                        {
                            _vfsRootPath = args[++i];
                            Console.WriteLine($"[DEBUG] Путь к VFS: {_vfsRootPath}");
                        }
                        else
                        {
                            throw new ArgumentException("Параметр --vfs-root требует значение");
                        }
                        break;

                    case "--script":
                    case "-s":
                        if (i + 1 < args.Length)
                        {
                            _startupScriptPath = args[++i];
                            Console.WriteLine($"[DEBUG] Путь к стартовому скрипту: {_startupScriptPath}");
                        }
                        else
                        {
                            throw new ArgumentException("Параметр --script требует значение");
                        }
                        break;

                    case "--help":
                    case "-h":
                        ShowHelp();
                        Environment.Exit(0);
                        break;

                    default:
                        if (args[i].StartsWith("-"))
                        {
                            throw new ArgumentException($"Неизвестный параметр: {args[i]}");
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(_startupScriptPath) && string.IsNullOrEmpty(_vfsRootPath))
            {
                throw new ArgumentException("Для выполнения скрипта необходимо указать путь к VFS");
            }
        }

        private static void ExecuteStartupScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Стартовый скрипт не найден: {scriptPath}");
            }

            Console.WriteLine($"=== Выполнение стартового скрипта: {scriptPath} ===\n");

            string[] lines = File.ReadAllLines(scriptPath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();


                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(GeneratePrompt());
                Console.ResetColor();
                Console.WriteLine(line);

                try
                {
                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0)
                        continue;

                    string command = tokens[0].ToLower();
                    string[] arguments = tokens.Skip(1).ToArray();

                    bool continueExecution = ExecuteCommand(command, arguments);

                    if (!continueExecution)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Скрипт остановлен на строке {i + 1} из-за ошибки");
                        Console.ResetColor();
                        Environment.Exit(1);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка в строке {i + 1}: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("Скрипт остановлен при первой ошибке");
                    Environment.Exit(1);
                }
            }

            Console.WriteLine("\n=== Скрипт успешно выполнен ===");
        }

        private static void RunInteractiveMode()
        {
            Console.WriteLine("Режим: Интерактивный (REPL)\n");

            while (true)
            {
                string prompt = GeneratePrompt();
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0)
                    continue;

                string command = tokens[0].ToLower();
                string[] arguments = tokens.Skip(1).ToArray();

                try
                {
                    bool continueExecution = ExecuteCommand(command, arguments);
                    if (!continueExecution)
                        break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        private static bool ExecuteCommand(string command, string[] arguments)
        {
            switch (command)
            {
                case "ls":
                    Console.WriteLine($"[ЗАГЛУШКА] Вызвана команда: ls");
                    if (!string.IsNullOrEmpty(_vfsRootPath))
                    {
                        Console.WriteLine($"[DEBUG] Корневой путь VFS: {_vfsRootPath}");
                    }
                    if (arguments.Length > 0)
                        Console.WriteLine($"Аргументы: {string.Join(", ", arguments)}");
                    break;

                case "cd":
                    if (arguments.Length != 1)
                        throw new ArgumentException("Команда 'cd' требует ровно один аргумент — путь.");
                    Console.WriteLine($"[ЗАГЛУШКА] Вызвана команда: cd {arguments[0]}");
                    break;

                case "pwd":
                    Console.WriteLine($"[ЗАГЛУШКА] Вызвана команда: pwd");
                    Console.WriteLine($"[DEBUG] Текущий VFS путь: {_vfsRootPath}");
                    break;

                case "exit":
                    Console.WriteLine("Выход...");
                    return false;

                default:
                    throw new InvalidOperationException($"Неизвестная команда: {command}");
            }

            return true;
        }

        private static string GeneratePrompt()
        {
            try
            {
                string username = Environment.UserName;
                string hostname = Environment.MachineName;
                string modeIndicator = _isInteractiveMode ? "REPL" : "SCRIPT";
                return $"{username}@{hostname}[{modeIndicator}]:~$ ";
            }
            catch
            {
                return "user@host[UNKNOWN]:~$ ";
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Использование: FirstTaskConfManag [ПАРАМЕТРЫ]");
            Console.WriteLine();
            Console.WriteLine("Параметры:");
            Console.WriteLine("  -v, --vfs-root ПУТЬ    Путь к физическому расположению VFS");
            Console.WriteLine("  -s, --script ПУТЬ      Путь к стартовому скрипту для выполнения");
            Console.WriteLine("  -h, --help             Показать эту справку");
            Console.WriteLine();
            Console.WriteLine("Примеры:");
            Console.WriteLine("  FirstTaskConfManag -v C:\\VFS -s startup.txt");
            Console.WriteLine("  FirstTaskConfManag --vfs-root /home/user/vfs --script init.cfg");
        }
    }
}
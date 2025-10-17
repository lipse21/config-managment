using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FirstTaskConfManag
{
    class VfsNode
    {
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string Name => Path.Split('/').Last() ?? "";

        public override string ToString() => IsDirectory ? $"{Name}/" : Name;
       
    }

    class Program
    {
        private static string _vfsCsvPath = "";
        private static string _startupScriptPath = "";
        private static bool _isInteractiveMode = true;
        private static Dictionary<string, VfsNode> _vfs = new();
        private static string _currentPath = "/";
        private static readonly DateTime _startTime = DateTime.Now;

        internal static void Main(string[] args)
        {
            Console.WriteLine("=== Конфигурационный менеджер (VFS этап) ===\n");

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

                if (string.IsNullOrEmpty(_vfsCsvPath))
                    throw new ArgumentException("Не указан путь к VFS (CSV-файл). Используйте --vfs-root.");

                LoadVfsFromCsv(_vfsCsvPath);

                if (_vfs.TryGetValue("/motd.txt", out var motdNode))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Encoding.UTF8.GetString(motdNode.Content));
                    Console.ResetColor();
                }

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
                            _vfsCsvPath = args[++i];
                            Console.WriteLine($"[DEBUG] Путь к VFS CSV: {_vfsCsvPath}");
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

            if (!string.IsNullOrEmpty(_startupScriptPath) && string.IsNullOrEmpty(_vfsCsvPath))
            {
                throw new ArgumentException("Для выполнения скрипта необходимо указать путь к VFS");
            }
        }

        private static void LoadVfsFromCsv(string csvPath)
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"Файл VFS не найден: {csvPath}");

            _vfs.Clear();
            var lines = File.ReadAllLines(csvPath);
            bool isFirst = true;

            foreach (var line in lines)
            {
                if (isFirst && line.StartsWith("path,"))
                {
                    isFirst = false;
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length < 3) continue;

                string path = parts[0];
                string contentB64 = parts[1];
                bool isDir = parts[2].Equals("true", StringComparison.OrdinalIgnoreCase);

                var node = new VfsNode
                {
                    Path = path,
                    IsDirectory = isDir,
                    Content = isDir ? Array.Empty<byte>() : Convert.FromBase64String(contentB64)
                };

                _vfs[path] = node;
            }

            Console.WriteLine($"[DEBUG] Загружено {_vfs.Count} элементов VFS из {csvPath}");
        }

        private static void SaveVfsToCsv(string outputPath)
        {
            using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
            writer.WriteLine("path,content_base64,is_directory");

            foreach (var kvp in _vfs.OrderBy(k => k.Key))
            {
                string contentB64 = kvp.Value.IsDirectory ? "" : Convert.ToBase64String(kvp.Value.Content);
                string isDir = kvp.Value.IsDirectory ? "true" : "false";
                writer.WriteLine($"{kvp.Key},{contentB64},{isDir}");
            }

            Console.WriteLine($"[INFO] VFS сохранена в {outputPath}");
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
                    if (!continueExecution && command == "exit")
                    {
      
                        Console.WriteLine("\n=== Скрипт успешно выполнен ===");
                        Environment.Exit(0); 
                    }
                    else if (!continueExecution)
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
                    ListDirectory(_currentPath, arguments);
                    break;

                case "cd":
                    if (arguments.Length != 1)
                        throw new ArgumentException("Команда 'cd' требует ровно один аргумент — путь.");
                    ChangeDirectory(arguments[0]);
                    break;

                case "pwd":
                    Console.WriteLine(_currentPath);
                    break;

                case "vfs-save":
                    if (arguments.Length != 1)
                        throw new ArgumentException("Команда 'vfs-save' требует путь для сохранения.");
                    SaveVfsToCsv(arguments[0]);
                    break;

                case "exit":
                    Console.WriteLine("Выход...");
                    return false;

                case "uname":                   
                    Console.WriteLine("Linux");
                    break;

                case "uptime":
                    ShowUptime();
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестная команда: {command}");
            }

            return true;
        }

        private static void ListDirectory(string basePath, string[] args)
        {
            string targetPath = args.Length > 0 ? ResolvePath(args[0]) : basePath;
            NormalizePath(ref targetPath);

            if (!_vfs.TryGetValue(targetPath, out var node) || !node.IsDirectory)
            {
                throw new InvalidOperationException($"Нет такого каталога: {targetPath}");
            }

            var children = _vfs
                .Where(kvp => kvp.Key != "/" && kvp.Key.StartsWith(targetPath + "/"))
                .Select(kvp => kvp.Key.Substring(targetPath.Length + 1))
                .Where(p => !p.Contains('/'))
                .Select(p => targetPath == "/" ? "/" + p : targetPath + "/" + p)
                .Where(p => _vfs.ContainsKey(p))
                .Select(p => _vfs[p])
                .OrderBy(n => n.Name)
                .ThenByDescending(n => n.IsDirectory)
                .ToList();

            foreach (var child in children)
            {
                Console.WriteLine(child);
            }
        }

        private static void ChangeDirectory(string pathArg)
        {
            string newPath = ResolvePath(pathArg);
            NormalizePath(ref newPath);

            if (!_vfs.TryGetValue(newPath, out var node) || !node.IsDirectory)
            {
                throw new InvalidOperationException($"Нет такого каталога: {newPath}");
            }

            _currentPath = newPath;
        }

        private static string ResolvePath(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
                return _currentPath;

            if (inputPath == ".")
                return _currentPath;

            if (inputPath == "..")
            {
                if (_currentPath == "/") return "/";
                var parts = _currentPath.Trim('/').Split('/');
                return "/" + string.Join("/", parts.Take(parts.Length - 1));
            }

            if (inputPath.StartsWith("/"))
                return inputPath;

            return _currentPath == "/" ? "/" + inputPath : _currentPath + "/" + inputPath;
        }

        private static void NormalizePath(ref string path)
        {
            if (string.IsNullOrEmpty(path)) path = "/";
            if (path != "/" && path.EndsWith("/"))
                path = path.TrimEnd('/');
        }

        private static string GeneratePrompt()
        {
            try
            {
                string username = Environment.UserName;
                string hostname = Environment.MachineName;
                string modeIndicator = _isInteractiveMode ? "REPL" : "SCRIPT";
                string displayPath = _currentPath == "/" ? "~" : _currentPath;
                return $"{username}@{hostname}[{modeIndicator}]:{displayPath}$ ";
            }
            catch
            {
                return "user@host[UNKNOWN]:~$ ";
            }
        }

        private static void ShowUptime()
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            string uptimeStr;

            if (elapsed.TotalHours >= 1)
            {
                int hours = (int)elapsed.TotalHours;
                int minutes = elapsed.Minutes;
                uptimeStr = minutes > 0
                    ? $"up {hours} hour{(hours == 1 ? "" : "s")}, {minutes} minute{(minutes == 1 ? "" : "s")}"
                    : $"up {hours} hour{(hours == 1 ? "" : "s")}";
            }
            else if (elapsed.TotalMinutes >= 1)
            {
                int minutes = (int)elapsed.TotalMinutes;
                uptimeStr = $"up {minutes} minute{(minutes == 1 ? "" : "s")}";
            }
            else
            {
                int seconds = (int)elapsed.TotalSeconds;
                uptimeStr = $"up {seconds} second{(seconds == 1 ? "" : "s")}";
            }

            Console.WriteLine(uptimeStr);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Использование: FirstTaskConfManag [ПАРАМЕТРЫ]");
            Console.WriteLine();
            Console.WriteLine("Параметры:");
            Console.WriteLine("  -v, --vfs-root ПУТЬ    Путь к CSV-файлу с виртуальной ФС");
            Console.WriteLine("  -s, --script ПУТЬ      Путь к стартовому скрипту для выполнения");
            Console.WriteLine("  -h, --help             Показать эту справку");
            Console.WriteLine();
            Console.WriteLine("Поддерживаемые команды:");
            Console.WriteLine("  ls [путь]              Показать содержимое каталога");
            Console.WriteLine("  cd путь                Сменить каталог");
            Console.WriteLine("  pwd                    Показать текущий путь");
            Console.WriteLine("  uname                  Показать имя ОС (эмуляция)");
            Console.WriteLine("  uptime                 Показать время с запуска эмулятора");
            Console.WriteLine("  vfs-save путь          Сохранить VFS в CSV");
            Console.WriteLine("  exit                   Выйти");
        }
    }
}

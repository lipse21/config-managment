using System;
using System.Diagnostics;
using System.Linq;

namespace FirstTaskConfManag
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=== Конфигурационный менеджер (REPL прототип) ===\n");

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
                    switch (command)
                    {
                        case "ls":
                            Console.WriteLine($"[ЗАГЛУШКА] Вызвана команда: ls");
                            if (arguments.Length > 0)
                                Console.WriteLine($"Аргументы: {string.Join(", ", arguments)}");
                            break;

                        case "cd":
                            if (arguments.Length != 1)
                                throw new ArgumentException("Команда 'cd' требует ровно один аргумент — путь.");
                            Console.WriteLine($"[ЗАГЛУШКА] Вызвана команда: cd {arguments[0]}");
                            break;

                        case "exit":
                            Console.WriteLine("Выход из REPL...");
                            return;

                        default:
                            throw new InvalidOperationException($"Неизвестная команда: {command}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        private static string GeneratePrompt()
        {
            try
            {
                string username = Environment.UserName;
                string hostname = Environment.MachineName;
                return $"{username}@{hostname}:~$ ";
            }
            catch
            {
                return "user@host:~$ ";
            }
        }
    }
}
# Config Management Application

Приложение управления конфигурацией на C# с поддержкой как Windows Forms, так и консольного интерфейса.

## Структура проекта

- **WindowsFormsApp/** - Windows Forms приложение для Windows
- **ConsoleApp/** - Консольное приложение (кроссплатформенное)
- **ConfigManagement.sln** - Файл решения Visual Studio

## Требования

- .NET 8.0 или выше
- Для Windows Forms: Windows с установленным .NET Desktop Runtime

## Запуск приложений

### Консольное приложение (работает на всех платформах)

```bash
cd ConsoleApp
dotnet run
```

### Windows Forms приложение (только Windows)

```bash
cd WindowsFormsApp
dotnet run
```

Или откройте `ConfigManagement.sln` в Visual Studio и запустите нужный проект.

## Функциональность

### Консольное приложение
- Интерактивное меню
- Приветствие пользователя
- Отображение информации о системе
- Поддержка русского языка

### Windows Forms приложение
- Графический интерфейс пользователя
- Поля ввода и кнопки
- Диалоговые окна
- Поддержка русского языка

## Сборка

Для сборки всего решения:

```bash
dotnet build ConfigManagement.sln
```

Для сборки отдельного проекта:

```bash
dotnet build ConsoleApp/ConfigManagementConsole.csproj
dotnet build WindowsFormsApp/ConfigManagementApp.csproj  # только на Windows
```

## Разработка

Проект создан для демонстрации возможностей C# в создании как консольных, так и оконных приложений. Включает:

- Современный C# с использованием .NET 8
- Обработка пользовательского ввода
- Валидация данных
- Локализация на русском языке
- Архитектура для расширения функциональности

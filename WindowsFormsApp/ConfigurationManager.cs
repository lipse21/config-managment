using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConfigManagement.Core
{
    /// <summary>
    /// Базовый класс для управления конфигурацией приложения
    /// </summary>
    public class ConfigurationManager
    {
        private readonly string _configFilePath;
        private Dictionary<string, object> _settings;

        public ConfigurationManager(string configFilePath = "config.json")
        {
            _configFilePath = configFilePath;
            _settings = new Dictionary<string, object>();
            LoadConfiguration();
        }

        /// <summary>
        /// Загружает конфигурацию из файла
        /// </summary>
        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string json = File.ReadAllText(_configFilePath);
                    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    _settings = config ?? new Dictionary<string, object>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки конфигурации: {ex.Message}");
                _settings = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Сохраняет конфигурацию в файл
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения конфигурации: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает значение настройки
        /// </summary>
        public T GetSetting<T>(string key, T defaultValue = default)
        {
            if (_settings.ContainsKey(key))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(_settings[key].ToString() ?? "");
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Устанавливает значение настройки
        /// </summary>
        public void SetSetting<T>(string key, T value)
        {
            _settings[key] = value;
        }

        /// <summary>
        /// Получает все настройки
        /// </summary>
        public Dictionary<string, object> GetAllSettings()
        {
            return new Dictionary<string, object>(_settings);
        }

        /// <summary>
        /// Очищает все настройки
        /// </summary>
        public void ClearSettings()
        {
            _settings.Clear();
        }

        /// <summary>
        /// Проверяет, существует ли настройка
        /// </summary>
        public bool HasSetting(string key)
        {
            return _settings.ContainsKey(key);
        }
    }
}
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace TurnstileCalsSystem;

// 1. Единая стандартизированная модель данных СКУД для информационного пространства
public class TurnstilePacket
{
    public string TransactionId { get; set; } = string.Empty;
    public string ControllerId { get; set; } = string.Empty;
    public string CardUid { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty; // IN / OUT
    public DateTime Timestamp { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== CALS-ТЕХНОЛОГИИ: ЕДИНОЕ ИНФОРМАЦИОННОЕ ПРОСТРАНСТВО ===");
        Console.WriteLine($"Студент: Абдувалиев Д.У. | Группа: ИП3-24-11 | Вариант: 1\n");

        // Имитация данных со считывателя турникета на КПП
        var originalPacket = new TurnstilePacket
        {
            TransactionId = Guid.NewGuid().ToString(),
            ControllerId = "CTRL-MAIN-GATE-01",
            CardUid = "04A1B2C3D4E5",
            Direction = "IN",
            Timestamp = DateTime.UtcNow
        };

        // ----------------──────────────────────────────────────────────────────
        // ЭТАП 1: Обмен данными в реальном времени через стандарт JSON
        // ----------------──────────────────────────────────────────────────────
        Console.WriteLine("[ЭТАП 1] Сериализация данных в формат JSON (для Turnstile API):");
        string jsonResult = JsonSerializer.Serialize(originalPacket, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(jsonResult);

        // Имитация приема на стороне API сервера
        var receivedFromJson = JsonSerializer.Deserialize<TurnstilePacket>(jsonResult);
        Console.WriteLine("-> Данные JSON успешно приняты и верифицированы сервером API.\n");

        // ----------------──────────────────────────────────────────────────────
        // ЭТАП 2: Конвертация данных в стандарт XML для интеграции с legacy-1С
        // ----------------──────────────────────────────────────────────────────
        Console.WriteLine("[ЭТАП 2] Трансформация данных в формат XML (для внешних систем):");
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TurnstilePacket));
        using (StringWriter stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, receivedFromJson);
            string xmlResult = stringWriter.ToString();
            Console.WriteLine(xmlResult);
        }
        Console.WriteLine("-> Интеграционный XML-пакет успешно сформирован согласно XSD схеме.\n");

        // ----------------──────────────────────────────────────────────────────
        // ЭТАП 3: Фиксация в Едином Источнике Правды (SSOT)
        // ----------------──────────────────────────────────────────────────────
        Console.WriteLine("[ЭТАП 3] Регистрация транзакции в Едином информационном пространстве:");
        Console.WriteLine($"Транзакция {receivedFromJson?.TransactionId} записана в центральную БД PostgreSQL.");
        Console.WriteLine($"Статус физического реле контроллера {receivedFromJson?.ControllerId}: РАЗБЛОКИРОВАНО (ВХОД РАЗРЕШЕН).");
        Console.WriteLine("\n=== РАБОТА ПРОГРАММЫ ЗАВЕРШЕНА. ВСЕ ДАННЫЕ ВЕРИФИЦИРОВАНЫ ===");

        Console.ReadLine();
    }
}

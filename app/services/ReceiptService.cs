using System.Text;
using game_platform.net.dto.response;
using game_platform.net.interfaces;
using game_platform.net.model;

namespace game_platform.net.services;

public class ReceiptService : IReceiptService {
    public ReceiptFile BuildReceipt(Purchase purchase) {
        var builder = new StringBuilder();
        builder.AppendLine("GAME_PLATFORM.NET");
        builder.AppendLine("Кассовый чек");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"Покупка: {purchase.Id}");
        builder.AppendLine($"Дата (UTC): {purchase.CreatedAtUtc:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine("Клиент:");
        builder.AppendLine($"- ({purchase.UserId})");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine("Игры (id):");
        for (int i = 0; i < purchase.GameIds.Length; i++) {
            builder.AppendLine(
                $"{purchase.GameIds[i]}"
            );
        }
        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"ИТОГО: {purchase.Total:F2} RUB");
        builder.AppendLine("Спасибо за покупку!");
        return new ReceiptFile {
            FileName = $"receipt-{purchase.Id}.txt",
            ContentType = "text/plain; charset=utf-8",
            Content = Encoding.UTF8.GetBytes(builder.ToString())
        };
    }
}
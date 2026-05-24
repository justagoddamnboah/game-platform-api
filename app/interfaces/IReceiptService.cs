using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IReceiptService {
    ReceiptFile BuildReceipt(Purchase purchase);
}
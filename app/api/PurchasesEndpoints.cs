using game_platform.net.dto;
using game_platform.net.dto.request;
using game_platform.net.dto.response;
using game_platform.net.interfaces;

namespace game_platform.net.api;

public static class PurchasesEndpoints {
    public static RouteGroupBuilder MapPurchasesEndpoints(this RouteGroupBuilder api) {
        var group = api.MapGroup("/purchases").WithTags("Purchases");

        group.MapGet("/", async (IPurchaseService purchases, IMapper mapper) => {
                var result = await purchases.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список покупок")
            .WithDescription("Возвращает историю покупок в упрощенном виде: id пользователя, массив id игр, сумма.")
            .Produces<IEnumerable<PurchaseResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{purchaseId:guid}/receipt",
                async (Guid purchaseId, IPurchaseService purchases, IReceiptService receipts) => {
                var purchase = await purchases.GetByIdAsync(purchaseId);
                if (purchase is null) {
                    return Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." });
                }
                var receipt = receipts.BuildReceipt(purchase);
                return Results.File(receipt.Content, receipt.ContentType, receipt.FileName);
            })
            .WithSummary("Скачать чек по покупке")
            .WithDescription("Формирует и отдает текстовый чек в виде файла .txt.")
            .Produces(StatusCodes.Status200OK, contentType: "text/plain")
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{purchaseId:guid}",
                async (Guid purchaseId, IPurchaseService purchases, IMapper mapper) => {
                var purchase = await purchases.GetByIdAsync(purchaseId);
                return purchase is null
                    ? Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." })
                    : Results.Ok(mapper.Map(purchase));
            })
            .WithSummary("Получить покупку по идентификатору")
            .WithDescription("Возвращает одну покупку: пользователь, список id игр, сумма.")
            .Produces<PurchaseResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreatePurchaseRequest request, IPurchaseService purchases, IMapper mapper) => {
                try {
                    var purchase = await purchases.CreateAsync(request);
                    return Results.Created($"/api/purchases/{purchase.Id}", mapper.Map(purchase));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Создать покупку")
            .WithDescription("Создаёт покупку по массиву id игр.")
            .Produces<PurchaseResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{purchaseId:guid}",
                async (Guid purchaseId, CreatePurchaseRequest request, IPurchaseService purchases, IMapper mapper) => {
                try {
                    var purchase = await purchases.UpdateAsync(purchaseId, request);
                    return purchase is null
                        ? Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." })
                        : Results.Ok(mapper.Map(purchase));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                } 
                })
            .WithSummary("Изменить покупку")
            .WithDescription("Откатывает прежние списания и бонусы, затем применяет новый состав покупки.")
            .Produces<PurchaseResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{purchaseId:guid}",
                async (Guid purchaseId, IPurchaseService purchases) => {
                var deleted = await purchases.DeleteAsync(purchaseId);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." });
                })
            .WithSummary("Удалить покупку")
            .WithDescription("Возвращает товары на склад и списывает начисленные за покупку бонусы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        return api;
    }
}
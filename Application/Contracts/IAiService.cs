namespace e_commerce_web_customer.Application.Contracts;

public interface IAiService
{
    Task<AiChatResult> AskAsync(
        string question,
        IReadOnlyList<AiChatMessage> history,
        CancellationToken cancellationToken = default);
}

public sealed record AiChatMessage(string Role, string Message);

public sealed record AiChatResult(
    string Reply,
    IReadOnlyList<AiSuggestedProduct> Products);

public sealed record AiSuggestedProduct(
    long Id,
    string Name,
    decimal Price,
    string ImageUrl,
    string? CategoryName,
    string Slug,
    int Quantity);

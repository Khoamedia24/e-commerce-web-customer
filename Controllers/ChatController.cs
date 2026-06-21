using System.ComponentModel.DataAnnotations;
using e_commerce_web_customer.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatController(
    IAiService aiService,
    ILogger<ChatController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AiChatResponse>> Ask(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new AiChatResponse(false, null, [], "Vui lòng nhập câu hỏi."));
        }

        var history = (request.History ?? [])
            .TakeLast(8)
            .Where(item => !string.IsNullOrWhiteSpace(item.Message))
            .Select(item => new AiChatMessage(item.Role, item.Message))
            .ToList();

        try
        {
            var result = await aiService.AskAsync(
                request.Message.Trim(),
                history,
                cancellationToken);
            var products = result.Products.Select(product => new AiSuggestedProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.ImageUrl,
                product.CategoryName,
                Url.Action("Details", "Product", new { slug = product.Slug })
                    ?? $"/product/{Uri.EscapeDataString(product.Slug)}"))
                .ToList();

            return new AiChatResponse(true, result.Reply, products);
        }
        catch (InvalidOperationException exception)
        {
            logger.LogWarning(exception, "AI chat request could not be completed.");
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new AiChatResponse(false, null, [], exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected AI chat error.");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new AiChatResponse(false, null, [], "Chatbox đang gặp lỗi. Vui lòng thử lại sau."));
        }
    }
}

public sealed record ChatRequest(
    [Required, StringLength(1000)] string Message,
    [MaxLength(8)] IReadOnlyList<ChatHistoryItem>? History);

public sealed record ChatHistoryItem(
    [Required, RegularExpression("^(user|assistant)$")] string Role,
    [Required, StringLength(2000)] string Message);

public sealed record AiChatResponse(
    bool Success,
    string? Reply,
    IReadOnlyList<AiSuggestedProductDto> Products,
    string? Message = null);

public sealed record AiSuggestedProductDto(
    long Id,
    string Name,
    decimal Price,
    string ImageUrl,
    string? CategoryName,
    string DetailUrl);

using Microsoft.Extensions.AI;

namespace Catalog.Services;

public class ProductAIService(IChatClient chatClient, CatalogDbContext dbContext)
{
    public async Task<string> SupportAsync(string userQuery, CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Select(p => new { p.Id, p.Name, p.Price })
            .ToListAsync(cancellationToken);

        string productCatalog = products.Count == 0
            ? "- (No products available)"
            : string.Join("\n", products.Select(p => $"- Id:{p.Id} Name:\"{p.Name}\" Price:{p.Price:C}"));

        var systemPrompt = $"""
                            You are a helpful assistant for an outdoor camping products store.

                            Rules:
                            1. Only answer questions related to outdoor camping or the product catalog. If unrelated say exactly: "I only answer questions about outdoor camping products."
                            2. Be concise and a little funny (light humor).
                            3. If you don't know, reply exactly: "I don't know that."
                            4. Do not store memory of the conversation.
                            5. When appropriate (most user questions), end with ONE relevant product recommendation from the catalog below.
                               - Pick the product that best matches the user's intent; if none clearly match, pick a random one.
                               - Format the recommendation on a new final line as:
                                 Recommendation: <Product Name> - <Price>
                            6. Do NOT invent products not in the catalog.

                            Product Catalog:
                            {productCatalog}
                            """;

        var chatHistory = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userQuery)
        };

        var response = await chatClient.GetResponseAsync(chatHistory, cancellationToken: cancellationToken);

        return response.Text ?? "No description available.";
    }
    
}
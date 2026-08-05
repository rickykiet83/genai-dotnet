using System.ClientModel;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

namespace Catalog.Extensions;

public static class AIServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredChatClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = GetRequiredSetting(configuration, "AI:Provider");

        switch (provider.ToLowerInvariant())
        {
            case "openai":
                services.AddChatClient(CreateOpenAIChatClient(configuration));
                services.AddEmbeddingGenerator(CreateOpenAIEmbeddingClient(configuration));
                break;
            case "ollama":
                services.AddChatClient(CreateOllamaChatClient(configuration));
                break;
            default:
                throw new InvalidOperationException(
                    $"Unsupported AI provider '{provider}'. Supported values are 'OpenAI' and 'Ollama'.");
        }

        return services;
    }

    private static IChatClient CreateOpenAIChatClient(IConfiguration configuration)
    {
        var credential = new ApiKeyCredential(GetRequiredSetting(configuration, "AI:OpenAI:ApiKey"));
        var model = GetRequiredSetting(configuration, "AI:OpenAI:Model");
        
        return new OpenAIClient(credential)
            .GetChatClient(model)
            .AsIChatClient();
    }
    
    private static IEmbeddingGenerator<string, Embedding<float>> CreateOpenAIEmbeddingClient(IConfiguration configuration)
    {
        var credential = new ApiKeyCredential(GetRequiredSetting(configuration, "AI:OpenAI:ApiKey"));
        var embeddingModel = GetRequiredSetting(configuration, "AI:OpenAI:EmbeddingModel");

        return new OpenAIClient(credential)
            .GetEmbeddingClient(embeddingModel)
            .AsIEmbeddingGenerator();
    }

    private static IChatClient CreateOllamaChatClient(IConfiguration configuration)
    {
        var endpoint = GetRequiredSetting(configuration, "AI:Ollama:Endpoint");
        var model = GetRequiredSetting(configuration, "AI:Ollama:Model");

        return new OllamaApiClient(new Uri(endpoint), model);
    }

    private static string GetRequiredSetting(IConfiguration configuration, string key) =>
        configuration[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"Missing configuration: {key}.");
}

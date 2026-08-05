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
        IChatClient chatClient = GetRequiredSetting(configuration, "AI:Provider").ToLowerInvariant() switch
        {
            "openai" => CreateOpenAIChatClient(configuration),
            "ollama" => CreateOllamaChatClient(configuration),
            var provider => throw new InvalidOperationException(
                $"Unsupported AI provider '{provider}'. Supported values are 'OpenAI' and 'Ollama'.")
        };

        services.AddChatClient(chatClient);
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

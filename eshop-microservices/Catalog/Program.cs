
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();

// Add AI Chat Client
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["OpenAI:ApiKey"]
                                      ?? throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey."));

var model = config["OpenAI:Model"] ??
            throw new InvalidOperationException("Missing configuration: OpenAI:Model.");

// Create a chat client
var openAiClient = new OpenAIClient(credential);
IChatClient chatClient = openAiClient
    .GetChatClient(model)
    .AsIChatClient();

builder.Services.AddChatClient(chatClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();

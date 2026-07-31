
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductService>();

// Add AI Chat Client
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ??
                                      throw new InvalidOperationException(
                                          "Missing configuration: GitHubModels:Token."));
var endpoint = config["GitHubModels:Endpoint"] ??
               throw new InvalidOperationException("Missing configuration: GitHubModels:Endpoint.");
var model = config["GitHubModels:Model"] ??
            throw new InvalidOperationException("Missing configuration: GitHubModels:Model.");
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri(endpoint)
};

// create a chat client
IChatClient chatClient =
    new OpenAIClient(credential, options).GetChatClient(model).AsIChatClient();

builder.Services.AddChatClient(chatClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();

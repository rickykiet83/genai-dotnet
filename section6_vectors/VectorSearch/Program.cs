using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using CommunityToolkit.VectorData.InMemory;
using VectorSearch;

// get credentials from user secrets
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

// Create an embedding generator
IEmbeddingGenerator<string, Embedding<float>> generator =
    new OpenAIClient(credential, options)
        .GetEmbeddingClient(model)
        .AsIEmbeddingGenerator();

// Create and populate the vector store
var vectorStore = new InMemoryVectorStore();

var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    // generate the embedding vector for the movie description
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);

    // add the overall movie the in-memery vector store's movie collection
    await moviesStore.UpsertAsync(movie);
}

//1-Embed the user’s query
//2-Vectorized search
//3-Returns the records

// generate the embedding vector for the user's prompt
// var query = "I want to see family friendly movie";
var query = "A science fiction movie about space travel";
var queryEmbedding = await generator.GenerateVectorAsync(query);

// search the knowledge store based on the user's prompt
var searchResults = moviesStore.SearchAsync(queryEmbedding, top: 2);

// see the results just so we know what they look like
await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}
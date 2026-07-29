using Microsoft.Extensions.VectorData;

namespace VectorSearch_Ollama;

public class Movie
{
    //Represents one movie record.
    //Key is the unique ID for a movie in the vector store. Each stored record needs one.
    [VectorStoreKey] public int Key { get; set; }

    //These are regular searchable/retrievable fields.
    //They store the movie’s title and description as normal data—not as the mathematical vector used for similarity search.
    [VectorStoreData] public string Title { get; set; }

    [VectorStoreData] public string Description { get; set; }

    /*
    This is the embedding vector for the movie, typically produced from its Description.
    ReadOnlyMemory<float> holds an array-like sequence of floating-point numbers efficiently.
    dimensions: 384 means every embedding must contain exactly 384 numbers. It must match the embedding model's output size.
    CosineSimilarity compares the direction of vectors, so descriptions with similar meaning score highly even when they use different words.
    For example, "space explorers travel through a wormhole" should be semantically close to Interstellar's description.
     */
    [VectorStoreVector(
        dimensions: 384,
        DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}

public static class MovieData
{
    public static List<Movie> Movies =>
    [
        new Movie
        {
            Key = 0,
            Title = "Lion King",
            Description =
                "The Lion King is a classic Disney animated film that tells the story of a young lion named Simba who embarks on a journey to reclaim his throne as the king of the Pride Lands after the tragic death of his father."
        },
        new Movie
        {
            Key = 1,
            Title = "Inception",
            Description =
                "Inception is a science fiction film directed by Christopher Nolan that follows a group of thieves who enter the dreams of their targets to steal information."
        },
        new Movie
        {
            Key = 2,
            Title = "The Matrix",
            Description =
                "The Matrix is a science fiction film directed by the Wachowskis that follows a computer hacker named Neo who discovers that the world he lives in is a simulated reality created by machines."
        },
        new Movie
        {
            Key = 3,
            Title = "Shrek",
            Description =
                "Shrek is an animated film that tells the story of an ogre named Shrek who embarks on a quest to rescue Princess Fiona from a dragon and bring her back to the kingdom of Duloc."
        },
        new Movie
        {
            Key = 4,
            Title = "Interstellar",
            Description =
                "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival."
        }
    ];
}
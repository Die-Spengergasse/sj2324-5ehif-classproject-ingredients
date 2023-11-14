namespace Ingredients.Options;

public class Neo4JOptions
{
    public Uri Neo4JConnection { get; set; }

    public string Neo4JUser { get; set; }

    public string Neo4JPassword { get; set; }
    
    public string Neo4JDatabase { get; set; }
}
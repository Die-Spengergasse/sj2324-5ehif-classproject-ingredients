namespace Ingredients.Model;

public class Allergens
{
    public Allergens(string id, char typeShort, string? typeLong)
    {
        Id = id;
        TypeShort = typeShort;
        TypeLong = typeLong;
    }

    public string Id { get; set; }
    public char TypeShort { get; set; }
    public string? TypeLong { get; set; }
}
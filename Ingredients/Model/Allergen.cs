namespace Ingredients.Model;

public class Allergen
{
    public Allergen(string id, char typeShort, string? typeLong)
    {
        Id = id;
        TypeShort = typeShort;
        TypeLong = typeLong;
    }

    public string Id { get; set; }
    public char TypeShort { get; set; }
    public string? TypeLong { get; set; }
}
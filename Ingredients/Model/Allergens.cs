namespace Ingredients.Model;

public class Allergens
{
    public Allergens(string id, char typeShort, string? typeLong)
    {
        this.Id = id;
        this.TypeShort = typeShort;
        this.TypeLong = typeLong;
    }

    public string Id { get; set; }
    public char TypeShort { get; set; }
    public string? TypeLong { get; set; }
}
namespace Ingredients.Model;

/// <summary>
/// Represents a single ingredient.
/// </summary>
public class Ingredient
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double CarbohydratesInGram { get; set; }
    public double FatsInGram { get; set; }
    public double ProteinsInGram { get; set; }
    
    public Ingredient(string id, string name, double carbohydratesInGram, double fatsInGram, double proteinsInGram)
    {
        Id = id;
        Name = name;
        CarbohydratesInGram = carbohydratesInGram;
        FatsInGram = fatsInGram;
        ProteinsInGram = proteinsInGram;
    }
    
#pragma warning disable CS8618
    public Ingredient() { }
#pragma warning restore CS8618
}
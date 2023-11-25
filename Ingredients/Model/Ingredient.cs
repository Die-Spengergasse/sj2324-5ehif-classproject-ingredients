namespace Ingredients.Model;

public class Ingredient
{
    public Ingredient(string id, string name, double carbohydratesInGram, double fatsInGram, double proteinsInGram)
    {
        this.Id = id;
        this.Name = name;
        this.CarbohydratesInGram = carbohydratesInGram;
        this.FatsInGram = fatsInGram;
        this.ProteinsInGram = proteinsInGram;
    }
    
    public string Id { get; set; }
    public string Name { get; set; }
    public double CarbohydratesInGram { get; set; }
    public double FatsInGram { get; set; }
    public double ProteinsInGram { get; set; }
}
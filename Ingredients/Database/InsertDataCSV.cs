using Ingredients.Model;
using Neo4j.Driver;

namespace Ingredients.Database;

public class InsertDataCSV
{
    private readonly IDriver _driveForInsert;

    public InsertDataCSV(IDriver driveForInsert)
    {
        _driveForInsert = driveForInsert;
    }

    public async Task InsertDataFromCsv(IIngredientsRepository ingredientsRepository, string filePath, string filepathallergen)
    {
        await using var session = _driveForInsert.AsyncSession();
        await session.ExecuteWriteAsync(
            async rx =>
            {
                await rx.RunAsync(
                    "MATCH (n:Ingredient) " +
                    "DETACH DELETE n"
                );
            });

        var ingredients = ReadCsvFile(filePath);
        foreach (var ingredient in ingredients)
        {
            try
            {
                await ingredientsRepository.CreateIngredient(ingredient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Einfügen des Ingredients {ingredient.Name}: {ex.Message}");
            }
        }
        //can only work with branch with allergen controller
        var allergens = ReadCsvFileAllergens(filepathallergen);
        foreach(var allergene in allergens)
        {
            try
            {
                await ingredientsRepository.CreateAllergen(allergene);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Einfügen des Allergens {allergene.TypeLong}: {ex.Message}");
            }
        }
    }

    public List<Ingredient> ReadCsvFile(string filePath)
    {
        var ingredients = new List<Ingredient>();
        using (var reader = new StreamReader(filePath))
        {
            var lines = File.ReadAllLines(filePath).Skip(1);
            // TODO : Optimization possible 
            foreach (var line in lines)
            {
                var values = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                var carboi = values[6];
                double carbo = double.Parse(carboi.Trim('"'));
                var fatsi = values[12];
                double fats = double.Parse(fatsi.Trim('"'));
                var proteinsi = values[28];
                double proteins = double.Parse(proteinsi.Trim('"'));
                var ingredient = new Ingredient
                {
                    Id = values[2],
                    Name = values[1],
                    CarbohydratesInGram = carbo,
                    FatsInGram = fats,
                    ProteinsInGram = proteins,
                };

                ingredients.Add(ingredient);
            }
        }

        return ingredients;
    }
    public List<Allergens> ReadCsvFileAllergens(string filePath)
    {
        var allerenes = new List<Allergens>();
        using (var reader = new StreamReader(filePath))
        {
            var lines = File.ReadAllLines(filePath).Skip(1);
            // TODO : Optimization possible 
            foreach (var line in lines)
            {
                var values = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                var carboi = values[2];
                char carbo = char.Parse(carboi.Trim('"'));
                var allergens = new Allergens(values[0], carbo, values[2]);
              
                allerenes.Add(allergens);
            }
        }

        return allerenes;
    }
}
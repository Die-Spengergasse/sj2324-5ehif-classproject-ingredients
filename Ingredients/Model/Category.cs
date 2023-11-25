namespace Ingredients.Model;

public class Category
{
    public Category(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}
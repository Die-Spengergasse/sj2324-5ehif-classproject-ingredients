using Microsoft.Extensions.Logging;

namespace Ingredients.Database;

public interface IIngredientsRepository
{
    public Task AddSingleTrace(Ingredients ingredients);
}

public class IngredientsRepository : IIngredientsRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;

        private ILogger<IngredientsRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientsRepository"/> class.
        /// </summary>
        public IngredientsRepository(INeo4JDataAccess neo4JDataAccess, ILogger<IngredientsRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        public Task AddSingleTrace(Ingredients ingredients)
        {
            throw new NotImplementedException();
        }
    }
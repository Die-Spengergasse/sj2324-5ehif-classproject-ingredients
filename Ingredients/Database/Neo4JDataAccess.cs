using Ingredients.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Ingredients.Database;

public interface INeo4JDataAccess : IAsyncDisposable
{
    Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);
       
    Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

    Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

    Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);
}

public class Neo4JDataAccess : INeo4JDataAccess
{
    private readonly string _database;

    private readonly ILogger<Neo4JDataAccess> _logger;
    private readonly IAsyncSession _session;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Neo4JDataAccess" /> class.
    /// </summary>
    public Neo4JDataAccess(IDriver driver, ILogger<Neo4JDataAccess> logger, IOptions<Neo4JOptions> appSettingsOptions)
    {
        _logger = logger;
        _database = appSettingsOptions.Value.Neo4JDatabase ?? "neo4j";
        _session = driver.AsyncSession(o => o.WithDatabase(_database));
    }

    /// <summary>
    ///     Execute read list as an asynchronous operation.
    /// </summary>
    public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey,
        IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
    }

    /// <summary>
    ///     Execute read dictionary as an asynchronous operation.
    /// </summary>
    public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey,
        IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync<Dictionary<string, object>>(query, returnObjectKey, parameters);
    }

    /// <summary>
    ///     Execute read scalar as an asynchronous operation.
    /// </summary>
    public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.ReadTransactionAsync(async tx =>
            {
                var scalar = default(T);
                var res = await tx.RunAsync(query, parameters);
                scalar = (await res.SingleAsync())[0].As<T>();
                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            throw;
        }
    }

    /// <summary>
    ///     Execute write transaction
    /// </summary>
    public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.WriteTransactionAsync(async tx =>
            {
                var scalar = default(T);
                var res = await tx.RunAsync(query, parameters);
                scalar = (await res.SingleAsync())[0].As<T>();
                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            throw;
        }
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or
    ///     resetting unmanaged resources asynchronously.
    /// </summary>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _session.CloseAsync();
    }

    /// <summary>
    ///     Execute read transaction as an asynchronous operation.
    /// </summary>
    private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey,
        IDictionary<string, object>? parameters)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.ReadTransactionAsync(async tx =>
            {
                var data = new List<T>();
                var res = await tx.RunAsync(query, parameters);
                var records = await res.ToListAsync();
                data = records.Select(x => (T)x.Values[returnObjectKey]).ToList();
                return data;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            throw;
        }
    }
}
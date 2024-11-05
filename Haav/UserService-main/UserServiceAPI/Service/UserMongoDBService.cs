using MongoDB.Driver;
using Model;


namespace Service;
public class UserMongoDBService : IUserDBRepository
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly ILogger<UserMongoDBService> _logger;
    public UserMongoDBService(ILogger<UserMongoDBService> logger, IConfiguration configuration)
    {
        _logger = logger;

        var connectionString = configuration["MongoConnectionString"] ?? "<blank>";
        var databaseName = configuration["DatabaseName"] ?? "blank>";
        var collectionName = configuration["CollectionName"] ?? "blank>";

        _logger.LogInformation($"Connected to MongoDB using: {connectionString}");
        _logger.LogInformation($" Using database: {databaseName}");
        _logger.LogInformation($" Using Collection: {collectionName}");

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _userCollection = database.GetCollection<User>(collectionName);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
            _userCollection = null!;
        }
    }
    public async Task<User> CreateUserAsync(User user)
    {
        // TODO: Validering af user
        await _userCollection.InsertOneAsync(user);
        return user;
    }
    // TODO: Tilføj resten af interfacet her!

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userCollection.Find(u => true).ToListAsync();
    }

    public async Task<User> UpdateUserAsync(string id, User updatedUser)
    {
        await _userCollection.ReplaceOneAsync(u => u.Id == id, updatedUser);
        return updatedUser;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var result = await _userCollection.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
    }
}
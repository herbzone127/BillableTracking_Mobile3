using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Tables;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Services
{
    public interface IDatabaseService
    {
        Task InitializeDatabaseAsync();
        Task<int> InsertAsync<T>(T item) where T : class, new();
        Task<int> UpdateAsync<T>(T item) where T : class, new();
        Task<int> DeleteAsync<T>(T item) where T : class, new();
        Task<T> GetAsync<T>(string id) where T : class, new();
        Task<List<T>> GetAllAsync<T>() where T : class, new();
    }

    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;
        private readonly List<Type> _registeredTables = new List<Type>();

        public DatabaseService()
        {
            // Define the database path
            _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BillableTracking.db3");
        }

        // Initialize the database and register all tables in one go
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                if (_database == null)
                {
                    _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
                }

                // Register all tables here
                await RegisterTables();
                Console.WriteLine("SQLite database initialized and all tables registered.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SQLite database: {ex.Message}");
                throw;
            }
        }

        // Method to register all tables in one go
        private async Task RegisterTables()
        {
            var tablesToRegister = new[] { typeof(BillableTracking_Mobile3.Tables.UserRecord),typeof(Tables.SiteConfiguration) }; // Add more types as needed
            foreach (var tableType in tablesToRegister)
            {
                if (!_registeredTables.Contains(tableType))
                {
                    await _database.CreateTableAsync(tableType);
                    _registeredTables.Add(tableType);
                    Console.WriteLine($"Table created for {tableType.Name}.");
                }
            }
        }

        // Insert a new record
        public async Task<int> InsertAsync<T>(T item) where T : class, new()
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                EnsureTableRegistered<T>();
                return await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        // Update an existing record
        public async Task<int> UpdateAsync<T>(T item) where T : class, new()
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                EnsureTableRegistered<T>();
                return await _database.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        // Delete a record
        public async Task<int> DeleteAsync<T>(T item) where T : class, new()
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                EnsureTableRegistered<T>();
                return await _database.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        // Retrieve a record by ID
        public async Task<T> GetAsync<T>(string id) where T : class, new()
        {
            try
            {
                EnsureTableRegistered<T>();
                return await _database.Table<T>().FirstOrDefaultAsync(item => GetId(item) == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving {typeof(T).Name} by Id: {ex.Message}");
                throw;
            }
        }

        // Retrieve all records
        public async Task<List<T>> GetAllAsync<T>() where T : class, new()
        {
            try
            {
                EnsureTableRegistered<T>();
                return await _database.Table<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving all {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        // Helper method to ensure the table is registered
        private void EnsureTableRegistered<T>() where T : class, new()
        {
            var tableType = typeof(T);
            if (!_registeredTables.Contains(tableType))
            {
                Task.Run(async () => await RegisterTables()).GetAwaiter().GetResult();
            }
        }

        // Helper method to get the Id of an entity
        private string GetId<T>(T item) where T : class
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} must have an Id property.");
            return (string)idProperty.GetValue(item);
        }
    }
}

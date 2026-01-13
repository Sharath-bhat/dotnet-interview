using Microsoft.Data.Sqlite;
using System.Data;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TodoService : ITodoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TodoService> _logger;

        public TodoService(IConfiguration configuration, ILogger<TodoService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _logger = logger;
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                    VALUES (@title, @description, @isCompleted, @createdAt);
                    SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@title", todo.Title);
                command.Parameters.AddWithValue("@description", todo.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
                command.Parameters.AddWithValue("@createdAt", todo.CreatedAt.ToString("o"));

                var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                todo.Id = id;

                _logger.LogInformation("Created todo with ID {TodoId}", id);
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            try
            {
                var todos = new List<Todo>();
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    todos.Add(new Todo
                    {
                        Id = reader.GetInt32("Id"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                        IsCompleted = reader.GetInt32("IsCompleted") == 1,
                        CreatedAt = DateTime.Parse(reader.GetString("CreatedAt"))
                    });
                }

                return todos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todos");
                throw;
            }
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Todo
                    {
                        Id = reader.GetInt32("Id"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                        IsCompleted = reader.GetInt32("IsCompleted") == 1,
                        CreatedAt = DateTime.Parse(reader.GetString("CreatedAt"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo with ID {TodoId}", id);
                throw;
            }
        }

        public async Task<Todo?> UpdateAsync(int id, Todo todo)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Todos
                    SET Title = @title, Description = @description, IsCompleted = @isCompleted
                    WHERE Id = @id";

                command.Parameters.AddWithValue("@title", todo.Title);
                command.Parameters.AddWithValue("@description", todo.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    todo.Id = id;
                    _logger.LogInformation("Updated todo with ID {TodoId}", id);
                    return todo;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with ID {TodoId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted todo with ID {TodoId}", id);
                }

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID {TodoId}", id);
                throw;
            }
        }
    }
}
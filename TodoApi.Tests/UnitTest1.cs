using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoRepositoryTests : IDisposable
    {
        private readonly TodoService _repository;
        private readonly string _testDbPath;

        public TodoRepositoryTests()
        {
            _testDbPath = $"test_{Guid.NewGuid()}.db";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Data Source={_testDbPath}"
                })
                .Build();

            var logger = Mock.Of<ILogger<TodoRepository>>();
            _repository = new TodoRepository(configuration, logger);

            InitializeTestDatabase();
        }

        private void InitializeTestDatabase()
        {
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_testDbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }

        [Fact]
        public async Task CreateAsync_ValidTodo_ReturnsCreatedTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.CreateAsync(todo);

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal(todo.Title, result.Title);
            Assert.Equal(todo.Description, result.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(todo);

            // Act
            var result = await _repository.GetByIdAsync(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(created.Title, result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingTodo_ReturnsUpdatedTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Original Title",
                Description = "Original Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(todo);

            var updatedTodo = new Todo
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true,
                CreatedAt = created.CreatedAt
            };

            // Act
            var result = await _repository.UpdateAsync(created.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_ExistingTodo_ReturnsTrue()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(todo);

            // Act
            var result = await _repository.DeleteAsync(created.Id);

            // Assert
            Assert.True(result);

            // Verify deletion
            var deletedTodo = await _repository.GetByIdAsync(created.Id);
            Assert.Null(deletedTodo);
        }

        public void Dispose()
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }
    }
}

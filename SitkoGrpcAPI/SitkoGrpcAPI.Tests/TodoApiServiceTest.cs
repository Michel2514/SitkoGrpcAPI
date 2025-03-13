using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SitkoGrpcAPI.Data;
using SitkoGrpcAPI.Services;
using System.Linq.Expressions;
using static SitkoGrpcAPI.TodoService;

namespace SitkoGrpcAPI.Tests
{
    public class TodoApiServiceTest
    {
        public class TodoApiServiceTests
        {
            private readonly Mock<TodoDbContext> _mockDbContext;
            private readonly TodoApiService _todoApiService;

            public TodoApiServiceTests()
            {
                _mockDbContext = new Mock<TodoDbContext>();
                _todoApiService = new TodoApiService(_mockDbContext.Object);
            }

            [Fact]
            public async Task TodoItemsAll_ShouldReturnAllTodoItems()
            {
                // Arrange
                var mockData = new List<TodoItem>
                {
                    new TodoItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Task 1",
                        Completed = false,
                        CreationDate = DateTime.UtcNow,
                        ExecutionDate = DateTime.UtcNow.AddDays(1),
                        Description = "Description 1"
                    },
                    new TodoItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Task 2",
                        Completed = true,
                        CreationDate = DateTime.UtcNow,
                        ExecutionDate = DateTime.UtcNow.AddDays(2),
                        Description = "Description 2"
                    }
                }.AsQueryable();

                var mockDbSet = new Mock<DbSet<TodoItem>>();
                mockDbSet.As<IAsyncEnumerable<TodoItem>>()
                    .Setup(m => m.GetAsyncEnumerator(default))
                    .Returns(new TestAsyncEnumerator<TodoItem>(mockData.GetEnumerator()));

                mockDbSet.As<IQueryable<TodoItem>>()
                    .Setup(m => m.Provider)
                    .Returns(new TestAsyncQueryProvider<TodoItem>(mockData.Provider));

                mockDbSet.As<IQueryable<TodoItem>>()
                    .Setup(m => m.Expression)
                    .Returns(mockData.Expression);

                mockDbSet.As<IQueryable<TodoItem>>()
                    .Setup(m => m.ElementType)
                    .Returns(mockData.ElementType);

                mockDbSet.As<IQueryable<TodoItem>>()
                    .Setup(m => m.GetEnumerator())
                    .Returns(mockData.GetEnumerator());

                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                // Act
                var result = await _todoApiService.TodoItemsAll(new Empty(), Mock.Of<ServerCallContext>());

                // Assert
                Assert.Equal(2, result.TodoItems.Count);
                Assert.Equal("Task 1", result.TodoItems[0].Name);
                Assert.Equal("Task 2", result.TodoItems[1].Name);
            }

            [Fact]
            public async Task TodoItemById_ShouldReturnTodoItem()
            {
                // Arrange
                var todoItemId = Guid.NewGuid();
                var mockData = new TodoItem
                {
                    Id = todoItemId,
                    Name = "Task 1",
                    Completed = false,
                    CreationDate = DateTime.UtcNow,
                    ExecutionDate = DateTime.UtcNow.AddDays(1),
                    Description = "Description 1"
                };

                var mockDbSet = new Mock<DbSet<TodoItem>>();
                mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync(mockData);

                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                // Act
                var result = await _todoApiService.TodoItemById(new TodoItemIdRequest { Id = todoItemId.ToString() },
                    Mock.Of<ServerCallContext>());

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Task 1", result.Name);
                Assert.Equal(todoItemId.ToString(), result.Id);
            }

            [Fact]
            public async Task TodoItemById_ShouldThrowNotFound()
            {
                // Arrange
                var todoItemId = Guid.NewGuid();
                var mockDbSet = new Mock<DbSet<TodoItem>>();
                mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync((TodoItem)null);

                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                // Act & Assert
                await Assert.ThrowsAsync<RpcException>(() =>
                    _todoApiService.TodoItemById(new TodoItemIdRequest { Id = todoItemId.ToString() },
                        Mock.Of<ServerCallContext>()));
            }

            [Fact]
            public async Task TodoItemByIdDelete_ShouldReturnTrue()
            {
                // Arrange
                var todoItemId = Guid.NewGuid();
                var mockData = new TodoItem
                {
                    Id = todoItemId,
                    Name = "Task 1",
                    Completed = false,
                    CreationDate = DateTime.UtcNow,
                    ExecutionDate = DateTime.UtcNow.AddDays(1),
                    Description = "Description 1"
                };

                var mockDbSet = new Mock<DbSet<TodoItem>>();
                mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync(mockData);

                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                // Act
                var result =
                    await _todoApiService.TodoItemByIdDelete(new TodoItemIdRequest { Id = todoItemId.ToString() },
                        Mock.Of<ServerCallContext>());

                // Assert
                Assert.True(result.Result);
                _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
            }

            [Fact]
            public async Task TodoTaskCreate_ShouldReturnCreatedTodoItem()
            {
                // Arrange
                var mockDbSet = new Mock<DbSet<TodoItem>>();
                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                var request = new TodoTaskCreateRequest
                {
                    Name = "New Task",
                    Completed = false,
                    Description = "New Description"
                };

                // Act
                var result = await _todoApiService.TodoTaskCreate(request, Mock.Of<ServerCallContext>());

                // Assert
                Assert.NotNull(result);
                Assert.Equal("New Task", result.Name);
                Assert.False(result.Completed);
                Assert.Equal("New Description", result.Description);
                mockDbSet.Verify(db => db.AddAsync(It.IsAny<TodoItem>(), default), Times.Once);
                _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
            }

            [Fact]
            public async Task TodoTaskUpdate_ShouldReturnTrue()
            {
                // Arrange
                var todoItemId = Guid.NewGuid();
                var mockData = new TodoItem
                {
                    Id = todoItemId,
                    Name = "Task 1",
                    Completed = false,
                    CreationDate = DateTime.UtcNow,
                    ExecutionDate = DateTime.UtcNow.AddDays(1),
                    Description = "Description 1"
                };

                var mockDbSet = new Mock<DbSet<TodoItem>>();
                mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync(mockData);

                _mockDbContext.Setup(db => db.TodoItems).Returns(mockDbSet.Object);

                var request = new TodoItemGrpc
                {
                    Id = todoItemId.ToString(),
                    Name = "Updated Task",
                    Completed = true,
                    Description = "Updated Description"
                };

                // Act
                var result = await _todoApiService.TodoTaskUpdate(request, Mock.Of<ServerCallContext>());

                // Assert
                Assert.True(result.Result);
                _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
            }
        }

        // ¬спомогательные классы дл€ мокировани€ IAsyncEnumerable
        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }

            public T Current => _inner.Current;
        }

        public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<T>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var result = Execute(expression);
                return (TResult)typeof(Task).GetMethod("FromResult")?
                    .MakeGenericMethod(typeof(TResult).GetGenericArguments()[0])
                    .Invoke(null, new[] { result });
            }
        }

        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
            {
            }

            public TestAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }
    }
}
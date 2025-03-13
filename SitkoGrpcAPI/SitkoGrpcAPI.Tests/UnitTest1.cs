using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using SitkoClient;


namespace SitkoGrpcAPI.Tests
{
    public class UnitTest1
    {
        private readonly Mock<ITodoService> _mockDbContext;
        //private readonly TodoApiService _todoApiService;


        [Fact]
        public async Task TodoItemsAll_ShouldReturnAllTodoItems()
        {
            var e =
                _mockDbContext.Setup(repo => repo.TodoListAllAsync())
                    .Returns(GetTestImems());
            ;
        }

        private async Task<List<TodoItemGrpc>> GetTestImems()
        {
            var todos = new List<TodoItemGrpc>
            {
                new TodoItemGrpc
                {
                    Completed = true, ExecutionDate = new Timestamp(), Name = "Test1", CreationDate = new Timestamp(),
                    Description = "scdff", Id = "213412423b f"
                },
                new TodoItemGrpc
                {
                    Completed = true, ExecutionDate = new Timestamp(), Name = "Test2", CreationDate = new Timestamp(),
                    Description = "scdff", Id = "213412423b f"
                },
                new TodoItemGrpc
                {
                    Completed = true, ExecutionDate = new Timestamp(), Name = "Test3", CreationDate = new Timestamp(),
                    Description = "scdff", Id = "213412423b f"
                },
                new TodoItemGrpc
                {
                    Completed = true, ExecutionDate = new Timestamp(), Name = "Test4", CreationDate = new Timestamp(),
                    Description = "scdff", Id = "213412423b f"
                }
            };
            return await Task.Run(() => todos);
        }
    }
}
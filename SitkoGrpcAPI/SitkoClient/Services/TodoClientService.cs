using Grpc.Net.Client;

namespace SitkoClient.Services
{
    public class TodoClientService : ITodoService
    {
        private readonly string APIgRPCService = "https://localhost:7126";

        public async Task<List<TodoItemGrpc>> TodoListAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultResponse = await todo.TodoItemsAllAsync
                (new Google.Protobuf.WellKnownTypes.Empty());
            return resultResponse.TodoItems.ToList();
        }

        public async Task<TodoItemGrpc> TodoTaskCreateAsync(TodoTaskCreateRequest todoItem)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultResponse = await todo.TodoTaskCreateAsync(todoItem);
            return resultResponse;
        }

        public async Task<bool> TodoTaskUpdateAsync(TodoItemGrpc todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultResponse = await todo.TodoTaskUpdateAsync(todoItemId);
            return resultResponse.Result;
        }

        public async Task<TodoItemGrpc> TodoItemByIdAsync(string todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultResponse = await todo.TodoItemByIdAsync
                (new TodoItemIdRequest { Id = todoItemId });
            return resultResponse;
        }

        public async Task<bool> TodoItemByIdDeleteAsync(string todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultResponse = await todo.TodoItemByIdDeleteAsync(new TodoItemIdRequest { Id = todoItemId });
            return resultResponse.Result;
        }
    }
}
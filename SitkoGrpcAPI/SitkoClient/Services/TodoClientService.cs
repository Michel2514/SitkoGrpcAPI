using Grpc.Net.Client;

namespace SitkoClient.Services
{
    public class TodoClientService : ITodoService
    {
        private string APIgRPCService = "https://localhost:7126";

        public async Task<List<TodoItemGrpc>> TodoListAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoItemsAllAsync
                (new Google.Protobuf.WellKnownTypes.Empty());
            return resultReply.TodoItem.ToList();
        }

        public async Task<TodoItemGrpc> TodoTaskCreateAsync(TodoTaskCreateRequest todoItem)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoTaskCreateAsync(todoItem);
            return resultReply;
        }

        public async Task<bool> TodoTaskUpdateAsync(TodoItemGrpc todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoTaskUpdateAsync(todoItemId);
            return resultReply.Result;
        }

        public async Task<TodoItemGrpc> TodoItemByIdAsync(string todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoItemByIdAsync
                (new TodoItemIdRequest { Id = todoItemId });
            return resultReply;
        }

        public async Task<bool> TodoItemByIdDeleteAsync(string todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoItemByIdDeleteAsync(new TodoItemIdRequest { Id = todoItemId });
            return resultReply.Result;
        }
    }
}
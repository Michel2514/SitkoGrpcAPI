using Grpc.Net.Client;

namespace SitkoClient.Services
{
    public class TodoClientService : ITodoService
    {
        private string APIgRPCService = "https://localhost:7126";

        public async Task<TodoItemsReply> TodoListAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);

            return await todo.TodoItemsAllAsync
                (new Google.Protobuf.WellKnownTypes.Empty());
        }

        public async Task<bool> TodoTaskCreateAsync(TodoTaskCreateRequest todoItem)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoTaskCreateAsync(todoItem);
            return resultReply.Result;
        }

        public async Task<bool> TodoTaskUpdateAsync(TodoItemUpdateRequest todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoTaskUpdateAsync(todoItemId);
            return resultReply.Result;
        }

        public async Task<TodoItemReply> TodoItemByIdAsync(TodoItemIdRequest todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoItemByIdAsync(todoItemId);
            return resultReply;
        }

        public async Task<bool> TodoItemByIdDeleteAsync(TodoItemIdRequest todoItemId)
        {
            using var channel = GrpcChannel.ForAddress(APIgRPCService);
            var todo = new SitkoClient.TodoService.TodoServiceClient(channel);
            var resultReply = await todo.TodoItemByIdDeleteAsync(todoItemId);
            return resultReply.Result;
        }
    }
}
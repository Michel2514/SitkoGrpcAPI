namespace SitkoClient
{
    public interface ITodoService
    {
        Task<List<TodoItemGrpc>> TodoListAllAsync();
        Task<TodoItemGrpc> TodoTaskCreateAsync(TodoTaskCreateRequest todoItem);
        Task<bool> TodoTaskUpdateAsync(TodoItemGrpc todoItemId);
        Task<TodoItemGrpc> TodoItemByIdAsync(string todoItemId);
        Task<bool> TodoItemByIdDeleteAsync(string todoItemId);
    }
}
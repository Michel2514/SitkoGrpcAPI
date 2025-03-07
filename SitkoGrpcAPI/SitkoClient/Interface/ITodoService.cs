namespace SitkoClient
{
    public interface ITodoService
    {
        Task<TodoItemsReply> TodoListAllAsync();
        Task<bool> TodoTaskCreateAsync(TodoTaskCreateRequest todoItem);
        Task<bool> TodoTaskUpdateAsync(TodoItemUpdateRequest todoItemId);
        Task<TodoItemReply> TodoItemByIdAsync(TodoItemIdRequest todoItemId);
        Task<bool> TodoItemByIdDeleteAsync(TodoItemIdRequest todoItemId);
    }
}
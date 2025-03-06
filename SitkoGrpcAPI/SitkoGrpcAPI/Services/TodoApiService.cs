using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SitkoGrpcAPI.Data;

namespace SitkoGrpcAPI.Services
{
    public class TodoApiService : TodoService.TodoServiceBase
    {
        private TodoDbContext _db;

        public TodoApiService(TodoDbContext db)
        {
            _db = db;
        }

        public override async Task<TodoItemsReply> TodoItemsAll(Empty request, ServerCallContext context)
        {
            var todoItemsReply = await _db.TodoItems.Select(x => new TodoItemReply
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Completed = x.Completed,
                CreationDate = x.CreationDate.ToTimestamp(),
                ExecutionDate = x.ExecutionDate!.Value.ToTimestamp(),
                Description = x.Description
            }).ToListAsync();

            var todoItemsReplyList = new TodoItemsReply();
            todoItemsReplyList.TodoItem.AddRange(todoItemsReply);
            return await Task.FromResult(todoItemsReplyList);
        }

        public override async Task<TodoItemReply> TodoItemById(TodoItemIdRequest request, ServerCallContext context)
        {
            var todoItemByIdReply = await TodoItemGetById(request.Id);
            if (todoItemByIdReply != null)
            {
                return await Task.FromResult(new TodoItemReply
                {
                    Id = request.Id,
                    Name = todoItemByIdReply.Name,
                    Completed = todoItemByIdReply.Completed,
                    ExecutionDate = todoItemByIdReply.ExecutionDate!.Value.ToTimestamp(),
                    CreationDate = todoItemByIdReply.CreationDate.ToTimestamp(),
                    Description = todoItemByIdReply.Description
                });
            }

            return new TodoItemReply();
        }

        public override async Task<ResultReply> TodoItemByIdDelete(TodoItemIdRequest request, ServerCallContext context)
        {
            try
            {
                var todoItemById = await TodoItemGetById(request.Id);
                if (todoItemById != null)
                {
                    _db.TodoItems.Remove(todoItemById);
                    await _db.SaveChangesAsync();
                    return new ResultReply { Result = true };
                }

                Console.WriteLine("todoItem не найден");
                return new ResultReply { Result = false };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return await Task.FromResult(new ResultReply { Result = false });
            }
        }

        public override async Task<ResultReply> TodoTaskCreate
            (TodoTaskCreateRequest request, ServerCallContext context)
        {
            try
            {
                var todoParse = new TodoItem
                {
                    Name = request.Name,
                    Completed = request.Completed,
                    CreationDate = DateTime.UtcNow,
                    ExecutionDate = request.ExecutionDate.ToDateTime(),
                    Description = request.Description,
                };
                await _db.TodoItems.AddAsync(todoParse);
                await _db.SaveChangesAsync();
                return await Task.FromResult(new ResultReply { Result = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return await Task.FromResult(new ResultReply { Result = false });
            }
        }

        public override async Task<ResultReply> TodoTaskUpdate(TodoItemUpdateRequest request, ServerCallContext context)
        {
            try
            {
                var todoById = await TodoItemGetById(request.Id);
                if (todoById != null)
                {
                    todoById.Name = request.Name;
                    todoById.ExecutionDate = request.ExecutionDate.ToDateTime();
                    todoById.Completed = request.Completed;
                    todoById.Description = request.Description;

                    return await Task.FromResult(new ResultReply { Result = true });
                }

                Console.WriteLine("todoItem не найден");
                return await Task.FromResult(new ResultReply { Result = false });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return await Task.FromResult(new ResultReply { Result = false });
            }
        }

        private async Task<TodoItem?> TodoItemGetById(string guidId)
        {
            if (!Guid.TryParse(guidId, out Guid guidParse)) return null;
            var todoItemByIdReply = await _db.TodoItems
                .FirstOrDefaultAsync(x => x.Id == guidParse);
            if (todoItemByIdReply != null)
            {
                return await Task.FromResult(todoItemByIdReply);
            }

            return await Task.FromResult(new TodoItem());
        }
    }
}
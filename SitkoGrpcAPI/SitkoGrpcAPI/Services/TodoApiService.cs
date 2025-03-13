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

        /// <summary>
        /// Возвращает все коллекцию TodoItemGrpc.
        /// </summary>
        /// <param name="request">Передается Google.Protobuf.WellKnownTypes.Empty().</param>
        /// <returns>Возвращает объект TodoItemsResponse с коллекцией TodoItems.</returns>
        public override async Task<TodoItemsResponse> TodoItemsAll(Empty request, ServerCallContext context)
        {
            var todoItemsResponse = await _db.TodoItems.ToListAsync();
            var todoItemsResponseList = new TodoItemsResponse();

            todoItemsResponseList.TodoItems.AddRange(todoItemsResponse.Select(x => new TodoItemGrpc
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Completed = x.Completed,
                CreationDate = x.CreationDate.ToTimestamp(),
                ExecutionDate = x.ExecutionDate!.Value.ToTimestamp(),
                Description = x.Description
            }));
            return todoItemsResponseList;
        }

        /// <summary>
        /// Возвращает TodoItemGrpc по id типа string
        /// </summary>
        /// <param name="request">Передается строковое представление Guid.</param>
        /// <param name="context"></param>
        /// <returns>Возвращает элемент TodoItemGrpc по Id, если такой есть.</returns>
        /// <exception cref="RpcException">Если элемента нет, выбрасывает исключение StatusCode.NotFound.</exception>
        public override async Task<TodoItemGrpc> TodoItemById(TodoItemIdRequest request, ServerCallContext context)
        {
            var todoItemByIdReply = await TodoItemGetById(request.Id);
            if (todoItemByIdReply != null)
            {
                return new TodoItemGrpc
                {
                    Id = request.Id,
                    Name = todoItemByIdReply.Name,
                    Completed = todoItemByIdReply.Completed,
                    CreationDate = todoItemByIdReply.CreationDate.ToTimestamp(),
                    ExecutionDate = todoItemByIdReply.ExecutionDate!.Value.ToTimestamp(),
                    Description = todoItemByIdReply.Description
                };
            }

            throw new RpcException(new Status(StatusCode.NotFound, "TodoItem by id not found "));
        }

        /// <summary>
        /// Удаляет TodoItemGrpc по id типа string.
        /// </summary>
        /// <param name="request">Передается строковое представление Guid.</param>
        /// <param name="context"></param>
        /// <returns>Возвращает результат удаления true или false в объекте ResultResponse.</returns>
        /// <exception cref="RpcException">Если при удалении возникли не предвиденные ситуации, выбрасывает исключение StatusCode.Internal.</exception>
        public override async Task<ResultResponse> TodoItemByIdDelete(TodoItemIdRequest request,
            ServerCallContext context)
        {
            try
            {
                var todoItemById = await TodoItemGetById(request.Id);
                if (todoItemById == null)
                {
                    return new ResultResponse { Result = false };
                }

                _db.TodoItems.Remove(todoItemById);
                await _db.SaveChangesAsync();
                return new ResultResponse { Result = true };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RpcException(new Status(StatusCode.Internal,
                    "An unexpected event occurred during deletion"));
            }
        }

        /// <summary>
        /// Создает новый объект TodoItem.
        /// </summary>
        /// <param name="request">Принимает новый объект TodoItemGrpc без поля Id.</param>
        /// <param name="context"></param>
        /// <returns>Возвращает сохраненный элемент TodoItemGrpc с присвоенным значением поля Id.</returns>
        /// <exception cref="RpcException">Если при сохранении произошла ошибка, выбрасывает исключение StatusCode.Internal.</exception>
        public override async Task<TodoItemGrpc> TodoTaskCreate
            (TodoTaskCreateRequest request, ServerCallContext context)
        {
            try
            {
                var todoParse = new TodoItem
                {
                    Name = request.Name,
                    Completed = request.Completed,
                    CreationDate = DateTime.UtcNow,
                    ExecutionDate = new Timestamp().ToDateTime(),
                    Description = request.Description,
                };
                await _db.TodoItems.AddAsync(todoParse);
                await _db.SaveChangesAsync();

                return new TodoItemGrpc
                {
                    Id = todoParse.Id.ToString(),
                    Name = todoParse.Name,
                    Completed = todoParse.Completed,
                    CreationDate = todoParse.CreationDate.ToTimestamp(),
                    ExecutionDate = todoParse.ExecutionDate.Value.ToTimestamp(),
                    Description = todoParse.Description
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RpcException(new Status(StatusCode.Internal,
                    "An unexpected event occurred during added"));
            }
        }

        /// <summary>
        /// Сохраняет измененный объект TodoItemGrpc. 
        /// </summary>
        /// <param name="request">Принимает измененный объект TodoItemGrpc.</param>
        /// <param name="context"></param>
        /// <returns>Возвращает результат сохранения true или false в объекте ResultResponse.</returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<ResultResponse> TodoTaskUpdate(TodoItemGrpc request, ServerCallContext context)
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
                    await _db.SaveChangesAsync();
                    return new ResultResponse { Result = true };
                }

                Console.WriteLine("todoItem не найден");
                return new ResultResponse { Result = false };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RpcException(new Status(StatusCode.Internal,
                    "An unexpected event occurred during update"));
            }
        }

        private async Task<TodoItem?> TodoItemGetById(string guidId)
        {
            if (!Guid.TryParse(guidId, out var guidParse)) return null;
            var todoItemByIdReply = await _db.TodoItems
                .FirstOrDefaultAsync(x => x.Id == guidParse);
            return todoItemByIdReply;
        }
    }
}
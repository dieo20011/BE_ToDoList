using MongoDB.Driver;
using ToDoList_FS.Model;

namespace ToDoList_FS
{
    public class MongoDBService
    {
        private readonly IMongoCollection<TodoItem> _todoItems;

        public MongoDBService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Todolist_Paging");
            _todoItems = database.GetCollection<TodoItem>("Paging");
        }

        public async Task AddTodoItem(TodoItem item)
        {
            await _todoItems.InsertOneAsync(item);
        }

        public async Task<List<TodoItem>> GetTodoList()
        {
            return await _todoItems.Find(todo => true).ToListAsync();
        }
    }
}

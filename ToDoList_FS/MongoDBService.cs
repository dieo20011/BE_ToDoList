using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList_FS.Controllers;
using ToDoList_FS.Model;

namespace ToDoList_FS
{
    public class MongoDBService 
    {
        private readonly IMongoCollection<TodoItem> _todoItems;
        private readonly IMongoCollection<User> _users;
        private readonly string _jwtSecret = "banhxeo0210_abc1234567890abcdef";  // 128 bits (16 bytes)
        private readonly IMongoCollection<Holiday> _holidayCollection;

        public MongoDBService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase("Todolist_Paging");
            _todoItems = database.GetCollection<TodoItem>("Paging");
            _users = database.GetCollection<User>("Users");
            _holidayCollection = database.GetCollection<Holiday>("Holiday");
        }
        public async Task<User?> GetUserById(string UserId)
        {
            return await _users.Find(u => u.Id == UserId).FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateUser(string id, string newFullname, string newUsername)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            var existingUserName = await _users.Find(u => u.UserName == newUsername && u.Id != id).FirstOrDefaultAsync();
            if(existingUserName != null)
            {
                return false;
            }
            var update = Builders<User>.Update
                .Set(u => u.FullName, newFullname)
                .Set(u => u.UserName, newUsername);
            var result = await _users.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }
        public async Task<bool> UpdatePassword(string id, string oldPassword, string newPassword)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return false;
            if(!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                return false;
            }

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var update = Builders<User>.Update.Set(u => u.Password, hashPassword);
            var result = await _users.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }
        public async Task<string> RegisterUser(string username, string password, string fullname)
        {
            var existingUser = await _users.Find(u => u.UserName == username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return "Username đã tồn tại";
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { UserName = username, Password = hashedPassword, FullName = fullname };
            await _users.InsertOneAsync(user);

            return GenerateJwtToken(user);
        }
        public async Task<string> Login(string username, string password)
        {
            var existingUser = await _users.Find(u => u.UserName == username).FirstOrDefaultAsync();
            if(existingUser == null || !BCrypt.Net.BCrypt.Verify(password, existingUser.Password)) {
                return "Invalid username or password";
            }
            return GenerateJwtToken(existingUser);
        }

        private string GenerateJwtToken(User user)
        {
            var key = _jwtSecret.Length < 32
                ? Encoding.UTF8.GetBytes(PadOrGenerateKey(_jwtSecret))
                : Encoding.UTF8.GetBytes(_jwtSecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Add user ID for more context
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string PadOrGenerateKey(string originalKey)
        {
            if (string.IsNullOrEmpty(originalKey))
            {
                return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            }

            if (originalKey.Length < 32)
            {
                return originalKey.PadRight(32, 'X');
            }

            return originalKey;
        }
        //Task
        public async Task<List<TodoItem>> GetTodoList(string UserId, int status = 0)
        {
            // If status is 0 (All), return all tasks for the user
            if (status == (int)Model.TaskStatus.All)
            {
                return await _todoItems.Find(todo => todo.UserId == UserId).ToListAsync();
            }
            
            // Otherwise, filter by both userId and status
            // Status: 1=Pending, 2=InProgress, 3=Done
            var filter = Builders<TodoItem>.Filter.And(
                Builders<TodoItem>.Filter.Eq(todo => todo.UserId, UserId),
                Builders<TodoItem>.Filter.Eq(todo => todo.Status, status)
            );
            var result = await _todoItems.Find(filter).ToListAsync();
            return result;
        }

        public async Task AddTask(TodoItem item)
        {
            Console.WriteLine($"Adding task with status: {item.Status}");
            await _todoItems.InsertOneAsync(item);
        }
        public async Task UpdateTask(string id, TodoItem item)
        {
            var filter = Builders<TodoItem>.Filter.Eq(todo => todo.Id, id);
            var request = Builders<TodoItem>.Update
                .Set(todo => todo.Title, item.Title)
                .Set(todo => todo.Status, item.Status)
                .Set(todo => todo.Description, item.Description)
                .Set(todo => todo.FromDate, item.FromDate)
                .Set(todo => todo.ToDate, item.ToDate);
            await _todoItems.UpdateOneAsync(filter, request);
        }
        public async Task DeleteTask(string id)
        {
           var filter = Builders<TodoItem>.Filter.Eq(todo => todo.Id, id);
           await _todoItems.DeleteOneAsync(filter);
        }

        public async Task<HolidayPaginatedResult> GetHolidaysAsync(string userId, HolidayQueryParams queryParams)
        {
            // Get total record
            var totalRecord = await _holidayCollection.CountDocumentsAsync(x => x.UserId == userId);

            
            var query = _holidayCollection.Find(x => x.UserId == userId);
            
            // Add search condition if keyword is not empty
            if (!string.IsNullOrEmpty(queryParams.Keyword))
            {
                var keyword = queryParams.Keyword.ToLower();
                var filter = Builders<Holiday>.Filter.And(
                    Builders<Holiday>.Filter.Eq(x => x.UserId, userId),
                    Builders<Holiday>.Filter.Regex(
                        x => x.Name,
                        new MongoDB.Bson.BsonRegularExpression(keyword, "i")
                    )
                );
                query = _holidayCollection.Find(filter);
            }
            
            // Lấy items theo trang
            var items = await query
                .Sort(Builders<Holiday>.Sort.Descending(x => x.CreatedDate))
                .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                .Limit(queryParams.PageSize)
                .ToListAsync();

            return new HolidayPaginatedResult
            {
                Items = items,
                TotalRecord = (int)totalRecord  // Trả về tổng số record không phụ thuộc search
            };
        }

        public async Task CreateHolidayAsync(HolidayDTO holidayDto, string userId)
        {
            // Nếu fromDate và toDate khác nhau, tách thành các ngày riêng biệt
            if (holidayDto.FromDate.Date != holidayDto.ToDate.Date)
            {
                var currentDate = holidayDto.FromDate.Date;
                var endDate = holidayDto.ToDate.Date;

                while (currentDate <= endDate)
                {
                    var holiday = new Holiday
                    {
                        Name = holidayDto.Name,
                        FromDate = currentDate,
                        ToDate = currentDate, // Mỗi item sẽ có fromDate = toDate
                        Description = holidayDto.Description,
                        IsAnnualHoliday = holidayDto.IsAnnualHoliday,
                        UserId = userId,
                        CreatedDate = DateTime.UtcNow
                    };
                    
                    await _holidayCollection.InsertOneAsync(holiday);
                    currentDate = currentDate.AddDays(1);
                }
            }
            else // Nếu fromDate = toDate, tạo một item duy nhất
            {
                var holiday = new Holiday
                {
                    Name = holidayDto.Name,
                    FromDate = holidayDto.FromDate.Date,
                    ToDate = holidayDto.FromDate.Date,
                    Description = holidayDto.Description,
                    IsAnnualHoliday = holidayDto.IsAnnualHoliday,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                
                await _holidayCollection.InsertOneAsync(holiday);
            }
        }

        public async Task<bool> UpdateHolidayAsync(string id, string userId, HolidayDTO holidayDto)
        {
            var update = Builders<Holiday>.Update
                .Set(h => h.Name, holidayDto.Name)
                .Set(h => h.FromDate, holidayDto.FromDate)
                .Set(h => h.ToDate, holidayDto.ToDate)
                .Set(h => h.Description, holidayDto.Description)
                .Set(h => h.IsAnnualHoliday, holidayDto.IsAnnualHoliday)
                .Set(h => h.UpdatedDate, DateTime.UtcNow);

            var result = await _holidayCollection.UpdateOneAsync(
                x => x.Id == id && x.UserId == userId,
                update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteHolidayAsync(string id, string userId)
        {
            var result = await _holidayCollection.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
            return result.DeletedCount > 0;
        }

        public async Task<Holiday?> GetHolidayByIdAsync(string id, string userId)
        {
            return await _holidayCollection.Find(x => x.Id == id && x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Holiday>> GetAllHolidaysAsync(string userId)
        {
            return await _holidayCollection
                .Find(x => x.UserId == userId)
                .Sort(Builders<Holiday>.Sort.Descending(x => x.CreatedDate))
                .ToListAsync();
        }

        public async Task<TodoItem> GetTaskById(string id)
        {
            return await _todoItems.Find(todo => todo.Id == id).FirstOrDefaultAsync();
        }
    }
}

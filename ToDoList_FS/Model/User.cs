using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ToDoList_FS.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("fullname")]
        public string? FullName { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("isAdmin")]
        public bool IsAdmin { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
    }

    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class UserSignUpRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }

    public class UserUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;



    public class PasswordUpdateRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
}

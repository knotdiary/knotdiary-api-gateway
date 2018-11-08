namespace KnotDiary.ApiGateway.Models
{
    public class User
    {
        public string Id { get; set; }
        
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Description { get; set; }

        public string AvatarUrl { get; set; }

        public string ProfileBackgroundUrl { get; set; }
    }
}

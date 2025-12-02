namespace FBS.Internal.Models
{
    public class CurrentUserViewModel
    {
        public Guid UserId { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsAdmin { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        
        public string? Address { get; set; }
    }
}

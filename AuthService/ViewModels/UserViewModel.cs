namespace AuthService.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
        public string Token { get; set; } = string.Empty;

        public UserViewModel()
        {
            Roles = new List<string>();
        }
    }
}

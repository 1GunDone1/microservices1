namespace UserManagementApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
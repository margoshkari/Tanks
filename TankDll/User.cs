
namespace TankDll
{
    public class User
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public int[] Color { get; set; }
        public User()
        {
            Login = "";
            Password = "";
            Email = "";
            Score = 0;
            Color = new int[3];
        }
        public User(string login, string password, string email)
        {
            Login = login;
            Password = password;
            Email = email;
        }
    }
}

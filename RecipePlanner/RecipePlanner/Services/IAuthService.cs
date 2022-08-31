namespace RecipePlanner.Services
{
    public interface IAuthService
    {
        public User User { get; set; }

        public void Set(string id);
    }
}

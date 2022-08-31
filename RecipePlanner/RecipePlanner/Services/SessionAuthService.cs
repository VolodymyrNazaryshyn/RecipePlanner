namespace RecipePlanner.Services
{
    public class SessionAuthService : IAuthService
    {
        private readonly UserdbContext _context;
        public User User { get; set; }

        public SessionAuthService(UserdbContext context)
        {
            _context = context;
        }

        public void Set(string id)
        {
            User = _context.Users.Find(System.Guid.Parse(id));
        }
    }
}

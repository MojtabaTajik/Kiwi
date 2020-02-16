using Data.Services.Blog;
using Model.Entities.Account;

namespace Data.Services.User
{
    public class KiwiUserServices : IUserServices
    {
        private readonly IBlogService _blogService;
        private Login LoginInfo => _blogService.GetAdminInfo().Result;

        public KiwiUserServices(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public bool ValidateUser(string username, string password)
        {
            return (username.ToLower() == LoginInfo.UserName.ToLower()) && VerifyHashedPassword(password);
        }

        private bool VerifyHashedPassword(string password)
        {
            return KiwiCryptography.HashPassword(password) == LoginInfo.Password;
        }
    }
}
using Data.Services.Blog;
using Model.Entities.Account;
using Model.Entities.Blog;
using System;

namespace Data.Seed
{
    public class DataSeeder
    {
        private readonly IBlogService _blogService;

        public DataSeeder(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public bool SeedDatabase()
        {
            bool seedResult = false;

            var adminInfo = _blogService.GetAdminInfo().Result;

            if (adminInfo == null)
            {
                seedResult = _blogService.SetAdminInfo(new Login
                {
                    ID = Guid.NewGuid().ToString(),
                    UserName = "Kiwi",
                    Password = KiwiCryptography.HashPassword("K!w!Bl0g"),
                }).Result;
            }

            var blogInfo = _blogService.GetBlogInfo().Result;
            if (blogInfo == null)
            {
                seedResult &= _blogService.SetBlogInfo(new BlogInfo
                {
                    ID = Guid.NewGuid().ToString(),
                    Title = "Kiwi Blog",
                    GithubUsername = "MojtabaTajik",
                    LinkedInUsername = "Mojtaba-Tajik",
                    TelegramUsername = "BinBreaker",
                    TwitterUsername = "MojtabaTajik",
                    About = "Kiwi blog written by Mojtaba Tajik"
                }).Result;
            }

            return seedResult;
        }
    }
}
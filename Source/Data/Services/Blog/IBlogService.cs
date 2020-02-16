using Model.Entities.Account;
using Model.Entities.Blog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Services.Blog
{
    public interface IBlogService
    {
        Task<BlogInfo> GetBlogInfo(string id = "");

        Task<bool> SetBlogInfo(BlogInfo blogInfo);

        Task<IEnumerable<Post>> GetPosts();

        Task<IEnumerable<Post>> GetPosts(int count, int skip = 0);

        Task<IEnumerable<Post>> GetPostsByCategory(string category);

        Task<Post> GetPostById(string id);

        Task<IEnumerable<string>> GetCategories();

        Task SavePost(Post post);

        Task DeletePost(Post post);

        string SaveFile(byte[] bytes, string extension);

        byte[] RetrieveFile(string fileName);

        Task<byte[]> BackupDatabase();

        Task<bool> RestoreDatabase(byte[] databaseContent);

        Task<bool> LogVisitor(VisitorInfo visitorInfo);

        Task<string> ExportVisitors();

        Task<Login> GetAdminInfo();

        Task<bool> SetAdminInfo(Login loginInfo);

        bool IsAdmin();
    }
}
using LiteDB;
using Microsoft.AspNetCore.Http;
using Model.Entities.Account;
using Model.Entities.Blog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Data.Services.Blog
{
    public class LiteDbBlogService : IBlogService
    {
        private LiteDatabase _liteDatabase;
        
        private ILiteCollection<Post> _postCollection;
        private ILiteCollection<BlogInfo> _blogInfoCollection;
        private ILiteCollection<VisitorInfo> _visitorCollection;
        private ILiteCollection<Login> _loginCollection;


        private readonly IHttpContextAccessor _contextAccessor;
        private List<Post> _postCache;
        private const string DatabaseFileName = "Database.db";

        public LiteDbBlogService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
      
            InitLiteDb();
        }

        private void InitLiteDb()
        {
            const string databasePassword = "F3l0ny#&R<Mg6283`2@~!23";
            string dbConnection = $"Filename={DatabaseFileName};Password={databasePassword}";

            _liteDatabase = new LiteDatabase(dbConnection);

            _postCollection = _liteDatabase.GetCollection<Post>();
            _blogInfoCollection = _liteDatabase.GetCollection<BlogInfo>();
            _visitorCollection = _liteDatabase.GetCollection<VisitorInfo>();
            _loginCollection = _liteDatabase.GetCollection<Login>();

            UpdateCache();
            SortCache();
        }

        public Task<BlogInfo> GetBlogInfo(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return Task.FromResult(_blogInfoCollection?.FindAll()?.ToList().FirstOrDefault());

            return Task.FromResult(_blogInfoCollection?.FindById(id));
        }

        public Task<bool> SetBlogInfo(BlogInfo blogInfo)
        {
            return Task.FromResult(_blogInfoCollection.Upsert(blogInfo));
        }

        public Task<IEnumerable<Post>> GetPosts()
        {
            bool isAdmin = IsAdmin();

            var posts = _postCache
                .Where(p => p.PubDate <= DateTime.Now && (p.IsPublished || isAdmin));

            return Task.FromResult(posts);
        }

        public Task<IEnumerable<Post>> GetPosts(int count, int skip = 0)
        {
            bool isAdmin = IsAdmin();

            var posts = _postCache
                .Where(p => p.PubDate <= DateTime.Now && (p.IsPublished || isAdmin))
                .Skip(skip)
                .Take(count);

            return Task.FromResult(posts);
        }

        public Task<IEnumerable<Post>> GetPostsByCategory(string category)
        {
            bool isAdmin = IsAdmin();

            var posts = from p in _postCache
                where p.PubDate <= DateTime.Now && (p.IsPublished || isAdmin)
                where p.Categories.Contains(category, StringComparer.OrdinalIgnoreCase)
                select p;

            return Task.FromResult(posts);
        }

        public Task<Post> GetPostById(string id)
        {
            var post = _postCache.FirstOrDefault(p => p.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
            bool isAdmin = IsAdmin();

            if (post != null && post.PubDate <= DateTime.Now && (post.IsPublished || isAdmin))
            {
                return Task.FromResult(post);
            }

            return Task.FromResult<Post>(null);
        }

        public Task<IEnumerable<string>> GetCategories()
        {
            bool isAdmin = IsAdmin();

            var categories = _postCache
                .Where(p => p.IsPublished || isAdmin)
                .SelectMany(post => post.Categories)
                .Select(cat => cat.ToLowerInvariant())
                .Distinct();

            return Task.FromResult(categories);
        }

        public Task SavePost(Post post)
        {
            post.LastModified = DateTime.Now;

            var taskResult = Task.FromResult(_postCollection.Upsert(post));
            UpdateCache();

            return taskResult;
        }

        public Task DeletePost(Post post)
        {
            var taskResult = Task.FromResult(_postCollection.Delete(post.ID));
            UpdateCache();

            return taskResult;
        }

        public string SaveFile(byte[] bytes, string extension)
        {
            string fileId = $"{Guid.NewGuid().ToString()}{extension}";

            using var ms = new MemoryStream(bytes);
            _liteDatabase.FileStorage.Upload(fileId, fileId, ms);

            return fileId;
        }

        public byte[] RetrieveFile(string fileId)
        {
            var fileInfo = _liteDatabase.FileStorage.FindById(fileId);
            if (fileInfo == null)
                return null;

            using var ms = new MemoryStream();
            fileInfo.OpenRead().CopyTo(ms);
            return ms.ToArray();
        }

        public Task<byte[]> BackupDatabase()
        {
            // Find database file to make backup
            if (!File.Exists(DatabaseFileName))
                return null;

            try
            {
                // Commit all changes and dispose database instance to free database file lock
                _liteDatabase.Checkpoint();
                _liteDatabase.Dispose();

                // Return database content
                return Task.FromResult(File.ReadAllBytes(DatabaseFileName));
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                InitLiteDb();
            }
        }

        public Task<bool> RestoreDatabase(byte[] databaseContent)
        {
            if (databaseContent == null || !databaseContent.Any())
                return Task.FromResult(false);

            try
            {
                _liteDatabase.Checkpoint();
                _liteDatabase.Dispose();
                File.WriteAllBytes(DatabaseFileName, databaseContent);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
            finally
            {
                InitLiteDb();
            }
        }

        public Task<bool> LogVisitor(VisitorInfo visitorInfo)
        {
            return Task.FromResult(_visitorCollection.Upsert(visitorInfo));
        }

        public Task<string> ExportVisitors()
        {
            return Task.FromResult(JsonSerializer.Serialize(_visitorCollection?.FindAll()?.ToList()));
        }

        public Task<Login> GetAdminInfo()
        {
            return Task.FromResult(_loginCollection?.FindAll()?.ToList().FirstOrDefault());
        }

        public Task<bool> SetAdminInfo(Login loginInfo)
        {
            var tempLogin = _loginCollection.FindAll().FirstOrDefault();
            if (tempLogin != null)
            {
                tempLogin.UserName = loginInfo.UserName;
                tempLogin.Password = KiwiCryptography.HashPassword(loginInfo.Password); 
                
                return Task.FromResult(_loginCollection.Upsert(tempLogin));
            }

            return Task.FromResult(_loginCollection.Upsert(loginInfo));
        }

        public bool IsAdmin()
        {
            bool? isAdmin = _contextAccessor.HttpContext?.User?.Identity.IsAuthenticated;

            return isAdmin.HasValue && isAdmin.Value;
        }

        protected void UpdateCache()
        {
            _postCache = _postCollection.FindAll().ToList();
            SortCache();
        }

        protected void SortCache()
        {
            _postCache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
        }
    }
}
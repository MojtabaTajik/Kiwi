namespace Kiwi
{
    public class BlogSettings
    {
        public string Owner { get; set; } = "The Owner";
        public int PostsPerPage { get; set; } = 4;
        public PostListView ListView { get; set; } = PostListView.TitlesAndExcerpts;
        public string[] ValidUploadExtensions { get; set; }
    }

    public enum PostListView
    {
        TitlesOnly,
        TitlesAndExcerpts,
        FullPosts
    }
}
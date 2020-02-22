namespace Model.Entities.Blog
{
    public class BlogInfo : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string About { get; set; }
        public string TwitterUsername { get; set; }
        public string GithubUsername { get; set; }
        public string LinkedInUsername { get; set; }
        public string TelegramUsername { get; set; }
        public string AvatarId { get; set; }
    }
}
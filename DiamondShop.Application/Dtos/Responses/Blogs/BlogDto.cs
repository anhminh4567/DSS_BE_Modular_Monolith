using DiamondShop.Application.Dtos.Responses.Accounts;

namespace DiamondShop.Application.Dtos.Responses.Blogs
{
    public class BlogDto
    {
        public string Id { get; set; }
        public List<string> Tags { get; set; } = new();
        public string AccountId { get; set; }
        public AccountDto Account { get; set; }
        public MediaDto? Thumbnail { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

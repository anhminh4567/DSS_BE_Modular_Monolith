using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Repositories.BlogRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.Blogs.Queries.GetAll
{
    public record GetAllBlogQuery(int CurrentPage, int PageSize): IRequest<PagingResponseDto<Blog>>;
    internal class GetAllBlogQueryHandler : IRequestHandler<GetAllBlogQuery, PagingResponseDto<Blog>>
    {
        private readonly IBlogRepository _blogRepository;
        public GetAllBlogQueryHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<PagingResponseDto<Blog>> Handle(GetAllBlogQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize);
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == 0 ? 20 : pageSize;
            var query = _blogRepository.GetQuery();
            var maxPage = (int)Math.Ceiling((decimal)query.Count()/pageSize);
            return new PagingResponseDto<Blog>(maxPage, currentPage, query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
        }
    }
}

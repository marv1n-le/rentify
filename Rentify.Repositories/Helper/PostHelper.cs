using Rentify.BusinessObjects.Entities;

namespace Rentify.Repositories.Helper;

public static class PostHelper
{
    public static IQueryable<Post> ApplySearchFilter(this IQueryable<Post> query, SearchFilterPostDto searchFilterPostDto)
    {
        if (!string.IsNullOrEmpty(searchFilterPostDto.Keyword))
        {
            var keyword = searchFilterPostDto.Keyword.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(keyword) ||
                p.Content.ToLower().Contains(keyword) || p.Tags.Any(t => keyword.Contains(t.ToLower())));
        }

        return query;
    }
}

public class SearchFilterPostDto
{
    public string? Keyword { get; set; }
    public List<string>? Tags { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
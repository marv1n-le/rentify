using Rentify.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Repositories.Helper;

public static class PostHelper
{
    public static IQueryable<Post> ApplySearchFilter(this IQueryable<Post> query, SearchFilterPostDto searchFilterPostDto)
    {
        // Search
        // TODO: Cần thêm search theo items, cần kiểm tra lại luồng

        // Search keyword
        if (!string.IsNullOrEmpty(searchFilterPostDto.Keyword))
        {
            var keyword = searchFilterPostDto.Keyword.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(keyword) ||
                p.Content.ToLower().Contains(keyword));
        }

        // Search Tags
        if (searchFilterPostDto.Tags != null && searchFilterPostDto.Tags.Any())
        {
            var tags = searchFilterPostDto.Tags.Select(t => t.ToLower()).ToList();
            query = query.Where(p =>
                p.Tags.Any(t => tags.Contains(t.ToLower())));
        }

        //TODO: Status cần kiểm tra lại luồng
        // Filter


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
using Microsoft.EntityFrameworkCore;

namespace Rentify.Repositories.Infrastructure;

public static class IQueryableExtension
{
    public static Task<PaginatedList<T>> GetPaginatedList<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        => PaginatedList<T>.CreateAsync(source.AsNoTracking(), pageIndex, pageSize);

}
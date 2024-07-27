using BlissShop.Common.DTO.Products;
using BlissShop.Common.Responses;
using BlissShop.Entities.Enums;

namespace BlissShop.Common.Extensions;

public static class ItemsExtension
{
    public static PageList<T> Pagination<T>(this List<T> items, int page, int pageSize)
    {
        var pageUsers = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var pageList = new PageList<T>()
        {
            TotalCount = items.Count,
            TotalPages = (int)Math.Ceiling((decimal)items.Count / pageSize),
            Items = pageUsers
        };

        return pageList;
    }

    public static List<ProductDTO> SortingItems(this List<ProductDTO> items, Sorting? sorting)
    {
        if (sorting == Sorting.Rating)
        {
            return items.OrderByDescending(x => x.TotalRating).ToList();
        }
        else if (sorting == Sorting.Expensive)
        {
            return items.OrderByDescending(x => x.Price).ToList();
        }
        else if (sorting == Sorting.Cheapest)
        {
            return items.OrderBy(x => x.Price).ToList();
        }

        return items;
    }
}

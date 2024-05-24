namespace BlissShop.Common.Requests;

public class SearchProductRequest
{
    public string? Search { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

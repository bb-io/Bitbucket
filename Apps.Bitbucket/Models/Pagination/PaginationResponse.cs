using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Pagination;

public class PaginationResponse<T>
{
    [JsonProperty("next")]
    public string? Next { get; set; }
    
    [JsonProperty("values")]
    public List<T> Values { get; set; } = [];
}
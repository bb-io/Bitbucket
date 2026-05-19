using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Pagination;

public class PaginationResponse<T>
{
    [JsonProperty("values")]
    public List<T> Values { get; set; } = [];
}
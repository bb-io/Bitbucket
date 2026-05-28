using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Handlers.Static;

public class MergeStrategyDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("merge_commit", "Merge commit"),
            new("squash", "Squash"),
            new("fast_forward", "Fast forward"),
            new("squash_fast_forward", "Squash fast forward"),
            new("rebase_fast_forward", "Rebase fast forward"),
            new("rebase_merge", "Rebase merge"),
        };
    }
}
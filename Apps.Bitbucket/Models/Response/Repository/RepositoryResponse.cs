using Apps.Bitbucket.Models.Entities.Repository;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.Repository;

public record RepositoryResponse
{
    public RepositoryResponse(RepositoryEntity repositoryEntity)
    {
        Uuid = repositoryEntity.Uuid;
        FullName = repositoryEntity.FullName;
        Name = repositoryEntity.Name;
        IsPrivate = repositoryEntity.IsPrivate;
        OwnerUuid = repositoryEntity.Owner.Uuid;
        OwnerDisplayName = repositoryEntity.Owner.DisplayName;
        Description = string.IsNullOrWhiteSpace(repositoryEntity.Description) ? null : repositoryEntity.Description;
        CreatedOn = repositoryEntity.CreatedOn;
        UpdatedOn = repositoryEntity.UpdatedOn;
        HasIssues = repositoryEntity.HasIssues;
        HasWiki = repositoryEntity.HasWiki;
        ForkPolicy = repositoryEntity.ForkPolicy;
        Language = string.IsNullOrWhiteSpace(repositoryEntity.Language) ? null : repositoryEntity.Language;
    }

    [Display("Repository UUID")] 
    public string Uuid { get; set; }

    [Display("Repository full name")] 
    public string FullName { get; set; }

    [Display("Repository name")]
    public string Name { get; set; }
    
    [Display("Repository language")]
    public string? Language { get; set; }

    [Display("Is private")]
    public bool IsPrivate { get; set; }

    [Display("Owner UUID")]
    public string OwnerUuid { get; set; }
    
    [Display("Owner display name")]
    public string OwnerDisplayName { get; set; }

    [Display("Repository description")]
    public string? Description { get; set; }

    [Display("Created on")]
    public DateTime CreatedOn { get; set; }

    [Display("Updated on")]
    public DateTime? UpdatedOn { get; set; }

    [Display("Has issues")]
    public bool HasIssues { get; set; }
    
    [Display("Has wiki")]
    public bool HasWiki { get; set; }

    [Display("Repository fork policy")]
    public string ForkPolicy { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ConsumerEndpoint.Consumer;

public record ApplicationOptions
{
    public const string Key = "HubCon";

    [Required(ErrorMessage = "Address Required")]
    public string HubAddress { get; init; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;
using FribergShared.Dto;

namespace FribergApi.Models;

public class Update
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string CarId { get; set; } = string.Empty;
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public DateTime TimeStamp { get; set; }
    [Required]
    public int Index { get; set; }
    [Required]
    public string Key { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;

    public FullUpdateDto ToDto()
    {
        return new FullUpdateDto
        {
            Id = Id,
            TimeStamp = TimeStamp,
            Index = Index,
            Key = Key,
            OldValue = OldValue,
            NewValue = NewValue,
        };
    }
}

using System.ComponentModel.DataAnnotations;
using FribergShared.Dto;

namespace FribergApi.Models;

public class Update
{
    [Key]
    public required string Id { get; set; } = string.Empty;
    public required string CarId { get; set; } = string.Empty;
    public required string UserId { get; set; } = string.Empty;
    public required DateTime TimeStamp { get; set; }
    public required int Index { get; set; }
    public required string Key { get; set; } = string.Empty;
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

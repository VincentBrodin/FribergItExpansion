using System.ComponentModel.DataAnnotations;
using FribergShared.Dto;

namespace FribergApi.Models;

public class Rental
{
    [Key]
    public required string Id { get; set; } = string.Empty;
    public required string CarId { get; set; } = string.Empty;
    public required string UserId { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }


    public FullRentalDto ToDto()
    {
        return new FullRentalDto
        {
            Id = Id,
            Start = Start,
            End = End,
        };
    }
}

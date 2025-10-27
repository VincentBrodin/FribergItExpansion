using System.ComponentModel.DataAnnotations;
using FribergShared.Dto;

namespace FribergApi.Models;

public class Car
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public int HorsePower { get; set; } = 0;
    public List<Update> Updates { get; set; } = [];
    public bool Deleted {get; set;} = false;

    public FullCarDto ToDto()
    {
        return new FullCarDto
        {
            Id = Id,
            Make = Make,
            Model = Model,
            Year = Year,
            HorsePower = HorsePower,
            Deleted = Deleted,
        };
    }
}

namespace FribergShared.Dto;

public class FullCarDto
{
    public string Id { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public int HorsePower { get; set; } = 0;
    public bool Deleted = false;
    public List<FullUpdateDto> Updates {get; set;} = [];
    public List<FullRentalDto> Rentals {get; set;} = [];
}

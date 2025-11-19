namespace FribergShared.Dto;

public class CarDto
{
    public string Id { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public int HorsePower { get; set; } = 0;
    public bool Deleted { get; set; } = false;
}

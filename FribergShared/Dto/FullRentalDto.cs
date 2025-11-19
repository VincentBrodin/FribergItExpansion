namespace FribergShared.Dto;

public class FullRentalDto : RentalDto
{
    public CarDto Car { get; set; } = new();
}

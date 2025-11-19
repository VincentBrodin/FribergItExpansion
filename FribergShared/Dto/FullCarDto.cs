namespace FribergShared.Dto;

public class FullCarDto : CarDto
{
    public List<FullUpdateDto> Updates {get; set;} = [];
    public List<RentalDto> Rentals {get; set;} = [];
}

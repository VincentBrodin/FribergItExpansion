
namespace FribergShared.Dto;

public class FullUserDto : UserDto
{
    public List<FullRentalDto> Rentals {get; set;} = [];
}

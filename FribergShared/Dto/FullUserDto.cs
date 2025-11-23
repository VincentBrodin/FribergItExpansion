
namespace FribergShared.Dto;

public class FullUserDto : UserDto
{
    public List<FullRentalDto> Rentals {get; set;} = [];
    public bool IsAdmin {get; set;}
}

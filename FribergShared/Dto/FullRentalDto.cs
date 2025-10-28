namespace FribergShared.Dto;

public class FullRentalDto
{
    public string Id { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

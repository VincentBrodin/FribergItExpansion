namespace FribergShared.Dto;

public class FullUpdateDto
{
    public string Id { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public DateTime TimeStamp { get; set; }
    public int Index { get; set; }
    public string Key { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}

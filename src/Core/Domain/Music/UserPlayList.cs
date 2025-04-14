namespace Domain.Music;
using User;    

public record UserPlayList(string? Id, string? Name, User? User, Image? PlayListImage, Track? Track)
{
  public string? Id { get; init; } = Id;
  public string? Name { get; set; } = Name ?? throw new ArgumentNullException(nameof(Name));
  public User? User { get; set; } = User;
  public Image? PlayListImage { get; set; } = PlayListImage;
  public Track? Track { get; set; } = Track;
}
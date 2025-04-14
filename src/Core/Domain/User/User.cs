using Domain.Music;

namespace Domain.User;

public class User(string userName, string email, Image profileImage, string country)
{
  public string? Username { get; } = userName;
  public string? Email { get; } = email;
  public Image? ProfileImage { get; } = profileImage;
  public string? Country { get; set; } = country;
}
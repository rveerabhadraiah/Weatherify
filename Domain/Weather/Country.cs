namespace Domain.Weather
{
  public class Country
  {
    public string? Name { get; }
    public DateTime? SunRise { get; }
    public DateTime? SunSet { get; }

    public Country(string name, DateTime sunRise, DateTime sunSet)
    {
      Name = name;
      SunRise = sunRise;
      SunSet = sunSet;
    }

    public override bool Equals(object? obj)
    {
      if (obj is Country other)
      {
        return Name == other.Name && SunRise == other.SunRise && SunSet == other.SunSet;
      }

      return false;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Name, SunRise, SunSet);
    }
  }
}

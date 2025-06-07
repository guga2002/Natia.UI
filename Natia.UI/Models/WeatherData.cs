namespace Natia.UI.Models;

public class WeatherData
{
    public Current? current { get; set; }
}


public class Current
{
    public string? time { get; set; }
    public int interval { get; set; }
    public double temperature_2m { get; set; }
    public double wind_speed_10m { get; set; }
}

namespace NatiaGuard.BrainStorm.Models
{
    public class WeatherData
    {
        public current current { get; set; }
    }

   
    public class current
    {
        public string time { get; set; }
        public int interval { get; set; }
        public double temperature_2m { get; set; }
        public double wind_speed_10m { get; set; }
    }


}

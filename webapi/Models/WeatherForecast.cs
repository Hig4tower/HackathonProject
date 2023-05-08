namespace webapi.Models;

public class WeatherForecast
{
    public string? Date { get; set; }

    public int TemperatureC { get; set; }

    public int? TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}

public class WeatherExtension : WeatherForecast
{
    public string? DayOrNight { get; set; }
    public int? Humidity { get; set; }
    public int? PrecipitationProbability { get; set; }
    public int? WindK { get; set; }
    public int? WindM => (int)(0.62137 * WindK);
    public string? WindDir { get; set; }
}
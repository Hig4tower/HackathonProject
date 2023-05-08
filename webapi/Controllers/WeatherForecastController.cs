using Microsoft.AspNetCore.Mvc;
using Mindee;
using Mindee.Parsing;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly MindeeClient _mindeeClient;

    public WeatherExtension WeatherExtension = new();

    public WeatherForecastController(ILogger<WeatherForecastController> logger, MindeeClient mindeeClient)
    {
        _logger = logger;
        _mindeeClient = mindeeClient;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherExtension> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherExtension
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)).ToString(),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost(Name = "AddWeatherImage")]
    public async Task<WeatherExtension> Create()
    {
        var file = Request.Form.Files[0];

        var prediction = await _mindeeClient.LoadDocument(file.OpenReadStream(), file.FileName).ParseAsync(new CustomEndpoint(
                         "weather_data",
                         "myScribe"));
        var weatherOCR = new WeatherExtension();
        foreach (var field in prediction.Inference.DocumentPrediction.Fields)
        {
            switch (field.Key)
            {
                case "summary":
                    weatherOCR.Summary = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
                    break;

                case "day_of_month":
                    weatherOCR.Date = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
                    break;

                case "day_or_night":
                    weatherOCR.DayOrNight = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
                    break;

                case "humidity":
                    weatherOCR.Humidity = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
                    break;

                case "temperature_c":
                    weatherOCR.TemperatureC = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
                    break;

                case "precipitation_probability":
                    weatherOCR.PrecipitationProbability = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
                    break;

                case "wind_k":
                    weatherOCR.WindK = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
                    break;

                case "wind_dir":
                    weatherOCR.WindDir = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
                    break;

                default:
                    break;
            }
        }
        return weatherOCR;
    }
}
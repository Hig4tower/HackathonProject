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
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
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
            switch (nameof(field.Key).ToLower())
            {
                case "summary":
                    weatherOCR.Summary = field.Value.Values.Select(x => x.Content).ToString();
                    break;

                case "":
                    break;

                default:
                    break;
            }
        }
        return weatherOCR;
    }
}
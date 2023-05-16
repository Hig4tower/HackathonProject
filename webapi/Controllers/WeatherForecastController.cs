using Microsoft.AspNetCore.Mvc;
using Mindee;
using Mindee.Parsing;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
        string apiKey = "afcb495eb02fe265f8ae8ce9aa2798d4";
        MindeeClient mindeeClient = MindeeClientInit.Create(apiKey);
        CustomEndpoint myEndpoint = new CustomEndpoint(
            endpointName: "weather_data",
            accountName: "myScribe"
        );
        var documentParsed = await mindeeClient
            .LoadDocument(file.OpenReadStream(), file.FileName)
            .ParseAsync(myEndpoint);

        var test = documentParsed.Inference.DocumentPrediction.Fields;
        var weatherextension = new WeatherExtension();
        weatherextension.Date = string.Join("", test.FirstOrDefault(x => x.Key == "day_of_month").Value.Values);
        weatherextension.Summary = string.Join("", test.FirstOrDefault(x => x.Key == "summary").Value.Values);
        weatherextension.DayOrNight = string.Join("", test.FirstOrDefault(x => x.Key == "day_or_night").Value.Values);
        weatherextension.WindDir = string.Join("", test.FirstOrDefault(x => x.Key == "wind_dir").Value.Values);

        weatherextension.Humidity = int.Parse(test.FirstOrDefault(x => x.Key == "humidity").Value.Values[0].ToString());
        weatherextension.PrecipitationProbability = int.Parse(test.FirstOrDefault(x => x.Key == "precipitation_probability").Value.Values[0].ToString());
        weatherextension.TemperatureC = int.Parse(test.FirstOrDefault(x => x.Key == "temperature_c").Value.Values[0].ToString());
        weatherextension.WindK = int.Parse(test.FirstOrDefault(x => x.Key == "wind_k").Value.Values[0].ToString());
        return weatherextension;
    }

    //[HttpPost(Name = "AddWeatherImage")]
    //public async Task<WeatherExtension> Create()
    //{
    //    var file = Request.Form.Files[0];
    //    var prediction = await _mindeeClient.LoadDocument(file.OpenReadStream(), file.FileName).ParseAsync(new CustomEndpoint(
    //                     "weather_data",
    //                     "myScribe"));
    //    var weatherOCR = new WeatherExtension();
    //    foreach (var field in prediction.Inference.DocumentPrediction.Fields)
    //    {
    //        switch (field.Key)
    //        {
    //            case "summary":
    //                weatherOCR.Summary = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
    //                break;

    //            case "day_of_month":
    //                weatherOCR.Date = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
    //                break;

    //            case "day_or_night":
    //                weatherOCR.DayOrNight = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
    //                break;

    //            case "humidity":
    //                weatherOCR.Humidity = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
    //                break;

    //            case "temperature_c":
    //                weatherOCR.TemperatureC = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
    //                break;

    //            case "precipitation_probability":
    //                weatherOCR.PrecipitationProbability = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
    //                break;

    //            case "wind_k":
    //                weatherOCR.WindK = int.Parse(field.Value.Values.ToList().FirstOrDefault().Content);
    //                break;

    //            case "wind_dir":
    //                weatherOCR.WindDir = string.Join("", field.Value.Values.Select(x => x.Content + " ")).Trim();
    //                break;

    //            default:
    //                break;
    //        }
    //    }
    //    return weatherOCR;
    //}
}
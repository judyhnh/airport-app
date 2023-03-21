using System;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace MyWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirportController : ControllerBase
{
  public object JsonConvert { get; private set; }

  [HttpPost("GetAirport")]
public IActionResult GetAirport([FromBody] AirportRequest request)
{
    [HttpPost("GetAirport")]
public IActionResult GetAirport([FromBody] AirportRequest request)
{
    var airports = LoadAirports();
    var airportTimezones = LoadAirportTimezones();

    var airport = airports.FirstOrDefault(a => a.IataCode == request.AirportIata);
    var airportTimezone = airportTimezones.FirstOrDefault(t => t.Airport == request.AirportIata);

    if (airport == null || airportTimezone == null)
    {
        return NotFound("Airport not found");
    }

    var localDateTime = LocalDateTime.FromDateTime(request.LocalTime);
    var timeZoneProvider = DateTimeZoneProviders.Tzdb;
    var sourceTimeZone = timeZoneProvider["Europe/Vienna"]; // Vienna local time
    var targetTimeZone = timeZoneProvider[airportTimezone.Timezone]; // Destination time

    var sourceZonedDateTime = localDateTime.InZoneLeniently(sourceTimeZone);
    var targetZonedDateTime = sourceZonedDateTime.WithZone(targetTimeZone);

    var result = new
    {
        Airport = airport,
        LocalTime = request.LocalTime,
        ConvertedTime = targetZonedDateTime.ToDateTimeUnspecified()
    };

    return Ok(result);
}

}

private List<Airport> LoadAirports()
{
    var json = System.IO.File.ReadAllText("../Airports/airports.json");
    return JsonConvert.DeserializeObject<List<Airport>>(json);
}

private List<AirportTimezone> LoadAirportTimezones()
{
    var json = System.IO.File.ReadAllText("../Timezones/timezones.json");
    return JsonConvert.DeserializeObject<List<AirportTimezone>>(json);
}



}

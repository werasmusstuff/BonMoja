using EventLogger.Abstractions;
using EventLogger.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventLogger.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
  private readonly IEventService _eventService;

  public EventsController(IEventService eventService)
  {
    _eventService = eventService;
  }

  [HttpPost]
  public async Task<IActionResult> PostEvent([FromBody] EventRequest request)
  {
    var eventResult = await _eventService.RecordEventAsync(request);
    return Ok(eventResult);
  }

  [HttpGet("{userId}")]
  public async Task<IActionResult> GetEvents(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
  {
    var result = await _eventService.GetEventsByUserAsync(userId, page, pageSize);
    return Ok(result);
  }
}
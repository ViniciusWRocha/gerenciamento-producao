using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Embora o service seja síncrono, é boa prática em controllers API

[ApiController]
[Route("api/[controller]")] // Define a rota base como /api/Calendar
public class CalendarController : ControllerBase
{
    // O Service é injetado automaticamente pelo framework
    private readonly GoogleCalendarService _calendarService;

    // Construtor que recebe a instância do Service configurada no Program.cs
    public CalendarController(GoogleCalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    // --- Endpoint para CRIAR EVENTO (POST /api/Calendar/create) ---

    /// <summary>
    /// Cria um novo evento no Google Calendar.
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateEvent([FromBody] CreateEventDto dto)
    {
        try
        {
            // Use o ID do calendário compartilhado
            var calendarId = "e96a4fe0acce51e1436e1b25ecfd9055123036df7caabdbfcad011b2a82111fb@group.calendar.google.com";
            var ev = _calendarService.CreateEvent(
                calendarId,
                dto.Title,
                dto.Start,
                dto.End,
                dto.Description
            );

            return Ok(new
            {
                success = true,
                eventId = ev.Id,
                htmlLink = ev.HtmlLink,
                message = "Evento criado com sucesso!"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                error = "Erro ao criar evento.",
                details = ex.Message
            });
        }
    }

    // --- Endpoint para LISTAR EVENTOS (GET /api/Calendar/list) ---

    /// <summary>
    /// Lista os próximos eventos, formatados para uso em calendários front-end (ex: FullCalendar).
    /// </summary>
    [HttpGet("list")]
    public IActionResult ListEvents()
    {
        try
        {
            var calendarId = "e96a4fe0acce51e1436e1b25ecfd9055123036df7caabdbfcad011b2a82111fb@group.calendar.google.com";
            var events = _calendarService.ListEvents(calendarId);

            var formattedEvents = events.Select(ev => new
            {
                title = ev.Summary,
                start = ev.Start.DateTimeRaw ?? ev.Start.Date,
                end = ev.End.DateTimeRaw ?? ev.End.Date
            }).ToList();

            return Ok(formattedEvents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Erro ao listar eventos.",
                details = ex.Message
            });
        }
    }

    // DTO (Data Transfer Object) para receber dados da requisição POST
    public class CreateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
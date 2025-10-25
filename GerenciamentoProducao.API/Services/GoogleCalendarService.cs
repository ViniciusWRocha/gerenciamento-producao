using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;

public class GoogleCalendarService
{
    private readonly CalendarService _service;

    public GoogleCalendarService(string credentialsPath, string applicationName)
    {
        GoogleCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(CalendarService.Scope.Calendar);
        }

        _service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName
        });
    }

    public Event CreateEvent(string calendarId, string title, DateTime start, DateTime end, string description = "")
    {
        var newEvent = new Event
        {
            Summary = title,
            Description = description,
            Start = new EventDateTime
            {
                DateTime = start,
                TimeZone = "America/Sao_Paulo"
            },
            End = new EventDateTime
            {
                DateTime = end,
                TimeZone = "America/Sao_Paulo"
            }
        };

        var request = _service.Events.Insert(newEvent, calendarId);
        return request.Execute();
    }

    public IList<Event> ListEvents(string calendarId, DateTime? timeMin = null)
    {
        var request = _service.Events.List(calendarId);
        request.SingleEvents = true;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        request.TimeMin = timeMin ?? DateTime.Now;

        return request.Execute().Items;
    }
}

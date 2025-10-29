using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace GerenciamentoProducao.Services
{
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

    public void DeleteEvent(string calendarId, string eventId)
    {
        var request = _service.Events.Delete(calendarId, eventId);
        request.Execute();
    }

    public Event UpdateEvent(string calendarId, string eventId, string title, DateTime start, DateTime end, string description = "")
    {
        var existingEvent = _service.Events.Get(calendarId, eventId).Execute();
        
        existingEvent.Summary = title;
        existingEvent.Description = description;
        existingEvent.Start = new EventDateTime
        {
            DateTime = start,
            TimeZone = "America/Sao_Paulo"
        };
        existingEvent.End = new EventDateTime
        {
            DateTime = end,
            TimeZone = "America/Sao_Paulo"
        };

        var request = _service.Events.Update(existingEvent, calendarId, eventId);
        return request.Execute();
    }

    public Event GetEvent(string calendarId, string eventId)
    {
        var request = _service.Events.Get(calendarId, eventId);
        return request.Execute();
    }
}
}

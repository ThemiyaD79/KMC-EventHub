using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KNC.Client
{
    public class EventService
    {
        private readonly HttpClient _httpClient;

        public EventService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44349/api/KMCEvents/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
// GET ALL EVENTS
        public async Task<List<KMCEvent>> GetAllEvents(string search = null, string category = null)
        {
            string url = "";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (queryParams.Count > 0) url = "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<KMCEvent>>(jsonString);
            }
            return new List<KMCEvent>();
        }

        // 2. GET SINGLE EVENT
        public async Task<KMCEvent> GetEventById(string id)
        {
            var response = await _httpClient.GetAsync($"{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<KMCEvent>(jsonString);
            }
            return null;
        }

        // 3. ADD EVENT
        public async Task AddEvent(KMCEvent newEvent)
        {
            var response = await _httpClient.PostAsJsonAsync("", newEvent);
            if (!response.IsSuccessStatusCode)
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add event: {errorDetails}");
            }
        }

        // 4. UPDATE EVENT
        public async Task UpdateEvent(KMCEvent updatedEvent)
        {
            var response = await _httpClient.PutAsJsonAsync($"{updatedEvent.EventId}", updatedEvent);
            if (!response.IsSuccessStatusCode)
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update event: {errorDetails}");
            }
        }

        //  delete event 
        public async Task DeleteEvent(string id)
        {
            var response = await _httpClient.DeleteAsync($"{id}");
            if (!response.IsSuccessStatusCode)
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete event: {errorDetails}");
            }
        }

        //  register for event 
        public async Task RegisterForEvent(EventRegistration registration)
        {
            var response = await _httpClient.PostAsJsonAsync("Register", registration);
            if (!response.IsSuccessStatusCode)
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Registration Failed: {errorDetails}");
            }
        }
        public async Task<List<RegistrationDTO>> GetAllRegistrations()
        {
            var response = await _httpClient.GetAsync("Registrations");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RegistrationDTO>>(jsonString);
            }
            return new List<RegistrationDTO>();
        }
    }
    public class KMCEvent
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
    }

    public class EventRegistration
    {
        public string EventId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class RegistrationDTO
    {
        public int RegistrationId { get; set; }
        public string EventId { get; set; }
        public string EventTitle { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
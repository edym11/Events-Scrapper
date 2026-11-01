using System;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using WebScrapper.Models;
using System.Globalization;

namespace WebScrapper.Scrapers;

public class LocationExtraction
{
    Dictionary<string, string> eventsDictionary = new Dictionary<string, string>();
    HttpClient client = new HttpClient();

    public LocationExtraction()
    {
        client.DefaultRequestHeaders.Add("User-Agent","DwardEventScrapper/1.0");
    }

    public async Task<List<EventModel>> getExactCoordinates(List<EventModel> events)
    {
        foreach(var ev in events){
            if(ev.location == null || ev.city == null)
            {
                continue;
            }
            string request = $"{ev.location}, {ev.city}";

            if (eventsDictionary.ContainsKey(request))
            {
                string coordinates = eventsDictionary[request];
                if(coordinates != "")
                {
                    string[] subs = coordinates.Split(",");
                    ev.latitude = double.Parse(subs[0], CultureInfo.InvariantCulture);
                    ev.longitude = double.Parse(subs[1], CultureInfo.InvariantCulture);
                }
            } 
            else
            {
                string saved = "";
                try
                {
                    string url = "https://nominatim.openstreetmap.org/search?q=" + Uri.EscapeDataString(request) + "&format=jsonv2&limit=1&countrycodes=ro";
                    string json = await client.GetStringAsync(url);
                    JsonDocument doc = JsonDocument.Parse(json);

                    if(doc.RootElement.GetArrayLength() == 0)
                    {
                        await Task.Delay(1010);
                        url = "https://nominatim.openstreetmap.org/search?q=" + Uri.EscapeDataString(ev.city) + "&format=jsonv2&limit=1&countrycodes=ro";
                        json = await client.GetStringAsync(url);
                        doc = JsonDocument.Parse(json);
                    }                    

                    if(doc.RootElement.GetArrayLength() > 0)
                    {
                        JsonElement result = doc.RootElement[0];
                        string lat = result.GetProperty("lat").ToString();
                        string lon = result.GetProperty("lon").ToString();

                        saved = $"{lat},{lon}";
                        ev.latitude = double.Parse(lat, CultureInfo.InvariantCulture);
                        ev.longitude = double.Parse(lon, CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    Console.WriteLine("Unable to retrieve coordinates for" + request);
                }
                eventsDictionary.Add(request, saved);
                await Task.Delay(1010);
            }
        }
        return events;
    }
}

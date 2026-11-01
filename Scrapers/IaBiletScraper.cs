using WebScrapper.Interfacess;
using WebScrapper.Models;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScrapper.Scrapers;

public class IaBiletScraper : IScrapper
{
    public async Task<List<EventModel>> ScrapeEvents(DateTime dateTime, int daysToScrape)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

        var scrapedEvents = new List<EventModel>();
        var seen = new List<string>();

        int pageNumber = 1;
        int maxPages = 15;
        bool keepLooking = true;

        while (keepLooking && pageNumber <= maxPages)
        {
            string siteUrl2 = $"https://www.iabilet.ro/bilete-in-cluj-napoca/?page={pageNumber}";
            var documentIaBilet = await context.OpenAsync(siteUrl2);

            var scriptTags = documentIaBilet.QuerySelectorAll("script[type='application/ld+json']");
            int newOnPage = 0;

            if(scriptTags.Length == 0)
            {
                keepLooking = false;
                break;
            }

            foreach(var script in scriptTags)
            {
                string cleanText = script.TextContent;
                cleanText = cleanText.Replace("/*<![CDATA[*/", "").Replace("/*]]>*/", "").Trim();

                try 
                {
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(cleanText);
                    var root = jsonDoc.RootElement;

                    var jsonType = root.GetProperty("@type").GetString();

                    if (jsonType == "Event")
                    {
                        var eventTitle = root.GetProperty("name").GetString();
                        var eventDesc = root.GetProperty("description").GetString();
                        var eventDate = root.GetProperty("startDate").GetString();
                        
                        var locationBlock = root.GetProperty("location");
                        var eventVenue = locationBlock.GetProperty("name").GetString();
                        var eventCity = locationBlock.GetProperty("address").GetProperty("addressLocality").GetString();

                        var newEvent = new EventModel
                        {
                            title = eventTitle ?? " ",
                            description = eventDesc,
                            city = eventCity,
                            time = eventDate, 
                            location = eventVenue
                        };

                        string key = newEvent.title + "|" + newEvent.time;

                        if(newEvent.title != " " && !seen.Contains(key))
                        {
                            seen.Add(key);
                            scrapedEvents.Add(newEvent);
                            newOnPage++;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Skipping an event: {ex.Message}");
                }
            }
            
            Console.WriteLine($"IaBilet page {pageNumber}: {newOnPage} new events");

            if(newOnPage == 0)
            {
                keepLooking = false;
            }

            pageNumber++;
            await Task.Delay(1000);
        }
        return scrapedEvents;
    }
}

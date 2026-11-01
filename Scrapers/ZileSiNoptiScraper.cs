using WebScrapper.Interfacess;
using WebScrapper.Models;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScrapper.Scrapers;

public class ZileSiNoptiScraper : IScrapper
{
    public async Task<List<EventModel>> ScrapeEvents(DateTime startDate, int daysToScrape)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

        var scrapedEvents = new List<EventModel>();

        for(int i = 0; i < daysToScrape; i++)
        {
            DateTime targetDate = startDate.AddDays(i);
            string dateFormated = targetDate.ToString("yyyy-MM-dd");
            Console.WriteLine($"Date: {dateFormated} has the following events:\n");

            string siteUrl = $"https://zilesinopti.ro/evenimente-romania/?zi={dateFormated}";
            var documentZileSiNopti = await context.OpenAsync(siteUrl);

            var eventCards = documentZileSiNopti.QuerySelectorAll(".kzn-sw-item");
            foreach(var card in eventCards)
            {
                var title = card.QuerySelector(".kzn-sw-item-titlu a")?.TextContent;
                var city = card.QuerySelector(".kzn-sw-item-textsus")?.TextContent.Trim();
                var time = card.QuerySelector(".kzn-one-event-date")?.TextContent;
                var location = card.QuerySelector(".kzn-sw-item-adresa a")?.TextContent;
            
                if (!string.IsNullOrEmpty(title))
                {
                    // int index = title.IndexOf("@");
                    // if(index >= 0)
                    // {
                    //     title = title.Substring(0, index);
                    // }
                
                    // Console.WriteLine($"Title: {title}");
                    // Console.WriteLine($"City: {city}");
                    // Console.WriteLine($"Time: {time}");
                    // Console.WriteLine($"Location: {location}");
                    
                    
                    var eventUrl = card.QuerySelector(".kzn-sw-item-titlu a")?.GetAttribute("href");
                    if (!string.IsNullOrEmpty(eventUrl))
                    {
                        var document1 = await context.OpenAsync(eventUrl);
                        
                        var paragraphs = document1.QuerySelectorAll("#kzn-continut-articol p");
                        var description = string.Join("\n\n", paragraphs.Select(p => p.TextContent.Trim()));
                        description = description.Replace("\u00A0", " ");

                        // Console.WriteLine($"Description: {description}");

                        var newEvent = new EventModel
                        {
                            title = title,
                            description = description,
                            city = city,
                            time = time,
                            location = location
                        };
                        scrapedEvents.Add(newEvent);
                        await Task.Delay(500);
                    }
                    // var description = card.QuerySelector("kzn-continut-articol")?.TextContent;
                    // Console.WriteLine($"Description: {description}");
                    // Console.WriteLine("\n");
                }
            }

            await Task.Delay(1000);
        }

        return scrapedEvents;
    }
}

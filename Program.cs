using System.Diagnostics.Tracing;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WebScrapper.Manager;
using WebScrapper.Models;
using WebScrapper.Scrapers;


int daysToScrape = 3;
DateTime startDate = DateTime.Today;

Scrapper scrapper = new Scrapper(startDate, daysToScrape);
scrapper.AddSite(new ZileSiNoptiScraper());
scrapper.AddSite(new IaBiletScraper());

List<EventModel> scrapedEvents = await scrapper.Loop();

LocationExtraction locationExtraction = new LocationExtraction();
scrapedEvents = await locationExtraction.getExactCoordinates(scrapedEvents);

var options = new System.Text.Json.JsonSerializerOptions{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
string finalJson = System.Text.Json.JsonSerializer.Serialize(scrapedEvents, options);
string filePath = "events.json";

await System.IO.File.WriteAllTextAsync(filePath, finalJson);

Console.WriteLine(finalJson);
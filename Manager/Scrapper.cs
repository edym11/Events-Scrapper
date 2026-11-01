using WebScrapper.Interfacess;
using WebScrapper.Models;

namespace WebScrapper.Manager;

public class Scrapper
{
    bool IsFinished = false;
    DateTime dateTime; 
    int daysToScrape;
    Queue<IScrapper> scrappers = [];
    public Scrapper(DateTime startDate, int days)
    {
        dateTime = startDate;
        daysToScrape = days;
    }
    public void AddSite(IScrapper scrapper)
    {
        scrappers.Enqueue(scrapper);
    }
    public async Task<List<EventModel>> Loop()
    {
        List<EventModel> eventModels = new List<EventModel>();
        while(scrappers.Count > 0)
        {
            IScrapper scrapper = scrappers.Dequeue();
            List<EventModel> list = await scrapper.ScrapeEvents(dateTime, daysToScrape);
            eventModels.AddRange(list);
        } 
        IsFinished = true;
        return eventModels;
    }
}

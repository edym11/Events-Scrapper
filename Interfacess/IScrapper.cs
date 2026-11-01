using WebScrapper.Models;

namespace WebScrapper.Interfacess;

public interface IScrapper
{
    public Task<List<EventModel>> ScrapeEvents(DateTime startDate, int daysToScrape);
}

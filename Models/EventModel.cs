namespace WebScrapper.Models;

public class EventModel
{
    public required string title {get; set;}
    public string? description {get; set;}
    public string? city {get; set;}
    public string? time {get; set;}
    public string? location {get; set;}
    public double latitude {get; set;}
    public double longitude {get; set;}
}

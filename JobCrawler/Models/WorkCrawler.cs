namespace JobCrawler.Models;


public class WorkCrawler : IWorkCrawler
{
    public string Title { get; set; }
    public string Location { get; set; }
    public string Keywords { get; set; }
    public string Batch { get; set; }
    public DateTime Date { get; set; }
}
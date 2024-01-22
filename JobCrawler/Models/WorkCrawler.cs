namespace JobCrawler.Models;

/*Will perhaps be used with a database in the future*/
public class WorkCrawler : IWorkCrawler
{
    public string Title { get; set; }
    public string Location { get; set; }
    public string Keywords { get; set; }
    public string Batch { get; set; }
    public DateTime Date { get; set; }
}
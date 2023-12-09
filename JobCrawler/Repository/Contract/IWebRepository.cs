namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    List<string> JobsOfInterest(string keywords, string path);

     void FieldInput(params string[] textInput);
    
     List<string> IterateThroughJobAds(string keywords, string path, int pages);

     List<string> AcquireInterestingJobs(string keywords, string path);

}
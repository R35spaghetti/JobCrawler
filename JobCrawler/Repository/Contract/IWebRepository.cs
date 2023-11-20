namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    List<string> JobsOfInterest(string keywords);

     void FieldInput(params string[] textInput);
    
     List<string> IterateThroughJobAds(string keywords);

     List<string> AcquireInterestingJobs(string keywords);

}
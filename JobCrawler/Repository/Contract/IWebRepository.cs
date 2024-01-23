namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    List<string> JobsOfInterest(List<string> keywords, string path, List<string> negativeKeywords);

     void FieldInput(params string[] textInput);
    
     List<string> IterateThroughJobAds(List<string> keywords, string path, int pages, List<string> negativeKeywords);

     List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords);

     string FilterJobAd(string jobAdInfo, List<string> keywords, string path, List<string> negativeKeywords);
}
namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    string GrabText(string keywords);

     void FieldInput(params string[] textInput);
    
     List<string> IterateThroughJobAds(string keywords);
}
namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    string GrabText(string keywords);

    void FieldInput(string textInput);
    
    void IterateJobAds();
}
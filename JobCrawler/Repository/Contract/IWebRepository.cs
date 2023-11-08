namespace JobCrawler.Repository.Contract;

public interface IWebRepository
{
    void NavigateTo(string url);

    string GrabText(string keywords);

    string FieldInput(string textInput);
}
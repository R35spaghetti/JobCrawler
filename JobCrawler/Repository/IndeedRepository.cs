using JobCrawler.Repository.Contract;

namespace JobCrawler.Repository;

public class IndeedRepository : IWebRepository
{
    public void NavigateTo(string url)
    {
        throw new NotImplementedException();
    }

    public string GrabText(string keywords)
    {
        throw new NotImplementedException();
    }

    public void FieldInput(params string[] textInput)
    {
        throw new NotImplementedException();
    }

    public void IterateJobAds()
    {
        throw new NotImplementedException();
    }
}
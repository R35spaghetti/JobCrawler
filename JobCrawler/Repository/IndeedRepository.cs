using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace JobCrawler.Repository;

public class IndeedRepository : IWebRepository
{
    private readonly IWebDriver _driver = new FirefoxDriver();


    public void NavigateTo(string url)
    {
        _driver.Navigate().GoToUrl(url);
    }

    public List<string> JobsOfInterest(string keywords)
    {
        throw new NotImplementedException();
    }

    public void FieldInput(params string[] textInput)
    {
        Actions action = new Actions(_driver);
        IWebElement what = _driver.FindElement(By.Id("text-input-what"));
        IWebElement where = _driver.FindElement(By.Id("text-input-where"));
        IWebElement search =
            _driver.FindElement(By.CssSelector("button.yosegi-InlineWhatWhere-primaryButton[type='submit']"));
        what.SendKeys(textInput[0]);
        where.SendKeys(textInput[1]);
        action.Click(search).Build().Perform();
    }

    public List<string> IterateThroughJobAds(string keywords)
    {
        throw new NotImplementedException();
    }

    public List<string> AcquireInterestingJobs(string keywords)
    {
        throw new NotImplementedException();
    }
}
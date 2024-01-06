using System.Collections.ObjectModel;
using JobCrawler.Features;
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

    public List<string> JobsOfInterest(List<string> keywords, string path, List<string> negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        int pages = GetAmountOfJobs();

        int GetAmountOfJobs()
        {
            string amount = _driver.FindElement(By.CssSelector(".jobsearch-JobCountAndSortPane-jobCount > span:nth-child(1)")).Text;
            var replacements = new Dictionary<string, string>{{"jobb", ""},{",",""}};
            foreach (var strAmount in replacements)
            {
                amount = amount.Replace(strAmount.Key, strAmount.Value);
            }
            return Convert.ToInt32(amount);
        }

        List<string> jobs = IterateThroughJobAds(keywords, path, pages, negativeKeywords);
        return jobs;
    }

    public void FieldInput(params string[] textInput)
    {
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        Actions action = new Actions(_driver);
        IWebElement what = _driver.FindElement(By.Id("text-input-what"));
        IWebElement where = _driver.FindElement(By.Id("text-input-where"));
        IWebElement search =
            _driver.FindElement(By.CssSelector("button.yosegi-InlineWhatWhere-primaryButton[type='submit']"));
        what.SendKeys(textInput[0]);
        where.SendKeys(textInput[1]);
        action.Click(search).Build().Perform();
    }

    public List<string> IterateThroughJobAds(List<string> keywords, string path, int pages, List<string> negativeKeywords)
    {
        List<string> jobs = new List<string>();
        var jobList = _driver.FindElements(By.CssSelector(".css-zu9cdh"))
            .SelectMany(x => x.FindElements(By.CssSelector(".css-5lfssm.eu4oa1w0")))
                .Where(li=>li.Text != ""); //TODO: Get only job ads

        int jobCount = jobList.Count();
        for(int i = 1; i<= jobCount; i++)
        {
            By locator = By.CssSelector($"li.css-5lfssm:nth-child({i})"); //TODO: Error at i
            var clickJobAd = new ClickElementWrapper(_driver, locator);
            clickJobAd.Click();
            //AcquireInterestingJobs
        }
        GoToNextJobPage();
        
            return jobs;
    }

    private void GoToNextJobPage()
    {
        By locatorNext = By.CssSelector(".css-akkh0a");
        var clickNext = new ClickElementWrapper(_driver, locatorNext);
        clickNext.Click();
    }

    public List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords)
    {
        throw new NotImplementedException();
    }

    public void FolderStructureForAds(string jobAdInfo, string path)
    {
        throw new NotImplementedException();
    }
}
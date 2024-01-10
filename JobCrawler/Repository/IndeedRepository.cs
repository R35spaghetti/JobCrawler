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
        var theJobList = _driver.FindElements(By.CssSelector(
            "li.css-5lfssm:nth-child(n) > div:not(:has(#mosaic-afterFifthJobResult)):not(:has(#mosaic-afterTenthJobResult)) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > h2:nth-child(1) > a:nth-child(1)"));
        
        foreach (var link in theJobList)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", link);
            link.Click();
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
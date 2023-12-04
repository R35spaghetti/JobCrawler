using JobCrawler.Features;
using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace JobCrawler.Repository;

public class ArbetsformedlingenRepository : IWebRepository
{
    private readonly IWebDriver _driver = new FirefoxDriver();


    public void NavigateTo(string url)
    {
        _driver.Navigate().GoToUrl(url);
        Actions action = new Actions(_driver);
        IWebElement button = _driver.FindElement(By.LinkText("Sök jobb i Platsbanken"));
        action.Click(button).Build().Perform();
    }

    public List<string> JobsOfInterest(string keywords)
    {
        List<string> jobs = new List<string>();
        
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        ShowMoreJobAds();

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        jobs = IterateThroughJobAds(keywords);
        return jobs;
    }


    public void FieldInput(params string[] textInput)
    {
        Actions action = new Actions(_driver);

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        IWebElement input = _driver.FindElement(By.Id("search_input"));
        IWebElement search = _driver.FindElement(By.CssSelector(
            "button[type='button'][class='search-button btn btn-lg btn-app-link--custom inverse-focus'][aria-label='Sök']"));
        foreach (var item in textInput) input.SendKeys(item);
        action.Click(search).Build().Perform();
    }
    

    public List<string> IterateThroughJobAds(string keywords)
    {
        List<string> jobs = new List<string>();
        
        for (int i = 1; i <= 100; i++)
        {
            By locator = By.CssSelector($"pb-feature-search-result-card.ng-star-inserted:nth-child({i}) > div:nth-child(1) > div:nth-child(1) > h3:nth-child(1) > a:nth-child(1)");
            var clickJobAd = new StaleElementWrapper(_driver, locator);
            clickJobAd.Click();
            jobs.AddRange(AcquireInterestingJobs(keywords));

            if (i == 100)
            {
                _driver.Navigate().Back();
            }
        }


        return jobs;
    }

    public List<string> AcquireInterestingJobs(string keywords)
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("section.col-md-12"));
        List<string> jobAdInfo = jobAd.Select(element => element.Text).ToList();
        
        string jobs = FilterJobAd(jobAdInfo.First(), keywords);


        return new List<string> { jobs };
    }

    private string FilterJobAd(string jobAdInfo, string keywords)
    {
        if (jobAdInfo.Contains(keywords))
        {
            return jobAdInfo;
        }

        return string.Empty;
    }


    private void ShowMoreJobAds()
    {
        IWebElement showMoreJobAds = _driver.FindElement(By.ClassName("ads-per-page"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", showMoreJobAds);
        new Actions(_driver)
            .Click(showMoreJobAds)
            .Perform();
    }
}
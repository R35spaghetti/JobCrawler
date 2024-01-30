using JobCrawler.Features;
using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;

namespace JobCrawler.Repository;

public class ArbetsformedlingenRepository : IWebRepository
{
    private readonly IWebDriver _driver;

    public ArbetsformedlingenRepository()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--headless");
        _driver = new FirefoxDriver(options);
    }

    public void NavigateTo(string url)
    {
        _driver.Navigate().GoToUrl(url);
    }

    public List<string> JobsOfInterest(List<string> keywords, string path, List<string> negativeKeywords)
    {
        ListUtils.ReplaceNullWithEmpty(negativeKeywords);
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        ShowMoreJobAds();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        int pages = GetJobAdPages();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        List<string> jobs = IterateThroughJobAds(keywords, path, pages, negativeKeywords);
        return jobs;
    }

    private void ShowMoreJobAds()
    {
        IWebElement showMoreJobAds = _driver.FindElement(By.ClassName("ads-per-page"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", showMoreJobAds);
        new Actions(_driver)
            .Click(showMoreJobAds)
            .Perform();
    }

    private int GetJobAdPages()
    {
        try
        {
            IWebElement number = _driver.FindElement(By.CssSelector(".digi-navigation-pagination__page-button--last"));
            return Convert.ToInt32(number.Text);
        }
        catch (NoSuchElementException) //if less than 5 pages
        {
            IWebElement number = _driver.FindElement(By.CssSelector(".digi-typography"));
            int amountOfAds = GetThirdNumber(number.Text);
            return amountOfAds;
        }
    }

    private int GetThirdNumber(string str)
    {
        //Match one or more digits, find all the numbers in the string
        MatchCollection matches = Regex.Matches(str, @"\d+");
        return int.Parse(matches[2].Value);
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


    public List<string> IterateThroughJobAds(List<string> keywords, string path, int pages,
        List<string> negativeKeywords)
    {
        List<string> jobs = new List<string>();
        for (int j = 1; j <= pages; j++)
        {
            for (int i = 1; i <= 100; i++)
            {
                By locator =
                    By.CssSelector(
                        $"pb-feature-search-result-card.ng-star-inserted:nth-child({i}) > div:nth-child(1) > div:nth-child(1) > h3:nth-child(1) > a:nth-child(1)");
                var clickJobAd = new ClickElementWrapper(_driver, locator);
                clickJobAd.Click();
                List<string> result = AcquireInterestingJobs(keywords, path, negativeKeywords);
                if (result.First() != "")
                {
                    jobs.AddRange(result);
                }

                _driver.Navigate().Back();
            }

            CrawlerUtils.GoToNextJobPage(_driver,
                ".digi-button--icon-secondary > span:nth-child(1) > span:nth-child(1)");
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        }

        return jobs;
    }

    public List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("section.col-md-12"));
        List<string> jobAdInfo = jobAd.Select(element => element.GetAttribute("innerHTML")).ToList();

        string jobs = ListUtils.FilterJobAd(jobAdInfo.First(), keywords, path, negativeKeywords, _driver);
        return new List<string> { jobs };
    }
}
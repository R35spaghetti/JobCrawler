using System.Net;
using JobCrawler.Features;
using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;

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

    public List<string> JobsOfInterest(string keywords, string path, string negativeKeywords)
    {
        List<string> jobs = new List<string>();

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        ShowMoreJobAds();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        int pages = GetJobAdPages();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        jobs = IterateThroughJobAds(keywords, path, pages, negativeKeywords);
        return jobs;
    }

    private int GetJobAdPages()
    {
        IWebElement number = _driver.FindElement(By.CssSelector(".digi-navigation-pagination__page-button--last"));
        return Convert.ToInt32(number.Text);
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


    public List<string> IterateThroughJobAds(string keywords, string path, int pages, string negativeKeywords)
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

            NextOnehundred();
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        }

        return jobs;
    }

    private void NextOnehundred()
    {
        By locatorNext = By.CssSelector(".digi-button--icon-secondary > span:nth-child(1) > span:nth-child(1)");
        var clickNext = new ClickElementWrapper(_driver, locatorNext);
        clickNext.Click();
    }

    public List<string> AcquireInterestingJobs(string keywords, string path, string negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("section.col-md-12"));
        List<string> jobAdInfo = jobAd.Select(element => element.GetAttribute("innerHTML")).ToList();


        string jobs = FilterJobAd(jobAdInfo.First(), keywords, path, negativeKeywords);
        return new List<string> { jobs };
    }

    private string FilterJobAd(string jobAdInfo, string keywords, string path, string negativeKeywords)
    {
        /*
         * positive lookbehind with a positive lookahead,
         * matches the specified keywords,
         * even when it is preceded or followed by a whitespace (\s) character or a punctuation character (\p{{P}})
         */
        if (Regex.IsMatch(jobAdInfo.ToUpper(), $@"(?<=^|[\s\p{{P}}]){negativeKeywords.ToUpper()}(?=[\s\p{{P}}]|$)"))
        {
            return string.Empty;

        }
        else if (Regex.IsMatch(jobAdInfo.ToUpper(), $@"(?<=^|[\s\p{{P}}]){keywords.ToUpper()}(?=[\s\p{{P}}]|$)"))
        {
            var title = _driver.FindElement(By.CssSelector("#pb-company-name"));
            string textValue = title.Text;
            path += $"{textValue}.html";
            File.WriteAllText(path, jobAdInfo);
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
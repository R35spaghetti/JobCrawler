using System.Text.RegularExpressions;
using JobCrawler.Features;
using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace JobCrawler.Repository;

public class IndeedRepository : IWebRepository
{
    private readonly IWebDriver _driver;
    public IndeedRepository()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-blink-features");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
        _driver = new ChromeDriver(options);
    }
    public void NavigateTo(string url) 
    {   //flags as undefined
        var jsScript = "Object.defineProperty(navigator, 'webdriver', {get: () => undefined})";
        ((IJavaScriptExecutor)_driver).ExecuteScript(jsScript);
        _driver.Navigate().GoToUrl(url);
    }

    public List<string> JobsOfInterest(List<string> keywords, string path, List<string> negativeKeywords)
    {
        ListUtils.ReplaceNullWithEmpty(negativeKeywords);
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        int pages = GetAmountOfJobs();
        int GetAmountOfJobs()
        {
            var jobs = _driver.FindElement(By.CssSelector(".jobsearch-JobCountAndSortPane-jobCount > span:nth-child(1)")).Text;
            MatchCollection matches = Regex.Matches(jobs, @"\d+");
            jobs = string.Join("", matches.Select(m => m.Value));
            return Convert.ToInt32(jobs);
        }

        List<string> jobs = IterateThroughJobAds(keywords, path, pages, negativeKeywords);
        return jobs;
    }
    public void FieldInput(params string[] textInput)
    {
        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
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
        for (int i = 1; i <= pages; i++)
        {
            var jobList = _driver.FindElements(By.CssSelector(
                "li.css-5lfssm:nth-child(n) > div:not(:has(#mosaic-afterFifthJobResult)):not(:has(#mosaic-afterTenthJobResult)) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > h2:nth-child(1) > a:nth-child(1)"));
            foreach (var job in jobList)
            {
                By locator = By.CssSelector(
                    $"li.css-5lfssm:nth-child(n) > div:not(:has(#mosaic-afterFifthJobResult)):not(:has(#mosaic-afterTenthJobResult)) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > h2:nth-child(1) > a:nth-child(1)");
                var clickJobAd = new ClickElementWrapper(_driver, locator, job);
                clickJobAd.Click();
                var result = AcquireInterestingJobs(keywords, path, negativeKeywords);
                if (result.First() != "")
                {
                    jobs.AddRange(result);
                }

                pages--;
            }
            if (pages != 0 && pages > 0)
            {
                GoToNextJobPage();
            }
        }

        return jobs;
    }

    private void GoToNextJobPage()
    {
        By locatorNextPageArrowButton = By.CssSelector("li.css-227srf:last-child");
        var clickNext = new ClickElementWrapper(_driver, locatorNextPageArrowButton);
        clickNext.Click();
    }

    public List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("div.jobsearch-JobComponent-description:nth-child(2)"));
        List<string> jobAdInfo = jobAd.Select(element => element.GetAttribute("innerHTML")).ToList();
        string jobs = ListUtils.FilterJobAd(jobAdInfo.First(), keywords, path, negativeKeywords, _driver);
        return new List<string> { jobs };
    }
    

}
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
        ReplaceNullWithEmpty(negativeKeywords);
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        ShowMoreJobAds();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        int pages = GetJobAdPages();
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        List<string> jobs = IterateThroughJobAds(keywords, path, pages, negativeKeywords);
        return jobs;
    }
    private void ReplaceNullWithEmpty(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = "";
            }
        }
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

            NextOneHundred();
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        }

        return jobs;
    }

    private void NextOneHundred()
    {
        By locatorNext = By.CssSelector(".digi-button--icon-secondary > span:nth-child(1) > span:nth-child(1)");
        var clickNext = new ClickElementWrapper(_driver, locatorNext);
        clickNext.Click();
    }

    public List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("section.col-md-12"));
        List<string> jobAdInfo = jobAd.Select(element => element.GetAttribute("innerHTML")).ToList();

        string jobs = FilterJobAd(jobAdInfo.First(), keywords, path, negativeKeywords);
        return new List<string> { jobs };
    }

    public string FilterJobAd(string jobAdInfo, List<string> keywords, string path, List<string> negativeKeywords)
    {
        /*
         * positive lookbehind with a positive lookahead,
         * matches the specified keywords,
         * even when it is preceded or followed by a whitespace (\s) character or a punctuation character (\p{{P}})
         */
        if (keywords.Count == 1 && negativeKeywords.Count == 1)
        {
            string escapedStrNeg = Regex.Escape(negativeKeywords.First());
            string escapedStrPos = Regex.Escape(keywords.First());

            if (Regex.IsMatch(jobAdInfo.ToUpper(), $@"(?<=^|[\s\p{{P}}]){escapedStrNeg.ToUpper()}(?=[\s\p{{P}}]|$)"))
            {
                return string.Empty;
            }
            else if (Regex.IsMatch(jobAdInfo.ToUpper(),
                         $@"(?<=^|[\s\p{{P}}]){escapedStrPos.ToUpper()}(?=[\s\p{{P}}]|$)"))
            {
                FolderStructureForAds(jobAdInfo, path);
                return jobAdInfo;
            }
        }
        else
        {
            bool desirable = true;
            Parallel.ForEach(negativeKeywords, strNeg =>
            {
                string escapedStr = Regex.Escape(strNeg);
                if (Regex.IsMatch(jobAdInfo.ToUpper(), $@"(?<=^|[\s\p{{P}}]){escapedStr.ToUpper()}(?=[\s\p{{P}}]|$)"))
                {
                    desirable = false;
                }
            });
            Parallel.ForEach(keywords, strPos =>
            {
                string escapedStr = Regex.Escape(strPos);
                if (Regex.IsMatch(jobAdInfo.ToUpper(), $@"(?<=^|[\s\p{{P}}]){escapedStr.ToUpper()}(?=[\s\p{{P}}]|$)"))
                {
                    desirable = true;
                }
            });

            if (desirable)
            {
                FolderStructureForAds(jobAdInfo, path);
                return jobAdInfo;
            }
        }

        return string.Empty;
    }

    public void FolderStructureForAds(string jobAdInfo, string path)
    {
        var title = _driver.FindElement(By.CssSelector("#pb-company-name"));
        jobAdInfo += $"<a href='{_driver.Url}'>Go to job ad</a>";
        string documentName = title.Text;
        string headFolderName = GetHeadFolderName();
        string subFolderName = GetSubFolderName();
        string folderPath = $"{path}/{headFolderName}/{subFolderName}";

        Directory.CreateDirectory(folderPath);
        File.WriteAllText(folderPath + $"/{documentName}.html", jobAdInfo);
    }

    private string GetSubFolderName()
    {
        IWebElement subFolderName = _driver.FindElement(By.CssSelector("#pb-job-location"));
        if (subFolderName.Text == "")
        {
            return "Unknown";
        }
        else
        {
            var newSubFolderName = subFolderName.Text.Replace("Kommun: ", "").Trim();
            return newSubFolderName;
        }
    }

    private string GetHeadFolderName()
    {
        IWebElement folderTitle = _driver.FindElement(By.CssSelector(".extra-info-section > h2:nth-child(2)"));
        string dateTitle = GetProperTitle(folderTitle.Text);

        if (dateTitle == "")
        {
            return "Unknown";
        }

        return dateTitle;
    }

    private string GetProperTitle(string folderTitle)
    {
        string[] months =
        {
            "januari", "februari", "mars", "april", "maj", "juni", "juli", "augusti", "september", "oktober",
            "november", "december"
        };
        string monthsPattern = "(?:" + string.Join("|", months) + ")";
        string datePattern = $@"\s*\d{{1,2}}\s*{monthsPattern}\s*\d{{4}}";
        Match date = Regex.Match(folderTitle, datePattern);

        return date.Value;
    }
}
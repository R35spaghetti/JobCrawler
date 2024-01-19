using System.Text.RegularExpressions;
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
        By locatorNext = By.CssSelector(".css-akkh0a");
        var clickNext = new ClickElementWrapper(_driver, locatorNext);
        clickNext.Click();
    }

    public List<string> AcquireInterestingJobs(List<string> keywords, string path, List<string> negativeKeywords)
    {
        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        IList<IWebElement> jobAd = _driver.FindElements(By.CssSelector("div.jobsearch-JobComponent-description:nth-child(2)"));
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
        var title = _driver.FindElement(By.CssSelector(".jobsearch-JobInfoHeader-title > span:nth-child(1)"));
        jobAdInfo += $"<a href='{_driver.Url}'>Go to job ad</a>";
        string documentName = title.Text;
        string headFolderName = GetHeadFolderName();
        string folderPath = $"{path}/{headFolderName}/"; //No proper dates on indeed, yet

        Directory.CreateDirectory(folderPath);
        File.WriteAllText(folderPath + $"/{documentName}.html", jobAdInfo);    
    }
}
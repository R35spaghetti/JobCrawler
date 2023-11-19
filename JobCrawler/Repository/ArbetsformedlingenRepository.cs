using System.Collections.ObjectModel;
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

    public string GrabText(string keywords)
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        ShowMoreJobAds();

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        keywords = IterateThroughJobAds(keywords);
        return "";
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

    public void IterateJobAds()
    {
        throw new NotImplementedException();
    }

    private string IterateThroughJobAds(string keywords)
    {
        var jobAdContainer = _driver.FindElement(By.ClassName("result-container"));
        ReadOnlyCollection<IWebElement> jobAds = jobAdContainer.FindElements(By.ClassName("ng-star-inserted"));
        foreach (var jobAd in jobAds)
        {
            IWebElement clickJobAd = _driver.FindElement(By.CssSelector("div.card-container h3 a"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", jobAd);
            new Actions(_driver)
                .Click(clickJobAd)
                .Perform();
        }

        return keywords;
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
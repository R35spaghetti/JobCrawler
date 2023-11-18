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
        IWebElement showMoreJobAds = _driver.FindElement(By.ClassName("ads-per-page"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", showMoreJobAds);
        new Actions(_driver)
            .Click(showMoreJobAds)
            .Perform();

        return "";
        //TODO sort of
        //klicka på denna class="ads-per-page"
        //ta in alla annonser
        //gå igenom varje annons
        //vid varje annons ta ut ett värde med nyckelorden
        //resultatet blir vald annons med nyckelorden och ungefär en kortfattad del av annonsen
    }
    public void FieldInput(params string[] textInput)
    {        Actions action = new Actions(_driver);

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
        IWebElement input = _driver.FindElement(By.Id("search_input"));
        IWebElement search = _driver.FindElement(By.CssSelector("button[type='button'][class='search-button btn btn-lg btn-app-link--custom inverse-focus'][aria-label='Sök']"));
        foreach (var item in textInput) input.SendKeys(item);
        action.Click(search).Build().Perform();
    }

    public void IterateJobAds()
    {
        throw new NotImplementedException();
    }
}
using JobCrawler.Repository.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace JobCrawler.Repository;

public class WebRepository : IWebRepository
{
    private readonly IWebDriver _driver = new FirefoxDriver();

    public void NavigateTo(string url)
    {
        _driver.Navigate().GoToUrl(url);
        Actions action = new Actions(_driver);
        IWebElement button = _driver.FindElement(By.CssSelector(".cms-btn-app-link.blue-quick.quick"));
        action.Click(button).Build().Perform();
    }
}
using OpenQA.Selenium;

namespace JobCrawler.Features;

public static class CrawlerUtils
{
    public static void GoToNextJobPage(IWebDriver driver, string text)
    {
        By locatorNext = By.CssSelector(text);
        var clickNext = new ClickElementWrapper(driver, locatorNext);
        clickNext.Click();
    }
}
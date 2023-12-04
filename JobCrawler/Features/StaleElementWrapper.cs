using OpenQA.Selenium;

namespace JobCrawler.Features;

public class StaleElementWrapper
{
    private By locator;
    private IWebElement element;
    private IWebDriver driver;

    public StaleElementWrapper(IWebDriver driver, By locator)
    {
        this.driver = driver;
        this.locator = locator;
        this.element = driver.FindElement(locator);
    }

    public void Click()
    {
        try
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            element.Click();
        }
        catch (StaleElementReferenceException)
        {
            element = driver.FindElement(locator);
            element.Click();
        }
    }
}
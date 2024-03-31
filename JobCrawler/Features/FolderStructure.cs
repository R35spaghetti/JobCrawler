using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace JobCrawler.Features;

public static class FolderStructure
{
    public static void FolderStructureForAdsWithDates(string jobAdInfo, string path, IWebDriver driver,
        string titleName, string headName, string subfolderName)
    {
        var title = driver.FindElement(By.CssSelector(titleName));
        jobAdInfo += $"<a href='{driver.Url}'>Go to job ad</a>";
        string documentName = title.Text;
        string headFolderName = GetHeadFolderNameWithDates(driver, headName);
        string subFolderName = GetSubFolderNameWithDates(driver, subfolderName);
        string folderPath = $"{path}/{headFolderName}/{subFolderName}";

        Directory.CreateDirectory(folderPath);
        File.WriteAllText(folderPath + $"/{documentName}.html", jobAdInfo);
    }

    private static string GetHeadFolderNameWithDates(IWebDriver driver, string headName)
    {
        IWebElement folderTitle = driver.FindElement(By.CssSelector(headName));
        string dateTitle = GetProperTitleWithDates(folderTitle.Text);

        if (dateTitle == "")
        {
            return "Unknown";
        }

        return dateTitle;
    }

    private static string GetProperTitleWithDates(string folderTitle)
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

    private static string GetSubFolderNameWithDates(IWebDriver driver, string subfolderName)
    {
        IWebElement subFolderName = driver.FindElement(By.CssSelector(subfolderName));
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
    public static void FolderStructureForAdsWithoutDates(string jobAdInfo, string path, IWebDriver driver,
        string titleName, string headName)
    {
        var title = driver.FindElement(By.CssSelector(titleName));
        jobAdInfo += $"<a href='{driver.Url}'>Go to job ad</a>";
        string documentName = title.Text;
        string headFolderName = GetHeadFolderNameWithoutDates(driver, headName);
        string folderPath = $"{path}/{headFolderName}/"; //No proper dates on indeed, yet

        Directory.CreateDirectory(folderPath);
        File.WriteAllText(folderPath + $"/{documentName}.html", jobAdInfo);
    }

    private static string GetHeadFolderNameWithoutDates(IWebDriver driver, string headName)
    {
        IWebElement headFolderName = driver.FindElement(By.CssSelector(headName));
        if (headFolderName.Text == "")
        {
            return "Unknown";
        }

        return headFolderName.Text;
    }
}
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace JobCrawler.Features;

public static class ListUtils
{
    public static void ReplaceNullWithEmpty(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = "";
            }
        }
    }

    public static string FilterJobAd(string jobAdInfo, List<string> keywords, string path,
        List<string> negativeKeywords, IWebDriver driver)
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
                FolderStructure.FolderStructureForAdsWithDates(jobAdInfo, path, driver, "#pb-company-name",
                    ".extra-info-section > h2:nth-child(2)", "#pb-job-location");
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
                FolderStructure.FolderStructureForAdsWithDates(jobAdInfo, path, driver, "#pb-company-name",
                    ".extra-info-section > h2:nth-child(2)", "#pb-job-location");
                return jobAdInfo;
            }
        }

        return string.Empty;
    }
}
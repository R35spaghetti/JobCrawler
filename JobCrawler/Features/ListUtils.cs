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
}

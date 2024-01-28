using Microsoft.AspNetCore.Mvc;

namespace JobCrawler.Controllers;

[ApiController]
[Route("[controller]")]
public class WebController : ControllerBase
{
    private readonly WebRepositoryResolver _webRepositoryResolver;

    public WebController(WebRepositoryResolver webRepositoryResolver)
    {
        _webRepositoryResolver = webRepositoryResolver;
    }
    //Automatic crawl from latest to oldest
    [HttpGet("StartCrawlingArbetsförmedlingen")]
    public ActionResult StartCrawlAF(string input,[FromQuery]  List<string> keywords, string path,[FromQuery] List<string> negativeKeywords)
    {
        var webRepositoryResolver = _webRepositoryResolver("A");
        webRepositoryResolver.NavigateTo("https://arbetsformedlingen.se/platsbanken/");

        webRepositoryResolver.FieldInput(input);
        /*Will perhaps be used with a database in the future*/
        webRepositoryResolver.JobsOfInterest(keywords, path,negativeKeywords);
        
        return Ok();

    }
    //Automatic crawl from latest to oldest
    [HttpGet("StartCrawlingIndeed")]
    public ActionResult StartCrawlIndeed(string what, string where, [FromQuery]  List<string> keywords,string path, [FromQuery] List<string> negativeKeywords)
    {
        var webRepository = _webRepositoryResolver("B");
        webRepository.NavigateTo("https://se.indeed.com/");
        string[] inputs = new[] { what, where};
        webRepository.FieldInput(inputs);
        webRepository.JobsOfInterest(keywords, path,negativeKeywords);
        return Ok();
    }
    [HttpGet("StartCrawlingIndeedAndAF")]
    public ActionResult StartCrawlArbetsformedlingenAndIndeed(string job,[FromQuery]  List<string> keywords, string path,[FromQuery] List<string> negativeKeywords, string whereIndeed)
    {
        Thread afThread = new Thread(obj => StartCrawlAF(job, keywords, path, negativeKeywords));
        Thread indeedThread = new Thread(obj => StartCrawlIndeed(job, whereIndeed, keywords, path, negativeKeywords));
        
        afThread.Start();
        indeedThread.Start();

        afThread.Join();
        indeedThread.Join();
        
       
        return Ok();
    }
}
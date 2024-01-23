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
    public Task<ActionResult> StartCrawlAF(string input,[FromQuery]  List<string> keywords, string path,[FromQuery] List<string> negativeKeywords)
    {
        var webRepositoryResolver = _webRepositoryResolver("A");
        webRepositoryResolver.NavigateTo("https://arbetsformedlingen.se/");

        webRepositoryResolver.FieldInput(input);
        /*Will perhaps be used with a database in the future*/
        webRepositoryResolver.JobsOfInterest(keywords, path,negativeKeywords);
        
        return Task.FromResult<ActionResult>(Ok());
    }
    //Automatic crawl from latest to oldest
    [HttpGet("StartCrawlingIndeed")]
    public Task<ActionResult> StartCrawlIndeed(string what, string where, [FromQuery]  List<string> keywords,string path, [FromQuery] List<string> negativeKeywords)
    {
        var webRepository = _webRepositoryResolver("B");
        webRepository.NavigateTo("https://se.indeed.com/");
        string[] inputs = new[] { what, where};
        webRepository.FieldInput(inputs);
        webRepository.JobsOfInterest(keywords, path,negativeKeywords);
        return Task.FromResult<ActionResult>(Ok());
    }
}
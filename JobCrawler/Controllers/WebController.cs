using JobCrawler.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace JobCrawler.Controllers;

[ApiController]
[Route("[controller]")]
public class WebController : ControllerBase
{
    private readonly IWebRepository _webRepository;

    public WebController(IWebRepository webRepository)
    {
        _webRepository = webRepository;
    }

    [HttpGet("StartCrawling")]
    public Task<ActionResult> CrawlTest()
    {
        _webRepository.NavigateTo("https://arbetsformedlingen.se/");
        return Task.FromResult<ActionResult>(Ok());
    }

   
}
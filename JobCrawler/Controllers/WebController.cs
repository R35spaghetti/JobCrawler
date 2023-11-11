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
    //Testing
    [HttpGet("StartCrawlingArbetsförmedlingen")]
    public Task<ActionResult> StartCrawlAF(string input)
    {
        _webRepository.NavigateTo("https://arbetsformedlingen.se/");
        
        _webRepository.FieldInput(input);
        return Task.FromResult<ActionResult>(Ok());
    }    
  

   
}
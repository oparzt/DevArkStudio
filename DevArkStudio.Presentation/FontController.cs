using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;

public record FontRequest
{
    [Required] 
    [JsonPropertyName("fontName")]
    public string FontName { get; init; }
}

public record FontConnectRequest : FontRequest
{
    [Required]
    [JsonPropertyName("pageName")]
    public string PageName { get; init; }
    
    [Required]
    [JsonPropertyName("connect")]
    public bool Connect { get; init; }
}

public record FontCreateRequest : FontRequest
{
    [Required]
    [JsonPropertyName("fontFiles")]
    public IFormFile FontFiles { get; init; }
    
    [Required]
    [JsonPropertyName("fontStyle")]
    public string FontStyle { get; init; }
    
    [Required]
    [JsonPropertyName("fontWeight")]
    public string FontWeight { get; init; }
}

[ApiController]
[Route("/api/[Controller]/[Action]")]
public class FontController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProjectFonts([FromServices] FontService fontService)
    {
        return new JsonResult(fontService.GetAllFonts());
    }

    [HttpGet]
    public IActionResult GetConnectedFonts([FromQuery] string pageName, [FromServices] FontService fontService)
    {
        return new JsonResult(fontService.GetConnectedFontsList(pageName));
    }

    [HttpPost]
    public IActionResult ConnectFont([FromBody] FontConnectRequest fontConnectRequest,
        [FromServices] FontService fontService)
    {
        return new JsonResult(fontService.ConnectFont(fontConnectRequest.PageName, fontConnectRequest.FontName,
            fontConnectRequest.Connect));
    }
    
    [HttpPost]
    public IActionResult UploadFont([FromForm] FontCreateRequest fontCreateRequest, [FromServices] FontService fontService)
    {
        return new JsonResult(fontService.UploadFont(fontCreateRequest.FontName, fontCreateRequest.FontStyle, 
            fontCreateRequest.FontWeight, fontCreateRequest.FontFiles));
    }
    
    
}
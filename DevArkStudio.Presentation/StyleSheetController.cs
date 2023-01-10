using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;

public record SheetRequest
{
    [Required]
    [JsonPropertyName("sheetName")]
    public string SheetName { get; init; }
}

public record ComponentRequest : SheetRequest
{
    [Required]
    [JsonPropertyName("styleID")]
    public string StyleID { get; init; }
}

public record CreateComponentRequest : ComponentRequest
{
    [Required]
    [JsonPropertyName("styleManipulation")]
    public StyleManipulation StyleManipulation { get; init; }
}

public record UpdateComponentRequest : ComponentRequest
{
    [Required]
    [JsonPropertyName("selector")]
    public string Selector { get; init; }

    [Required]
    [JsonPropertyName("styles")]
    public Dictionary<string, string> Styles { get; init; }
}

[ApiController]
[Route("/api/[controller]/[action]")]
public class StyleSheetController : ControllerBase
{
    [HttpGet]
    public IActionResult GetStyleSheet([FromQuery] SheetRequest sheetRequest,
        [FromServices] StyleSheetService styleSheetService)
    {
        return new JsonResult(styleSheetService.GetStyleSheet(sheetRequest.SheetName));
    }

    [HttpPost]
    public IActionResult CreateStyleComponent([FromBody] CreateComponentRequest createComponentRequest,
        [FromServices] StyleSheetService styleSheetService)
    {
        return new JsonResult(styleSheetService.CreateStyleComponent(createComponentRequest.SheetName,
            createComponentRequest.StyleID, createComponentRequest.StyleManipulation));
    }
    
    [HttpGet]
    public IActionResult GetComponentStyles([FromQuery] ComponentRequest componentRequest,
        [FromServices] StyleSheetService styleSheetService)
    {
        return new JsonResult(
            styleSheetService.GetComponentStyles(componentRequest.SheetName, componentRequest.StyleID));
    }

    [HttpPost]
    public IActionResult UpdateComponentStyles([FromBody] UpdateComponentRequest updateComponentRequest,
        [FromServices] StyleSheetService styleSheetService)
    {
        return new JsonResult(
            styleSheetService.UpdateComponentStyles(updateComponentRequest.SheetName, updateComponentRequest.StyleID,
                updateComponentRequest.Selector, updateComponentRequest.Styles));
    }

    [HttpPost]
    public IActionResult RemoveComponentStyles([FromBody] ComponentRequest componentRequest,
        [FromServices] StyleSheetService styleSheetService)
    {
        return new JsonResult(
            styleSheetService.RemoveComponentStyles(componentRequest.SheetName, componentRequest.StyleID));
    }
}
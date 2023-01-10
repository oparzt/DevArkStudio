using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;



public record NodeRequest
{
    [Required]
    [JsonPropertyName("pageName")]
    public string PageName { get; init; }
    
    [Required]
    [JsonPropertyName("nodeID")]
    public string NodeID { get; init; }
}

public record UpdateClassNameRequest : NodeRequest
{
    [JsonPropertyName("className")]
    [MinLength(0)]
    public string ClassName { get; init; }
}

public record UpdateStyleIDRequest : NodeRequest
{
    [JsonPropertyName("styleID")]
    [MinLength(0)]
    public string StyleID { get; init; }
}

public record UpdateAttributesRequest : NodeRequest
{
    [Required]
    [JsonPropertyName("attributes")]
    public Dictionary<string, string> Attributes { get; init; }
}

public record UpdateTextContentRequest : NodeRequest
{
    [Required]
    [JsonPropertyName("textContent")]
    public string TextContent { get; init; }
}

public record UpdateTagRequest : NodeRequest
{
    [Required]
    [JsonPropertyName("tagName")]
    public string TagName { get; init; }
}

public record TransferNodeRequest : NodeRequest {
    [Required]
    [JsonPropertyName("targetNodeID")]
    public string TargetNodeID { get; init; }

    [Required] 
    [JsonPropertyName("nodeManipulation")]
    public NodeManipulation NodeManipulation { get; init; }
}

public record CreateTextNodeRequest : NodeRequest
{
    [Required]
    [JsonPropertyName("nodeManipulation")]
    public NodeManipulation NodeManipulation { get; init; }
}

public record CreateElmNodeRequest : NodeRequest
{
    [Required]
    [JsonPropertyName("targetNodeTag")]
    public string TargetNodeTag { get; init; }
    
    [Required]
    [JsonPropertyName("nodeManipulation")]
    public NodeManipulation NodeManipulation { get; init; }
}

// public 


[ApiController]
[Route("/api/[Controller]/[Action]")]
public class PageController : ControllerBase
{
    // GET
    [HttpGet]
    public IActionResult GetTree([FromQuery] string pageName,[FromServices] PageService pageService)
    {
        return new JsonResult(pageService.GetTree(pageName));
    }

    [HttpGet]
    public IActionResult GetNodeInfo([FromQuery] NodeRequest nodeRequest,
        [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.GetNode(nodeRequest.PageName, nodeRequest.NodeID));
    }

    [HttpPost]
    public IActionResult UpdateClassName([FromBody] UpdateClassNameRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.UpdateElementClass(request.PageName, request.NodeID, request.ClassName));
    }

    [HttpPost]
    public IActionResult UpdateID([FromBody] UpdateStyleIDRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.UpdateElementID(request.PageName, request.NodeID, request.StyleID));
    }
    
    [HttpPost]
    public IActionResult UpdateAttributes([FromBody] UpdateAttributesRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.UpdateElementAttributes(request.PageName, request.NodeID, request.Attributes));
    }
    
    [HttpPost]
    public IActionResult UpdateTextContent([FromBody] UpdateTextContentRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.UpdateTextContent(request.PageName, request.NodeID, request.TextContent));
    }
    
    [HttpPost]
    public IActionResult UpdateTagName([FromBody] UpdateTagRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.UpdateTagName(request.PageName, request.NodeID, request.TagName));
    }
    
    [HttpPost]
    public IActionResult DeleteNode([FromBody] NodeRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.DeleteNode(request.PageName, request.NodeID));
    }

    [HttpPost]
    public IActionResult TransferNode([FromBody] TransferNodeRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.TransferNode(request.PageName, request.NodeID, 
            request.TargetNodeID, request.NodeManipulation));
    }

    [HttpPost]
    public IActionResult CreateTextNode([FromBody] CreateTextNodeRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.CreateTextNode(request.PageName, request.NodeID, request.NodeManipulation));
    }
    
    [HttpPost]
    public IActionResult CreateElmNode([FromBody] CreateElmNodeRequest request, [FromServices] PageService pageService)
    {
        return new JsonResult(pageService.CreateElmNode(request.PageName, request.NodeID, 
            request.TargetNodeTag, request.NodeManipulation));
    }
}
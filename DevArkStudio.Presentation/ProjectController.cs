#nullable enable
using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence;
using DevArkStudio.Persistence.DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DevArkStudio.Presentation;


public class LoadPathDTO
{
    public string Path { get; set; }
}

public class ProjectPathDTO
{
    public string? Path { get; set; }
    public bool Ok { get; set; }
}

public class CreateProjectDTO
{
    public bool Ok { get; init; }
    public string ProjectName { get; init; }
    public string? Error { get; init; }    
}

public class LoadProjectDTO
{
    public bool Ok { get; init; }
    public ProjectDTO? ProjectDTO { get; init; }
    public string? Error { get; init; }
}

[ApiController]
[Route("/api/[controller]/[action]")]
public class ProjectController : ControllerBase
{
    [HttpGet()]
    public IActionResult GetProjectsNames([FromServices] ProjectLoaderService projectLoaderService)
    {
        return new JsonResult(projectLoaderService.GetSavedProjectsNames());
    }

    [HttpPost()]
    public IActionResult CreateProject([FromQuery] string name, [FromServices] ProjectService projectService)
    {
        var completeCreate = projectService.CreateProject(name);

        return new JsonResult(new CreateProjectDTO
        {
            Ok = completeCreate.Item1, 
            ProjectName = name,
            Error = completeCreate.Item1 ? null : completeCreate.Item2
        });
    }
    
    [HttpGet()]
    public IActionResult LoadProject([FromQuery] string name, [FromServices] ProjectService projectService)
    {
        var loadResult = projectService.LoadProject(name);

        return new JsonResult(new LoadProjectDTO
        {
            Ok = loadResult.Item1,
            ProjectDTO = loadResult.Item1 ? new ProjectDTO(loadResult.Item2, name) : null,
            Error = loadResult.Item1 ? loadResult.Item3 : null
        });
    }

    [HttpGet]
    public IActionResult SaveProject([FromServices] ProjectService projectService)
    {
        var complete = projectService.SaveProject();
        return complete ? new OkResult() : new BadRequestResult();
    }

    [HttpGet]
    public IActionResult ExportProject([FromServices] ProjectService projectService)
    {
        var result = projectService.ExportProject();
        if (!result.Item1 || result.Item2 is null) return new BadRequestResult();
        const string contentType = "application/zip";
        var mediaContentType = MediaTypeHeaderValue.Parse(contentType);
        return new FileStreamResult(new MemoryStream(result.Item2), mediaContentType);
    }
    
    // // GET
    // [HttpGet()]
    // [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    // public IActionResult Get([FromServices] ProjectService projectService, [FromServices] ProjectLoaderService projectLoaderService)
    // {
    //     // projectService.Project
    //     return new OkObjectResult(projectLoaderService.GetCurrentDirectory());
    // }
    //
    // [HttpPost()]
    // [ProducesResponseType(typeof(ProjectPathDTO),StatusCodes.Status200OK, "application/json")]
    // public IActionResult LoadByPath([FromBody] LoadPathDTO loadPathDTO, 
    //     [FromServices] ProjectLoaderService projectLoaderService)
    // {
    //     var ok = projectLoaderService.LoadProjectSrcDirectory(loadPathDTO.Path);
    //     return new OkObjectResult(new ProjectPathDTO
    //     {
    //         Path = projectLoaderService.GetProjectSrcDirectoryPath(),
    //         Ok = ok
    //     });
    // }
    
    
}
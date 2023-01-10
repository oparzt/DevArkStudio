#nullable enable
using System;
using System.IO;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;


public class RenderAnswer
{
    public bool Ok { get; init; }
}

public class TextRenderAnswer : RenderAnswer
{
    public string Answer { get; init; }
}

public class FileRenderAnswer : RenderAnswer
{
    public Stream? Answer { get; init; }
}

#nullable enable
public class ProjectService
{
    private readonly ProjectLoaderService _projectLoaderService;
    public Project? Project { get; private set; }
    
    public ProjectService([FromServices] ProjectLoaderService projectLoaderService)
    {
        _projectLoaderService = projectLoaderService;
    }

    public RenderAnswer RenderHTML(string path)
    {
        try
        {
            if (Project is null) throw new Exception("Проект не загружен");
            if (!Project.Pages.ContainsKey(path)) throw new Exception($"Такой страницы не существует: {path}");
            
            return new TextRenderAnswer { Ok = true, Answer = Project.Pages[path].RenderPage(true) };
        }
        catch (Exception e)
        {
            return new RenderAnswer { Ok = false };
        }
    }

    public RenderAnswer RenderCSS(string path)
    {
        try
        {
            if (Project is null) throw new Exception("Проект не загружен");
            if (!Project.StyleSheets.ContainsKey(path)) throw new Exception($"Такого файла CSS не существует: {path}");
            
            return new TextRenderAnswer { Ok = true, Answer = Project.StyleSheets[path].RenderStyleSheet(true) };
        }
        catch (Exception e)
        {
            return new RenderAnswer { Ok = false };
        }
    }

    public RenderAnswer RenderFile(string path)
    {
        // Console.WriteLine(path);
        try
        {
            if (Project is null) throw new Exception("Проект не загружен");
            var fileGetRes = _projectLoaderService.GetFileFromProject(Project, path);
            
            return new FileRenderAnswer { Ok = fileGetRes.Item1, Answer = fileGetRes.Item2};
        }
        catch (Exception e)
        {
            return new RenderAnswer { Ok = false };
        }
    }

    public (bool, byte[]?) ExportProject()
    {
        if (Project is null) return (false, null);
        return !SaveProject() ? (false, null) : _projectLoaderService.GetAllProject(Project);
    }
    
    public (bool, string?) CreateProject(string name)
    {
        if (name.Contains("/") || name.Contains("\\")) return (false, "Использование этих символов (\\, /) запрещено");
        return _projectLoaderService.CreateProject(name);
    }

    public (bool, Project?, string?) LoadProject(string name)
    {
        UnloadProject();
        var result = _projectLoaderService.LoadProject(name);
        if (result.Item1) Project = result.Item2;

        return result;
    }

    public void UnloadProject()
    {
        Project = null;
    }

    public bool SaveProject() => Project is not null && _projectLoaderService.SaveProject(Project);
    
   
}
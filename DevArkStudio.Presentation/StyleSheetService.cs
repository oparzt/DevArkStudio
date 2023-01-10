#nullable enable
using System.Collections.Generic;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence.DTOModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;

public class StyleSheetAnswer
{
    public bool Ok { get; init; }
    public StyleSheetDTO? StyleSheetDTO { get; init; }
}

public class StyleComponentAnswer
{
    public bool Ok { get; init; }
    public StyleComponentDTO? StyleComponentDTO { get; init; }
}

public class StyleSheetWithComponentAnswer : StyleSheetAnswer
{
    public StyleComponentDTO? StyleComponentDTO { get; init; }
}

public class StyleSheetService
{
    private ProjectService _projectService;

    public StyleSheetService([FromServices] ProjectService projectService)
    {
        _projectService = projectService;
    }
    
    public StyleSheetAnswer GetStyleSheet(string sheetName)
    {
        if (_projectService.Project is null
            || !_projectService.Project.StyleSheets.ContainsKey(sheetName))
            return new StyleSheetAnswer { Ok = false };
        
        return new StyleSheetAnswer
        {
            Ok = true,
            StyleSheetDTO = new StyleSheetDTO(_projectService.Project.StyleSheets[sheetName])
        };
    }

    public StyleSheetWithComponentAnswer CreateStyleComponent(string sheetName, string rootStyleID, StyleManipulation styleManipulation)
    {
        if (_projectService.Project is null
            || !_projectService.Project.StyleSheets.ContainsKey(sheetName))
            return new StyleSheetWithComponentAnswer {Ok = false};

        var styleSheet = _projectService.Project.StyleSheets[sheetName];
        var result = styleSheet.CreateStyleComponent(rootStyleID, styleManipulation);

        if (result.Item1)
            return new StyleSheetWithComponentAnswer
            {
                Ok = true,
                StyleSheetDTO = new StyleSheetDTO(styleSheet),
                StyleComponentDTO = new StyleComponentDTO(styleSheet.StyleComponentsDict[result.Item2!])
            };
        return new StyleSheetWithComponentAnswer {Ok = false};
    }
    
    
    public StyleComponentAnswer GetComponentStyles(string sheetName, string styleID)
    {
        if (_projectService.Project is null
            || !_projectService.Project.StyleSheets.ContainsKey(sheetName))
            return new StyleComponentAnswer { Ok = false };
        var stylesheet = _projectService.Project.StyleSheets[sheetName];
        if (!stylesheet.StyleComponentsDict.ContainsKey(styleID))
        {
            stylesheet.StyleComponentsDict[styleID] = new StyleComponent();
            stylesheet.StyleComponents.Add(stylesheet.StyleComponentsDict[styleID]);
        }

        var styleComponent = stylesheet.StyleComponentsDict[styleID];
        return new StyleComponentAnswer {Ok = true, StyleComponentDTO = new StyleComponentDTO(styleComponent)};
    }

    public StyleComponentAnswer UpdateComponentStyles(string sheetName, string styleID, 
        string selector, Dictionary<string, string> styles)
    {
        if (_projectService.Project is null
            || !_projectService.Project.StyleSheets.ContainsKey(sheetName))
            return new StyleComponentAnswer { Ok = false };
        var stylesheet = _projectService.Project.StyleSheets[sheetName];
        if (!stylesheet.StyleComponentsDict.ContainsKey(styleID))
        {
            stylesheet.StyleComponentsDict[styleID] = new StyleComponent();
            stylesheet.StyleComponents.Add(stylesheet.StyleComponentsDict[styleID]);
        }

        var styleComponent = stylesheet.StyleComponentsDict[styleID];
        styleComponent.Selector = selector;
        styleComponent.Styles = styles;
        return new StyleComponentAnswer {Ok = true, StyleComponentDTO = new StyleComponentDTO(styleComponent)};
    }

    public StyleSheetAnswer RemoveComponentStyles(string sheetName, string styleID)
    {
        if (_projectService.Project is null
            || !_projectService.Project.StyleSheets.ContainsKey(sheetName))
            return new StyleSheetAnswer { Ok = false };
        
        var stylesheet = _projectService.Project.StyleSheets[sheetName];
        if (stylesheet.StyleComponentsDict.ContainsKey(styleID))
        {
            var styleComponent = stylesheet.StyleComponentsDict[styleID];
            stylesheet.StyleComponentsDict.Remove(styleID);
            stylesheet.StyleComponents.Remove(styleComponent);
        }
        
        return new StyleSheetAnswer
        {
            Ok = true,
            StyleSheetDTO = new StyleSheetDTO(stylesheet)
        };
    }
}
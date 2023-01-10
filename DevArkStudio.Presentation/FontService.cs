#nullable enable
using System.Linq;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence;
using DevArkStudio.Persistence.DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;

public class FontAPIResponse
{
    public bool Ok { get; init; }
    public string? Error { get; init; }
}

public class FontListResponse : FontAPIResponse
{
    public string[]? Fonts { get; init; }
}

public class FontConnectedListResponse : FontListResponse
{
    public string[]? ConnectedFonts { get; init; }
}

public class FontCreatedResponse : FontListResponse
{
    public string? FontName { get; init; }
}

public class FontService
{
    private readonly ProjectService _projectService;
    private readonly FontFileManagerService _fontFileManagerService;
    
    public FontService([FromServices] ProjectService projectService, 
        [FromServices] FontFileManagerService fontFileManagerService)
    {
        _projectService = projectService;
        _fontFileManagerService = fontFileManagerService;
    }

    public FontListResponse GetAllFonts()
    {
        if (_projectService.Project is null)
            return new FontListResponse {Ok = false, Error = "Ни один проект не открыт"};
        return new FontListResponse
        {
            Ok = true,
            Fonts = _projectService.Project.FontItems.Keys.ToArray()
        };
    }

    public FontConnectedListResponse GetConnectedFontsList(string pageName)
    {
        if (_projectService.Project is null)
            return new FontConnectedListResponse {Ok = false, Error = "Ни один проект не открыт"};
        if (!_projectService.Project.Pages.ContainsKey(pageName))
            return new FontConnectedListResponse {Ok = false, Error = "Подобных страниц не найдено"};

        return new FontConnectedListResponse
        {
            Ok = true,
            Fonts = _projectService.Project.FontItems.Keys.ToArray(),
            ConnectedFonts = _projectService.Project.Pages[pageName].Fonts.ToArray()
        };
    }

    public FontConnectedListResponse ConnectFont(string pageName, string fontName, bool connect)
    {
        if (_projectService.Project is null)
            return new FontConnectedListResponse {Ok = false, Error = "Ни один проект не открыт"};
        if (!_projectService.Project.Pages.ContainsKey(pageName))
            return new FontConnectedListResponse {Ok = false, Error = "Подобных страниц не найдено"};
        if (!_projectService.Project.FontItems.ContainsKey(fontName))
            return new FontConnectedListResponse {Ok = false, Error = "Подобных шрифтов не найдено"};
        if (!connect)
        {
            _projectService.Project.Pages[pageName].Fonts.Remove(fontName);
        }
        else if (!_projectService.Project.Pages[pageName].Fonts.Contains(fontName))
        {
            _projectService.Project.Pages[pageName].Fonts.Add(fontName);
        }
        
        return new FontConnectedListResponse
        {
            Ok = true,
            Fonts = _projectService.Project.FontItems.Keys.ToArray(),
            ConnectedFonts = _projectService.Project.Pages[pageName].Fonts.ToArray()
        };
    }

    public FontCreatedResponse UploadFont(string fontName, string fontStyle, string fontWeight, IFormFile fontFiles)
    {
        var fileName = $"{fontName};{fontStyle};{fontWeight}";
        if (fontFiles.ContentType != "application/zip")
            return new FontCreatedResponse {Ok = false, Error = "Формат загружаемого файла должен быть zip"};
        if (_projectService.Project is null)
            return new FontCreatedResponse {Ok = false, Error = "Ни один проект не открыт"};
        if (_projectService.Project.FontItems.ContainsKey(fileName)) 
            return new FontCreatedResponse {Ok = false, Error = "Такой шрифт уже был загружен"};
        var fontDTO = new FontDTO
        {
            FileName = fileName,
            FontFamily = fontName,
            FontStyle = fontStyle,
            FontWeight = fontWeight
        };
        var res = _fontFileManagerService.CreateFont(_projectService.Project.Name, fontFiles.OpenReadStream(), fontDTO);
        if (!res.Item1) return new FontCreatedResponse {Ok = false, Error = res.Item2};
        _projectService.Project.FontItems[fileName] = new FontItem
        {
            FontFamily = fontName,
            FontStyle = fontStyle,
            FontWeight = fontWeight,
                        
            EotFontPath = res.Item3!.Contains(".eot") ? $"/fonts/{fileName}/{fileName}.eot" : "",
            Woff2FontPath = res.Item3!.Contains(".woff2") ? $"/fonts/{fileName}/{fileName}.woff2" : "",
            WoffFontPath = res.Item3!.Contains(".woff") ? $"/fonts/{fileName}/{fileName}.woff" : "",
            TTFFontPath = res.Item3!.Contains(".ttf") ? $"/fonts/{fileName}/{fileName}.ttf" : "",
            SVGFontPath = res.Item3!.Contains(".svg") ? $"/fonts/{fileName}/{fileName}.svg" : ""
        };

        return new FontCreatedResponse
        {
            Ok = true,
            Fonts = _projectService.Project.FontItems.Keys.ToArray(),
            FontName = fileName
        };
    }
}
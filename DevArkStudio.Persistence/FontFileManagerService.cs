#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence.DTOModels;
using Exception = System.Exception;

namespace DevArkStudio.Persistence;

public class FontFileManagerService
{
    private readonly DirectoryInfo _projectContDirectory = new(Path.Combine(Directory.GetCurrentDirectory(), "./projects/"));
    private readonly HashSet<string> _fontExtensions = new() { ".eot", ".woff2", ".woff", ".ttf", ".svg" };

    public FontFileManagerService()
    {
        CheckContDirectory();
    }

    private void CheckContDirectory()
    {
        if (!_projectContDirectory.Exists) _projectContDirectory.Create();
    }

    private DirectoryInfo GetProjectDirectory(string projectName)
    {
        return new DirectoryInfo(Path.Combine(_projectContDirectory.FullName, projectName));
    }

    public (bool, FontItem[]?, string?) LoadFontItems(string projectName)
    {
        var projectSrcDir = GetProjectDirectory(projectName);
        if (!projectSrcDir.Exists) return (false, null, $"Проект {projectName} не существует");
        var projectFontsDir = new DirectoryInfo(Path.Combine(projectSrcDir.FullName, "fonts"));
        if (!projectFontsDir.Exists) return (true, new FontItem[] { }, null);
        var fontsDirs = projectFontsDir.GetDirectories();
        var fontItems = fontsDirs
            .Select(dir =>
            {
                var fontInfo = new FileInfo(Path.Combine(dir.FullName, "conf.json"));
                var fontFiles = dir.GetFiles().Where(file => _fontExtensions.Contains(file.Extension)).ToArray();

                if (fontFiles.Length == 0 || !fontInfo.Exists)
                {
                    dir.Delete(true);
                    return null;
                }
                
                var fontInfoStream = fontInfo.OpenRead();

                try
                {
                    var fontInfoDTO = JsonSerializer.Deserialize<FontDTO>(fontInfoStream);
                    fontInfoStream.Close();
                    if (fontInfoDTO is null) throw new Exception("Invalid JSON File");
                    
                    RenameFonts(fontFiles, fontInfoDTO, dir);
                    
                    return new FontItem
                    {
                        FontFamily = fontInfoDTO.FontFamily,
                        FontStyle = fontInfoDTO.FontStyle,
                        FontWeight = fontInfoDTO.FontWeight,
                        
                        EotFontPath = File.Exists(Path.Combine(Path.Combine(dir.FullName, fontInfoDTO.FileName + ".eot"))) 
                            ? $"/fonts/{dir.Name}/{fontInfoDTO.FileName}.eot" : "",
                        Woff2FontPath = File.Exists(Path.Combine(Path.Combine(dir.FullName, fontInfoDTO.FileName + ".woff2"))) 
                            ? $"/fonts/{dir.Name}/{fontInfoDTO.FileName}.woff2" : "",
                        WoffFontPath = File.Exists(Path.Combine(Path.Combine(dir.FullName, fontInfoDTO.FileName + ".woff"))) 
                            ? $"/fonts/{dir.Name}/{fontInfoDTO.FileName}.woff" : "",
                        TTFFontPath = File.Exists(Path.Combine(Path.Combine(dir.FullName, fontInfoDTO.FileName + ".ttf"))) 
                            ? $"/fonts/{dir.Name}/{fontInfoDTO.FileName}.ttf" : "",
                        SVGFontPath = File.Exists(Path.Combine(Path.Combine(dir.FullName, fontInfoDTO.FileName + ".svg"))) 
                            ? $"/fonts/{dir.Name}/{fontInfoDTO.FileName}.svg" : ""
                    };
                }
                catch (Exception e)
                {
                    fontInfoStream.Close();
                    fontInfo.Delete();
                    dir.Delete(true);

                    Console.WriteLine(e);
                }

                return null;
            })
            .OfType<FontItem>()
            .ToArray();
        return (true, fontItems, null);
    }

    public (bool, string?, HashSet<string>?) CreateFont(string projectName, Stream fontArchive, FontDTO fontDTO)
    {
        var projectSrcDir = GetProjectDirectory(projectName);
        if (!projectSrcDir.Exists) return (false, $"Проект {projectName} не существует", null);
        var projectFontsDir = new DirectoryInfo(Path.Combine(projectSrcDir.FullName, "fonts"));
        if (!projectFontsDir.Exists) projectFontsDir.Create();
        var curFontDir = new DirectoryInfo(Path.Combine(projectFontsDir.FullName, fontDTO.FileName));
        if (!curFontDir.Exists) curFontDir.Create();
        try
        {
            var zip = new ZipArchive(fontArchive, ZipArchiveMode.Read);
            var curFontConfFile = new FileInfo(Path.Combine(curFontDir.FullName, "conf.json"));
            zip.ExtractToDirectory(curFontDir.FullName, true);
            
            var fontFiles = curFontDir
                .GetFiles()
                .Where(file => _fontExtensions.Contains(file.Extension))
                .ToArray();

            if (fontFiles.Length == 0)
            {
                curFontDir.Delete(true);
                return (false, "В архиве нет шрифтов", null);
            }
            
            RenameFonts(fontFiles, fontDTO, curFontDir);
            
            var fileStream = new FileStream(curFontConfFile.FullName, FileMode.Create);
            fileStream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(fontDTO)));
            fileStream.Close();
            
            return (true, null, fontFiles.Select(file => file.Extension).ToHashSet());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return e switch
            {
                PathTooLongException => (false, "Слишком длинный путь", null),
                DirectoryNotFoundException => (false, "Директория распаковки не найдена", null),
                IOException => (false, "Ошибка ввода/вывода", null),
                UnauthorizedAccessException => (false, "Доступ к каталогу/архиву запрещен", null),
                InvalidDataException => (false, "Архив повреждён", null),
                _ => (false, "Неизвестная ошибка", null)
            };
        }
    }

    private static void RenameFonts(IEnumerable<FileInfo> fontFiles, FontDTO fontInfoDTO, DirectoryInfo dir)
    {
        foreach (var file in fontFiles)
        {
            var fileNameWithExtension = fontInfoDTO.FileName + file.Extension;
            var path = Path.Combine(dir.FullName, fileNameWithExtension);
            if (file.Name != fileNameWithExtension) file.MoveTo(path, true);
        }
    }
}
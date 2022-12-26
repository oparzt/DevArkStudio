#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence;

public class ProjectLoaderService
{
    private readonly DirectoryInfo _projectContDirectory = new(Path.Combine(Directory.GetCurrentDirectory(), "./projects/"));
    private DirectoryInfo? _projectSrcDirectory;

    public ProjectLoaderService()
    {
        CheckContDirectory();
    }

    private void CheckContDirectory()
    {
        if (!_projectContDirectory.Exists) _projectContDirectory.Create();
    }

    public string[] GetSavedProjectsNames()
    {
        CheckContDirectory();
        return _projectContDirectory.GetDirectories().Select(dir => dir.Name).ToArray();
    }

    public (bool, string?) CreateProject(string projectName)
    {
        var projectSrcDir = new DirectoryInfo(Path.Combine(_projectContDirectory.FullName, projectName));
        if (projectSrcDir.Exists) return (false, $"Проект {projectName} уже существует");
        projectSrcDir.Create();
        var htmlPage = new HTMLPage(new HTMLHead(), new HTMLElement("body"));
        var html = htmlPage.RenderPage(false);

        var fileStream = new FileStream(Path.Combine(projectSrcDir.FullName, "index.html"), FileMode.Create);
        fileStream.Write(Encoding.UTF8.GetBytes(html));
        fileStream.Close();
        var fileStream2 = new FileStream(Path.Combine(projectSrcDir.FullName, "index.css"), FileMode.Create);
        fileStream2.Write(Encoding.UTF8.GetBytes(""));
        fileStream2.Close();
        return (true, null);
    }

    public (bool, Project?, string?) LoadProject(string projectName)
    {
        var projectSrcDir = new DirectoryInfo(Path.Combine(_projectContDirectory.FullName, projectName));
        if (!projectSrcDir.Exists) return (false, null, $"Проект {projectName} не существует");
        _projectSrcDirectory = projectSrcDir;
        
        var project = new Project(projectName);
        var cssFiles = new List<FileInfo>();
        var htmlFiles = new List<FileInfo>();
        SearchFilesInDir(projectSrcDir, "*.css", cssFiles);
        SearchFilesInDir(projectSrcDir, "*.html", htmlFiles);

        foreach (var cssFile in cssFiles)
        {
            project.AddNewStyleSheet(new StyleSheet(cssFile),
                Path.GetRelativePath(_projectSrcDirectory.FullName, cssFile.FullName));
        }

        foreach (var htmlFile in htmlFiles)
        {
            project.AddNewPage(new HTMLPage(htmlFile, project), 
                Path.GetRelativePath( _projectSrcDirectory.FullName, htmlFile.FullName));
        }

        return (true, project, null);
    }
    
    public bool SaveProject(Project project)
    {
        if (_projectSrcDirectory is null || !_projectSrcDirectory.Exists) return false;

        foreach (var page in project.Pages)
        {
            var fileStream = new FileStream(Path.Combine(_projectSrcDirectory.FullName, page.Key), FileMode.Create);
            fileStream.Write(Encoding.UTF8.GetBytes(page.Value.RenderPage(false)));
            fileStream.Close();
        }
        
        foreach (var stylesheet in project.StyleSheets)
        {
            var fileStream = new FileStream(Path.Combine(_projectSrcDirectory.FullName, stylesheet.Key), FileMode.Create);
            fileStream.Write(Encoding.UTF8.GetBytes(stylesheet.Value.RenderStyleSheet(false)));
            fileStream.Close();
        }

        return true;
    }

    public (bool, FileStream?) GetFileFromProject(string filePath)
    {
        if (_projectSrcDirectory is null || !_projectSrcDirectory.Exists) return (false, null);
        var fileInfo = new FileInfo(Path.Combine(_projectSrcDirectory.FullName, filePath));
        return !fileInfo.Exists ? (false, null) : (true, new FileStream(fileInfo.FullName, FileMode.Open));
    }

    public (bool, byte[]?) GetAllProject()
    {
        if (_projectSrcDirectory is null || !_projectSrcDirectory.Exists) return (false, null);
        var zipFile = new FileInfo(Path.Combine(_projectContDirectory.FullName, _projectSrcDirectory.Name.Trim('/') + ".zip"));
        ZipFile.CreateFromDirectory(_projectSrcDirectory.FullName, zipFile.FullName);
        var finalResult = File.ReadAllBytes(zipFile.FullName);
        if (zipFile.Exists)
        {
            zipFile.Delete();
        }

        if (finalResult is null || !finalResult.Any())
        {
            return (false, null);
        }

        return (true, finalResult);
    }
    
    public bool UnloadProject()
    {
        _projectSrcDirectory = null;
        return true;
    }

    private static void SearchFilesInDir(DirectoryInfo root, string searchPattern, List<FileInfo> foundFiles)
    {
        FileInfo[]? files = null;

        try
        {
            files = root.GetFiles(searchPattern);
        }
        catch (UnauthorizedAccessException) {}
        catch (DirectoryNotFoundException) {}
        catch (Exception) { /* ignored */ }

        if (files == null) return;

        foundFiles.AddRange(files);

        var subDirs = root.GetDirectories();
        foreach (var dirInfo in subDirs) SearchFilesInDir(dirInfo, searchPattern, foundFiles);
    }
}


/*

public bool LoadProjectSrcDirectory(string path)
{
    if (Path.HasExtension(path)) path = Path.GetDirectoryName(path) ?? string.Empty;
    var projectSrcDirectory = new DirectoryInfo(path);
    var relativeToSrcDirectory = Path.GetRelativePath(_wwwrootPath.FullName, path);
    if (!projectSrcDirectory.Exists || !relativeToSrcDirectory.Contains("..")) return false;
    _projectSrcDirectory = projectSrcDirectory;
    return true;
}

public bool RenderProject()
{
    if (_projectSrcDirectory is null || !_projectSrcDirectory.Exists) return false;
    ClearProjectDestDirectory();
    CopyDirectory(_projectSrcDirectory.FullName, _projectDestDirectory.FullName, true);
    return true;
}


private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
{
    var dir = new DirectoryInfo(sourceDir);
    if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
    var dirs = dir.GetDirectories();

    Directory.CreateDirectory(destinationDir);

    foreach (var file in dir.GetFiles())
    {
        var targetFilePath = Path.Combine(destinationDir, file.Name);
        file.CopyTo(targetFilePath);
    }

    if (!recursive) return;
    
    foreach (var subDir in dirs)
    {
        var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, true);
    }
}
*/


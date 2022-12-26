using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class ProjectDTO
{ 
    public string Name { get; init; }
    
    public string[] PagePaths { get; init; }
    public string[] StyleSheetPaths { get; init; }
    
    public ProjectDTO() {}

    public ProjectDTO(Project project, string projectName)
    {
        Name = projectName;
        PagePaths = project.Pages.Keys.ToArray();
        StyleSheetPaths = project.StyleSheets.Keys.ToArray();
    }
}
using System.Collections.Generic;
using System.Linq;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class StyleSheetDTO
{
    public string StyleSheetPath { get; init; }
    public List<StyleComponentDTO> StyleComponents { get; init; }
    
    public StyleSheetDTO() {}

    public StyleSheetDTO(StyleSheet styleSheet)
    {
        StyleSheetPath = styleSheet.StyleSheetPath;
        StyleComponents = styleSheet.StyleComponents.Select(component => new StyleComponentDTO(component)).ToList();
    }
}
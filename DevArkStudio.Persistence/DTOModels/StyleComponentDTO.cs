using System.Collections.Generic;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class StyleComponentDTO
{
    public string StyleID { get; init; }
    public string Selector { get; init; } 
    public Dictionary<string, string> Styles { get; init; }
    
    public StyleComponentDTO() {}

    public StyleComponentDTO(StyleComponent styleComponent)
    {
        StyleID = styleComponent.StyleID;
        Selector = styleComponent.Selector;
        Styles = styleComponent.Styles;
    }
}
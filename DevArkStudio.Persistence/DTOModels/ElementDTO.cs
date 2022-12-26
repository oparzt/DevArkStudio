using System.Collections.Generic;
using System.Linq;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class ElementDTO : NodeDTO
{
    public List<string> ClassList { get; init; } = new();
    public string StyleID { get; init; } = "";
    public string TagName { get; init; } = "";
    public Dictionary<string, string> Attributes { get; init; } = new();

    public ElementDTO() {}

    public ElementDTO(IElement elm)
    {
        NodeID = elm.NodeID;
        ChildNodes = elm.ChildNodes.Select(innerElm => innerElm.NodeID).ToList();

        ClassList = elm.ClassList.ToList();
        StyleID = elm.StyleID;
        TagName = elm.TagName;
        Attributes = elm.Attributes;
    }

    public IElement CreateElement()
    {
        return new HTMLElement(TagName, NodeID)
        {
            Attributes = Attributes,
            StyleID = StyleID,
            ClassList = ClassList,
        };
    }
}
using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class TextDTO : NodeDTO
{
    public string TextContent { get; init; } = "";
    
    public TextDTO() {}

    public TextDTO(IText textNode)
    {
        ChildNodes = new List<string>();
        NodeID = textNode.NodeID;
        TextContent = textNode.TextContent;
    }

    public IText CreateTextNode()
    {
        return new TextNode(TextContent, NodeID);
    }
}
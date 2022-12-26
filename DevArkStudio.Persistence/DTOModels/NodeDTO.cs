using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Persistence.DTOModels;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "nodeType")]
[JsonDerivedType(typeof(ElementDTO), (int)NodeType.ELEMENT_NODE)]
[JsonDerivedType(typeof(TextDTO), (int)NodeType.TEXT_NODE)]
public class NodeDTO
{
    public List<string> ChildNodes { get; init; } = new();
    public string NodeID { get; init; }

    public NodeDTO() {}

    public NodeDTO(INode node)
    {
        NodeID = node.NodeID;
        ChildNodes = node.ChildNodes.Select(child => child.NodeID).ToList();
    }
}
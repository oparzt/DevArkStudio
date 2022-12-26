using System.Collections.Generic;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Domain.Interfaces
{
    public enum NodeType
    {
        SAMPLE_NODE = 0,
        ELEMENT_NODE = 1,
        TEXT_NODE = 2
    }
    
    [JsonDerivedType(typeof(HTMLElement), (int)Interfaces.NodeType.ELEMENT_NODE)]
    [JsonDerivedType(typeof(TextNode), (int)Interfaces.NodeType.TEXT_NODE)]
    public interface INode
    {
        NodeType NodeType { get; }
        List<INode> ChildNodes { get; }
        INode ParentNode { get; set; }
        
        string NodeID { get; }
        string NodeName { get; }
        string NodeValue { get; set; }
        string TextContent { get; set; }

        bool Append(INode node);
        bool Prepend(INode node);
        bool Before(INode node);
        bool After(INode node);
    }
}
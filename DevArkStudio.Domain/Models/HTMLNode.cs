using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models;

public class HTMLNode : INode
{
    public NodeType NodeType { get; protected init; } = NodeType.SAMPLE_NODE;
    public List<INode> ChildNodes { get; } = new();
    public INode ParentNode { get; set; }

    public string NodeID { get; protected init; } = "";
    public string NodeName { get; protected init; } = "#sample-node";
    public string NodeValue { get; set; } = "";
    public string TextContent { get; set; }
    
    public bool Append(INode node)
    {
        if (this is not IElement) return false;
        
        ChildNodes.Add(node);
        node.ParentNode?.ChildNodes.Remove(this);
        node.ParentNode = this;
        
        return true;
    }

    public bool Prepend(INode node)
    {
        if (this is not IElement) return false;

        ChildNodes.Insert(0, node);
        node.ParentNode?.ChildNodes.Remove(this);
        node.ParentNode = this;
        
        return true;
    }

    public bool Before(INode node)
    {
        if (ParentNode is null) return false;

        var pos = ParentNode.ChildNodes.IndexOf(this);
        ParentNode.ChildNodes.Insert(pos, node);
        node.ParentNode?.ChildNodes.Remove(this);
        node.ParentNode = ParentNode;
        
        return true;
    }

    public bool After(INode node)
    {
        if (ParentNode is null) return false;

        var pos = ParentNode.ChildNodes.IndexOf(this);
        ParentNode.ChildNodes.Insert(pos + 1, node);
        node.ParentNode?.ChildNodes.Remove(this);
        node.ParentNode = ParentNode;
        
        return true;
    }
}
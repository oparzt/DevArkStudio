using System.Collections.Generic;

namespace DevArkStudio.Domain.Interfaces
{
    public enum NodeType
    {
        ELEMENT_NODE = 1,
        ATTRIBUTE_NODE = 2,
        TEXT_NODE = 3,
        CDATA_SECTION_NODE = 4,
        PROCESSING_INSTRUCTION_NODE = 7,
        COMMENT_NODE = 8,
        DOCUMENT_NODE = 9,
        DOCUMENT_TYPE_NODE = 10,
        DOCUMENT_FRAGMENT_NODE = 11
    }
    
    public interface INode
    {
        NodeType NodeType { get; }
        List<INode> ChildNodes { get; }
        INode ParentNode { get; }
        
        string NodeName { get; }
        string NodeValue { get; set; }
        string TextContent { get; set; }
    }
}
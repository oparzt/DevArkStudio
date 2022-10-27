using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models
{
    public class TextNode : IText
    {
        public NodeType NodeType => NodeType.TEXT_NODE;
        public List<INode> ChildNodes => new ();
        public INode ParentNode { get; set; }
        public string NodeID { get; }
        public string NodeName => "#text";
        public string NodeValue
        {
            get => TextContent;
            set => TextContent = value;
        }

        public string TextContent { get; set; }

        public TextNode(string content = "", string nodeID = "")
        {
            TextContent = content;
            NodeID = nodeID;
        }
    }
}
using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models
{
    public class Text : IText
    {
        public NodeType NodeType { get; }
        public List<INode> ChildNodes { get; }
        public INode ParentNode { get; set; }
        public string NodeName { get; }
        public string NodeValue { get; set; }
        public string TextContent { get; set; }
    }
}
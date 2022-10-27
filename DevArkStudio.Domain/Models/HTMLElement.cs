using System.Collections.Generic;
using System.Linq;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models
{
    public class HTMLElement : IElement
    {
        public NodeType NodeType => NodeType.ELEMENT_NODE;
        public List<INode> ChildNodes { get; } = new();
        public INode ParentNode { get; set; }
        public string NodeName => TagName;
        public string NodeValue
        {
            get => null;
            set {}
        }

        public string TextContent
        {
            get => string.Join(" ", ChildNodes.Select(node => node.TextContent).Where(text => text.Length > 0));
            set
            {
                var text = new TextNode(value);
                ChildNodes.Clear();
                ChildNodes.Add(text);
            }
        }

        public HashSet<string> ClassList { get; private set; } = new();

        public string ClassName
        {
            get => string.Join(" ", ClassList);
            set => ClassList = new HashSet<string>(value.Split(' '));
        }

        public string StyleID { get; set; }
        public string NodeID { get; }
        
        public string InnerHTML => string.Join("\n", ChildNodes.Select(node => node is not HTMLElement element ? node.TextContent : element.OuterHTML));
        public string OuterHTML => "<" + TagName + ">\n" + InnerHTML + "\n</" + TagName + ">";
        
        public string TagName { get; private set; }
        public Dictionary<string, string> Attributes { get; }

        public HTMLElement(string tagName, string nodeID = "")
        {
            TagName = tagName;
            NodeID = nodeID;
        }
        
        public IElement CloneNode()
        {
            throw new System.NotImplementedException();
        }

        public bool AddClass(string className) => ClassList.Add(className);
        public bool RemoveClass(string className) => ClassList.Remove(className);

        public bool Append(INode node)
        {
            ChildNodes.Add(node);
            node.ParentNode?.ChildNodes.Remove(this);
            node.ParentNode = this;
            return true;
        }

        public bool Prepend(INode node)
        {
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

        public string GetAttribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public bool SetAttribute(string name, string value)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveAttribute(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
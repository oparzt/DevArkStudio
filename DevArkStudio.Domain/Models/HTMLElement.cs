using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevArkStudio.Domain.Interfaces;
using HtmlAgilityPack;

namespace DevArkStudio.Domain.Models
{
    public class HTMLElement : HTMLNode, IElement
    {
        private static readonly HashSet<string> VoidElms = new()
        {
            "area", "base", "br", "col", "embed", "hr", "img",
            "input", "link", "meta", "param", "source", "track", "wbr"
        };
        
        public new NodeType NodeType => NodeType.ELEMENT_NODE;

        public new string NodeName => TagName;
        public new string NodeValue
        {
            get => null;
            set {}
        }
        
        public new string TextContent
        {
            get => string.Join(" ", ChildNodes.Select(node => node.TextContent).Where(text => text.Length > 0));
            set
            {
                var text = new TextNode(value);
                ChildNodes.Clear();
                ChildNodes.Add(text);
            }
        }

        public List<string> ClassList { get; set; } = new();

        public string ClassName
        {
            get => string.Join(" ", ClassList);
            set => ClassList = value.Split(' ').ToList();
        }

        public string StyleID { get; set; } = "";

        public string InnerHTML => string.Join("\n", ChildNodes.Select(node => node is not HTMLElement element ? node.TextContent : element.OuterHTML));
        public string OuterHTML => "<" + TagName + ">\n" + InnerHTML + "\n</" + TagName + ">";

        public string TagName { get; private set; }

        public Dictionary<string, string> Attributes { get; init; } = new();

        public HTMLElement(string tagName, string nodeID = "")
        {
            TagName = tagName;
            NodeID = nodeID;
        }

        public IElement CloneNode()
        {
            throw new System.NotImplementedException();
        }

        public void AddClass(string className) => ClassList.Add(className);
        public bool RemoveClass(string className) => ClassList.Remove(className);

        

        public void UpdateAttributes(Dictionary<string, string> attributes)
        {
            Attributes.Clear();
            foreach (var attribute in attributes) Attributes[attribute.Key] = attribute.Value;
        }


        public void RenderInnerHTML(StringBuilder sb, bool develop = true)
        {
            foreach (var node in ChildNodes)
            {
                if (node is not HTMLElement element)
                {
                    sb.Append(node.TextContent);
                }
                else
                {
                    element.RenderOuterHTML(sb, develop);
                }
            }
        }
        
        public void RenderOuterHTML(StringBuilder sb, bool develop = true)
        {
            sb.Append("<");
            sb.Append(TagName);
            foreach (var pair in Attributes)
            {
                sb.Append(" ");
                sb.Append(pair.Key);
                sb.Append("=\"");
                if (develop && 
                    (pair.Key == "href" || pair.Key == "src") &&
                    (pair.Value.StartsWith(".") || pair.Value.StartsWith("/") || pair.Value.Length == 0))
                    sb.Append("/Develop/");
                sb.Append(pair.Value);
                sb.Append("\"");
            }

            if (StyleID.Length > 0)
            {
                sb.Append(" ");
                sb.Append("id=\"");
                sb.Append(StyleID);
                sb.Append("\"");
            }

            if (ClassList.Count > 0)
            {
                sb.Append(" ");
                sb.Append("class=\"");
                foreach (var classToken in ClassList)
                {
                    sb.Append(classToken);
                    sb.Append(" ");
                }
                sb.Append("\"");
            }
            
            sb.Append(">");
            RenderInnerHTML(sb, develop);
            
            
            if (VoidElms.Contains(TagName))
            {
                sb.Append(">");
            }
            else
            {
                sb.Append("</");
                sb.Append(TagName);
                sb.Append(">");
            }
        }
    }

}

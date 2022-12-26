using System.Collections.Generic;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Interfaces;
using HtmlAgilityPack;

namespace DevArkStudio.Domain.Models
{
    public class TextNode : HTMLNode, IText
    {
        public new NodeType NodeType => NodeType.TEXT_NODE;
        public new string NodeValue
        {
            get => TextContent;
            set => TextContent = value;
        }

        public TextNode(string content = "", string nodeID = "") : base()
        {
            TextContent = content;
            NodeID = nodeID;
            NodeName = "#text";
        }
    }
}
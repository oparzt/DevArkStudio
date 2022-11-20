using System;
using System.Linq;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Infrastructure.JSON
{
    public static class Serializer
    {
        public static string SerializeDocument(IDocument document)
        {
            var json = "{\"type\":\"document\",\"doctype\":\"" + document.DocumentType + "\",\"nodes\":[";
            json += String.Join(',', document.AllNodes.Values.Select(node => SerializeNode(node)));
            json += "]}";
            
            return Helper.FormatJson(json);
        }

        public static string SerializeNode(INode node)
        {
            var json = "{\"nodeType\":" + (int)node.NodeType + ",\"nodeId\":\"" + node.NodeID + "\",\"parentNode\":\"";
            json += (node.ParentNode?.NodeID ?? "") + "\",\"childNodes\":[";
            json += String.Join(',', node.ChildNodes.Select(iNode => "\"" + iNode.NodeID + "\""));
            json += "]";

            switch (node)
            {
                case HTMLElement element:
                    json += ",\"tagName\":\"" + element.TagName + "\"";
                    break;
                case TextNode text:
                    json += ",\"text\":\"" + text.TextContent + "\"";
                    break;
            }
            
            json += "}";
            
            return Helper.FormatJson(json);
        }
    }
}
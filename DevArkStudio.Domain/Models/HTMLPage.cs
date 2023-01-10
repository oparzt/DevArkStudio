using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevArkStudio.Domain.Interfaces;
using ExCSS;
using HtmlAgilityPack;

namespace DevArkStudio.Domain.Models
{
    public enum NodeManipulation
    {
        Append = 0,
        Prepend = 1,
        Before = 2,
        After = 3
    }
    
    public class HTMLPage : IHTMLPage
    {
        public string PageName { get; }
        public string DocumentType => "<!DOCTYPE html>";
        public HTMLHead HTMLHead { get; }
        public IElement BodyElement { get; }

        public Dictionary<string, INode> AllNodes { get; init; } = new();

        public List<string> StyleSheets { get; init; } = new();
        public List<string> Fonts { get; init; } = new ();
        public Project? Project { get; init; }

        public HTMLPage(HTMLHead htmlHead, IElement bodyElement, string pageName = "")
        {
            PageName = pageName;

            HTMLHead = htmlHead;
            BodyElement = bodyElement;

            htmlHead.HTMLPage = this;

            AllNodes[BodyElement.NodeID] = BodyElement;

            INode[] children;
            do
            { 
                children = AllNodes
                    .Values
                    .Select(node => node.ChildNodes)
                    .SelectMany(nodeList => nodeList)
                    .Where(node => !AllNodes.ContainsKey(node.NodeID))
                    .ToArray();
                foreach (var node in children) AllNodes[node.NodeID] = node;
            } while (children.Length > 0);
        }

        public HTMLPage(FileSystemInfo htmlFile, Project project, string pageName = "")
        {
            Project = project;
            PageName = pageName;
            var doc = new HtmlDocument();
            doc.Load(htmlFile.FullName);

            HTMLHead = new HTMLHead
            {
                Title = doc.DocumentNode.SelectSingleNode("//head/title")?.InnerText ?? "",
                HTMLPage = this
            };

            var cssParser = new StylesheetParser();
            var stylesheet = cssParser.Parse(string.Join(' ', doc.DocumentNode
                .SelectNodes("//head/style")
                ?.Select(styleNode => styleNode.InnerHtml).ToArray() ?? new string[]{}));
            
            Fonts = stylesheet.FontfaceSetRules
                .Select(fontface => fontface.Family.Trim('"') + ";" + fontface.Style + ";" + fontface.Weight)
                .Where(fontface => project.FontItems.ContainsKey(fontface))
                .ToList();
            
            StyleSheets = doc.DocumentNode
                .SelectNodes("//head/link")
                ?.Where(node => node.Attributes.Contains("rel") 
                               && node.Attributes.Contains("href")
                               && node.Attributes["rel"].Value == "stylesheet")
                .Select(node =>node.Attributes["href"].Value)
                .Select(href =>
                {
                    if (href.Length > 0 && href[0] == '/') return href.Remove(0, 1);
                    return href;
                })
                .Where(href => project.StyleSheets.ContainsKey(href))
                .ToList() ?? new List<string>();

            var bodyElmFromText = doc.DocumentNode.SelectSingleNode("//body");
            var elmsToAdd = new Queue<(INode, HtmlNode)>();
            BodyElement = new HTMLElement("body", GetUIDForNode());

            if (bodyElmFromText is not null) elmsToAdd.Enqueue((BodyElement, bodyElmFromText));
            AllNodes[BodyElement.NodeID] = BodyElement;
            
            while (elmsToAdd.TryDequeue(out var elmPair))
            {
                foreach (var node in elmPair.Item2.ChildNodes)
                {
                    INode? newNode;
                    switch (node.NodeType)
                    { 
                        case HtmlNodeType.Element:
                            var attributes = new Dictionary<string, string>();
                            foreach (var attr in node.Attributes) attributes[attr.Name] = attr.Value;
                            
                            newNode = new HTMLElement(node.OriginalName, GetUIDForNode())
                            {
                                Attributes = attributes, 
                                ClassName = attributes.ContainsKey("class") ? attributes["class"] : ""
                            };
                            break;
                        case HtmlNodeType.Text:
                            newNode = new TextNode(node.InnerText, GetUIDForNode());
                            break;
                        case HtmlNodeType.Document:
                        case HtmlNodeType.Comment:
                        default:
                            newNode = null;
                            break;
                    }

                    if (newNode is null) continue;
                    
                    (elmPair.Item1 as HTMLElement)?.Append(newNode);
                    AllNodes[newNode.NodeID] = newNode;
                    elmsToAdd.Enqueue((newNode, node));
                }
            }
        }

        public string RenderPage(bool develop = true)
        {
            var sb = new StringBuilder();
            sb.Append(DocumentType);
            sb.Append("\n<html>");
            HTMLHead.RenderOuterHTML(sb, develop);
            BodyElement.RenderOuterHTML(sb, develop);
            sb.Append("</html>");
            var html = sb.ToString();

            return html;
        }

        public void RemoveNode(string nodeID)
        {
            if (!AllNodes.ContainsKey(nodeID)) return;
            var targetNode = AllNodes[nodeID];
            targetNode.ParentNode?.ChildNodes.Remove(targetNode);
            AllNodes.Remove(nodeID);
        }

        public bool TransferNode(string rootNodeID, string targetNodeID, NodeManipulation nodeManipulation)
        {
            if (!AllNodes.ContainsKey(targetNodeID) 
                || !AllNodes.ContainsKey(rootNodeID)) return false;
            return MakeNodeManipulation(AllNodes[rootNodeID], AllNodes[targetNodeID], nodeManipulation);
        }

        public (bool, string?) CreateTextNode(string rootNodeID, NodeManipulation nodeManipulation)
        {
            if (!AllNodes.ContainsKey(rootNodeID)) return (false, null);
            var rootNode = AllNodes[rootNodeID];
            var targetNode = new TextNode("", GetUIDForNode());
            var manipulationResult = MakeNodeManipulation(rootNode, targetNode, nodeManipulation);
            
            if (!manipulationResult) return (false, null);
            
            AllNodes[targetNode.NodeID] = targetNode;
            return (true, targetNode.NodeID);
        }

        public (bool, string?) CreateElementNode(string rootNodeID, string targetNodeTag, NodeManipulation nodeManipulation)
        {
            if (!AllNodes.ContainsKey(rootNodeID) || targetNodeTag.Length == 0) return (false, null);
            var rootNode = AllNodes[rootNodeID];
            var targetNode = new HTMLElement(targetNodeTag, GetUIDForNode());
            var manipulationResult = MakeNodeManipulation(rootNode, targetNode, nodeManipulation);
            
            if (!manipulationResult) return (false, null);
            
            AllNodes[targetNode.NodeID] = targetNode;
            return (true, targetNode.NodeID);
        }

        private string GetUIDForNode()
        {
            string uid;
            do { uid = Guid.NewGuid().ToString(); } while (AllNodes.ContainsKey(uid));
            return uid;
        }

        private static bool MakeNodeManipulation(INode rootNode, INode targetNode, NodeManipulation nodeManipulation)
        {
            return nodeManipulation switch
            {
                NodeManipulation.Append => rootNode.Append(targetNode),
                NodeManipulation.Prepend => rootNode.Prepend(targetNode),
                NodeManipulation.Before => rootNode.Before(targetNode),
                NodeManipulation.After => rootNode.After(targetNode),
                _ => false
            };
        }
    }
}

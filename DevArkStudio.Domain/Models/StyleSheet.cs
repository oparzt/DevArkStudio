using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExCSS;

namespace DevArkStudio.Domain.Models;

public enum StyleManipulation
{
    DocumentStart = 0,
    AfterComponent = 1
}

public class StyleSheet
{
    public string StyleSheetPath { get; init; } = "";
    
    // public List<StyleSheet> InnerStyleSheets { get; set; } = new();
    public List<StyleComponent> StyleComponents { get; set; } = new();
    public Dictionary<string, StyleComponent> StyleComponentsDict { get; set; } = new();

    public StyleSheet() {}

    public StyleSheet(FileInfo fileInfo)
    {
        var cssParser = new StylesheetParser();
        var stylesheet = cssParser.Parse(new StreamReader(fileInfo.FullName).ReadToEnd());

        foreach (var styleRule in stylesheet.StyleRules)
        {
            var styleComponent = new StyleComponent()
            {
                StyleID = GetUIDForStyleComponent(),
                Selector = styleRule.SelectorText,
                Styles = styleRule.Style.ToDictionary(rule => rule.Name, rule => rule.Value)
            };
            StyleComponents.Add(styleComponent);
            StyleComponentsDict[styleComponent.StyleID] = styleComponent;
        }
    }

    private string GetUIDForStyleComponent()
    {
        string uid;
        do { uid = Guid.NewGuid().ToString();} while (StyleComponentsDict.ContainsKey(uid));
        return uid;
    }
    
    public string RenderStyleSheet(bool develop)
    {
        var sb = new StringBuilder();
        RenderStyleSheet(sb, develop);
        return sb.ToString();
    }

    public void RenderStyleSheet(StringBuilder sb, bool develop)
    {
        // foreach (var styleSheet in InnerStyleSheets)
        //     styleSheet.RenderStyleSheet(sb, develop);

        foreach (var styleComponent in StyleComponents)
            styleComponent.RenderStyleComponent(sb, develop);
    }

    public void RenderStyleSheetConnect(StringBuilder sb, bool develop)
    {
        sb.Append("<link rel=\"stylesheet\" href=\"");
        sb.Append(develop ? "/Develop/" : "/");
        sb.Append(StyleSheetPath);
        sb.Append("\">");
    }

    public (bool, string?) CreateStyleComponent(string rootStyleID, StyleManipulation styleManipulation)
    {
        var styleComponent =  new StyleComponent
        {
            StyleID = GetUIDForStyleComponent(),
            Selector = "",
            Styles = new Dictionary<string, string>()
        };
        
        switch (styleManipulation)
        {
            case StyleManipulation.DocumentStart:
                StyleComponents.Insert(0, styleComponent);
                break;
            case StyleManipulation.AfterComponent 
                 when StyleComponentsDict.ContainsKey(rootStyleID):
                StyleComponents.Insert(StyleComponents.FindIndex(component => component.StyleID == rootStyleID), styleComponent);
                break;
            default:
                return (false, null);
        }

        StyleComponentsDict[styleComponent.StyleID] = styleComponent;
        return (true, styleComponent.StyleID);
    }
}
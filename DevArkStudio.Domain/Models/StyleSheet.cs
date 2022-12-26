using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExCSS;

namespace DevArkStudio.Domain.Models;

public class StyleSheet
{
    public string StyleSheetPath { get; set; }
    
    public List<StyleSheet> InnerStyleSheets { get; set; } = new();
    public List<StyleComponent> StyleComponents { get; set; } = new();
    public Dictionary<string, StyleComponent> StyleComponentsDict { get; set; } = new();


    public StyleSheet()
    {
        
    }

    public StyleSheet(FileInfo fileInfo)
    {
        var cssParser = new StylesheetParser();
        var stylesheet = cssParser.Parse(new StreamReader(fileInfo.FullName).ReadToEnd());

        foreach (var styleRule in stylesheet.StyleRules)
        {
            var styleComponent = new StyleComponent()
            {
                Selector = styleRule.SelectorText,
                Styles = styleRule.Style.ToDictionary(rule => rule.Original, rule => rule.Value)
            };
            StyleComponents.Add(styleComponent);
            StyleComponentsDict[styleRule.SelectorText] = styleComponent;
        }
    }

    private string GetUIDForStyleComponent()
    {
        var uid = "";
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
        foreach (var styleSheet in InnerStyleSheets)
            styleSheet.RenderStyleSheet(sb, develop);

        foreach (var styleComponent in StyleComponents)
            styleComponent.RenderStyleComponent(sb, develop);
    }

    public void RenderStyleSheetConnect(StringBuilder sb, bool develop)
    {
        sb.Append("<link rel=\"stylesheet\" href=\"");
        sb.Append(develop ? "/Develop/" : "");
        sb.Append(StyleSheetPath);
        sb.Append("\">");
    }
}
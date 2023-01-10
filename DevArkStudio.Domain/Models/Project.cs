using System;
using System.Collections.Generic;
using System.Linq;

namespace DevArkStudio.Domain.Models;

public class Project
{
    public string Name { get; set; }

    public Dictionary<string, HTMLPage> Pages { get; init; } = new();
    public Dictionary<string, StyleSheet> StyleSheets { get; init; } = new();
    public Dictionary<string, FontItem> FontItems { get; init; } = new();

    public Project(string name)
    {
        Name = name;
    }

    public bool AddNewEmptyPage(string pageName, string pagePath)
    {
        if (Pages.ContainsKey(pagePath)) return false;

        var htmlHead = new HTMLHead();
        var bodyElm = new HTMLElement("body", Guid.NewGuid().ToString());
        Pages[pagePath] = new HTMLPage(htmlHead, bodyElm, pageName);

        return true;
    }

    public bool AddNewPage(HTMLPage htmlPage, string pagePath)
    {
        if (Pages.ContainsKey(pagePath)) return false;
        Pages[pagePath] = htmlPage;
        return true;
    }

    public void DeletePage(string pagePath)
    {
        Pages.Remove(pagePath);
    }

    public bool AddNewEmptyStyleSheet(string styleSheetPath)
    {
        if (StyleSheets.ContainsKey(styleSheetPath)) return false;

        var styleSheet = new StyleSheet
        {
            StyleSheetPath = styleSheetPath
        };
        StyleSheets[styleSheetPath] = styleSheet;
        
        return true;
    }

    public bool AddNewStyleSheet(StyleSheet styleSheet, string styleSheetPath)
    {
        if (StyleSheets.ContainsKey(styleSheetPath)) return false;
        StyleSheets[styleSheetPath] = styleSheet;
        return true;
    }

    public void DeleteStyleSheet(string styleSheetPath)
    {
        StyleSheets.Remove(styleSheetPath);
    }
}
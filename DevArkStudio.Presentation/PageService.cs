#nullable enable
using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence;
using DevArkStudio.Persistence.DTOModels;
using Microsoft.AspNetCore.Mvc;

namespace DevArkStudio.Presentation;

public class TreeDTOAnswer
{
    public bool Ok { get; init; }
    public TreeDTO? TreeDTO { get; init; }
}

public class NodeDTOAnswer
{
    public bool Ok { get; init; }
    public NodeDTO? NodeDTO { get; init; }
}

public class NodeWithTreeDTOAnswer
{
    public bool Ok { get; init; }
    public TreeDTO? TreeDTO { get; init; }
    public NodeDTO? NodeDTO { get; init; }
}

public class PageService
{
    private readonly ProjectService _projectService;
    private readonly ProjectDTOService _projectDTOService;
    
    public PageService([FromServices] ProjectService projectService, 
        [FromServices] ProjectDTOService projectDTOService )
    {
        _projectService = projectService;
        _projectDTOService = projectDTOService;
    }

    public TreeDTOAnswer GetTree(string pageName)
    {
        if (_projectService.Project is null || !_projectService.Project.Pages.ContainsKey(pageName)) 
            return new TreeDTOAnswer {Ok = false, TreeDTO = null};
        var page = _projectService.Project.Pages[pageName];
        var treeDTO = _projectDTOService.BuildTree(page.BodyElement);
        return new TreeDTOAnswer { Ok = true, TreeDTO = treeDTO };
    }

    public NodeDTOAnswer GetNode(string pageName, string nodeID)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || !_projectService.Project.Pages[pageName].AllNodes.ContainsKey(nodeID))
            return new NodeDTOAnswer { Ok = false, NodeDTO = null };
        return new NodeDTOAnswer
        {
            Ok = true,
            NodeDTO = _projectDTOService.BuildNodeDTO(_projectService.Project.Pages[pageName].AllNodes[nodeID])
        };
    }

    public NodeDTOAnswer UpdateElementClass(string pageName, string nodeID, string className)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || !_projectService.Project.Pages[pageName].AllNodes.ContainsKey(nodeID)
            || _projectService.Project.Pages[pageName].AllNodes[nodeID] is not IElement)
            return new NodeDTOAnswer { Ok = false, NodeDTO = null };
        (_projectService.Project.Pages[pageName].AllNodes[nodeID] as IElement)!.ClassName = className;
        return new NodeDTOAnswer
        {
            Ok = true,
            NodeDTO = _projectDTOService.BuildNodeDTO(_projectService.Project.Pages[pageName].AllNodes[nodeID])
        };
    }

    public NodeDTOAnswer UpdateElementID(string pageName, string nodeID, string styleID)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || !_projectService.Project.Pages[pageName].AllNodes.ContainsKey(nodeID)
            || _projectService.Project.Pages[pageName].AllNodes[nodeID] is not IElement)
            return new NodeDTOAnswer { Ok = false, NodeDTO = null };
        (_projectService.Project.Pages[pageName].AllNodes[nodeID] as IElement)!.StyleID = styleID;
        return new NodeDTOAnswer
        {
            Ok = true,
            NodeDTO = _projectDTOService.BuildNodeDTO(_projectService.Project.Pages[pageName].AllNodes[nodeID])
        };
    }
    
    public NodeDTOAnswer UpdateElementAttributes(string pageName, string nodeID, Dictionary<string, string> attributes)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || !_projectService.Project.Pages[pageName].AllNodes.ContainsKey(nodeID)
            || _projectService.Project.Pages[pageName].AllNodes[nodeID] is not IElement)
            return new NodeDTOAnswer { Ok = false, NodeDTO = null };
        (_projectService.Project.Pages[pageName].AllNodes[nodeID] as IElement)!.UpdateAttributes(attributes);
        return new NodeDTOAnswer
        {
            Ok = true,
            NodeDTO = _projectDTOService.BuildNodeDTO(_projectService.Project.Pages[pageName].AllNodes[nodeID])
        };
    }

    public NodeDTOAnswer UpdateTextContent(string pageName, string nodeID, string textContent)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || !_projectService.Project.Pages[pageName].AllNodes.ContainsKey(nodeID)
            || _projectService.Project.Pages[pageName].AllNodes[nodeID] is not IText)
            return new NodeDTOAnswer { Ok = false, NodeDTO = null };
        (_projectService.Project.Pages[pageName].AllNodes[nodeID] as IText)!.TextContent = textContent;
        return new NodeDTOAnswer
        {
            Ok = true,
            NodeDTO = _projectDTOService.BuildNodeDTO(_projectService.Project.Pages[pageName].AllNodes[nodeID])
        };
    }

    public TreeDTOAnswer DeleteNode(string pageName, string nodeID)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || _projectService.Project.Pages[pageName].BodyElement.NodeID == nodeID)
            return new TreeDTOAnswer { Ok = false, TreeDTO = null };
        _projectService.Project.Pages[pageName].RemoveNode(nodeID);
        return GetTree(pageName);
    }

    public TreeDTOAnswer TransferNode(string pageName, string rootNodeID, string targetNodeID,
        NodeManipulation nodeManipulation)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName)
            || _projectService.Project.Pages[pageName].BodyElement.NodeID == targetNodeID)
            return new TreeDTOAnswer { Ok = false, TreeDTO = null };
        var result = _projectService.Project.Pages[pageName].TransferNode(rootNodeID, targetNodeID, nodeManipulation);
        return !result ? new TreeDTOAnswer { Ok = false, TreeDTO = null } : GetTree(pageName);
    }

    public NodeWithTreeDTOAnswer CreateTextNode(string pageName, string rootNodeID, NodeManipulation nodeManipulation)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName))
            return new NodeWithTreeDTOAnswer { Ok = false, TreeDTO = null, NodeDTO = null };
        var page = _projectService.Project.Pages[pageName];
        var result = page.CreateTextNode(rootNodeID, nodeManipulation);
        return result.Item1
            ? new NodeWithTreeDTOAnswer
            {
                Ok = true,
                TreeDTO = _projectDTOService.BuildTree(page.BodyElement),
                NodeDTO = _projectDTOService.BuildNodeDTO(page.AllNodes[result.Item2!])
            }
            : new NodeWithTreeDTOAnswer {Ok = false, TreeDTO = null, NodeDTO = null};
    }
    
    public NodeWithTreeDTOAnswer CreateElmNode(string pageName, string rootNodeID, 
        string targetNodeTag, NodeManipulation nodeManipulation)
    {
        if (_projectService.Project is null
            || !_projectService.Project.Pages.ContainsKey(pageName))
            return new NodeWithTreeDTOAnswer { Ok = false, TreeDTO = null, NodeDTO = null };
        var page = _projectService.Project.Pages[pageName];
        var result = page.CreateElementNode(rootNodeID, targetNodeTag, nodeManipulation);
        return result.Item1
            ? new NodeWithTreeDTOAnswer
            {
                Ok = true,
                TreeDTO = _projectDTOService.BuildTree(page.BodyElement),
                NodeDTO = _projectDTOService.BuildNodeDTO(page.AllNodes[result.Item2!])
            }
            : new NodeWithTreeDTOAnswer {Ok = false, TreeDTO = null, NodeDTO = null};
    }
}
using System.Collections.Generic;
using System.Linq;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class PageDTO
{
    public string Name { get; init; } = "";
    public string BodyElmUID { get; init; } = "";
    public HeadDTO HeadDTO { get; init; } = new();

    public Dictionary<string, NodeDTO> AllNodes { get; init; } = new();
    
    public PageDTO() {}

    public PageDTO(IHTMLPage page)
    {
        Name = page.PageName;
        BodyElmUID = page.BodyElement.NodeID;
        AllNodes = page.AllNodes
            .Select(nodePair =>
            {
                var nodeDTO = nodePair.Value switch
                {
                    HTMLElement element => new ElementDTO(element),
                    TextNode text => new TextDTO(text),
                    _ => new NodeDTO()
                };

                return new KeyValuePair<string, NodeDTO>(nodePair.Key, nodeDTO);
            })
            .ToDictionary(
                source => source.Key,
                source => source.Value
            );
        HeadDTO = new HeadDTO(page.HTMLHead);
    }

    public IHTMLPage CreatePage()
    {
        var allNodes = AllNodes.Select(nodePair =>
        {
            INode node = nodePair.Value switch
            {
                ElementDTO elementDTO => elementDTO.CreateElement(),
                TextDTO textDTO => textDTO.CreateTextNode(),
                { } nodeDTO => new HTMLElement("div", nodeDTO.NodeID)
            };
            
            return new KeyValuePair<string, INode>(nodePair.Key, node);
        })
        .ToDictionary(
            source => source.Key,
            source => source.Value
        );
        
        foreach (var nodePair in AllNodes)
        {
            var nodeDTO = nodePair.Value;

            foreach (var childNode in nodeDTO.ChildNodes)
            {
                (allNodes[nodeDTO.NodeID] as IElement)?.Append(allNodes[childNode]);
            }
        }

        var htmlHead = HeadDTO.CreateHTMLHead();
        
        return new HTMLPage(htmlHead, allNodes[BodyElmUID] as IElement, Name)
        {
            AllNodes = allNodes
        };
    }
}
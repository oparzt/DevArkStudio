using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Persistence.DTOModels;

namespace DevArkStudio.Persistence;

public class ProjectDTOService
{

    public TreeDTO BuildTree(IElement bodyElement)
    {
        var nodeQueue = new Queue<INode>();
        var rootNodeDTO = new ElementDTO(bodyElement);
        var allNodesDTO = new Dictionary<string, NodeDTO>();
        
        nodeQueue.Enqueue(bodyElement);

        while (nodeQueue.TryDequeue(out var node))
        {
            foreach (var child in node.ChildNodes) nodeQueue.Enqueue(child);
            allNodesDTO[node.NodeID] = BuildNodeDTO(node);
        }

        return new TreeDTO
        {
            RootNode = rootNodeDTO,
            AllNodes = allNodesDTO
        };
    }

    public NodeDTO BuildNodeDTO(INode node)
    {
        return node switch
        {
            IElement element => new ElementDTO(element),
            IText text => new TextDTO(text),
            _ => new NodeDTO(node)
        };
    }
}
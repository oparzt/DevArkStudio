using System.Collections.Generic;

namespace DevArkStudio.Persistence.DTOModels;

public class TreeDTO
{
    public NodeDTO RootNode { get; set; }
    public Dictionary<string, NodeDTO> AllNodes { get; set; }
}
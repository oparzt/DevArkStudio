using System.Collections.Generic;

namespace DevArkStudio.Domain.Interfaces
{
    public interface IDocument
    { 
        string HTML { get; }
        string DocumentType { get; }
        string DocumentURL { get; set; }
        IElement DocumentElement { get; }
        Dictionary<string, INode> AllNodes { get; }
        
        Dictionary<string, string> Files { get; }
    }
}
using System.Collections.Generic;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Domain.Interfaces
{
    public interface IHTMLPage
    { 
        string PageName { get; }
        string DocumentType { get; }
        HTMLHead HTMLHead { get; }
        IElement BodyElement { get; }
        Dictionary<string, INode> AllNodes { get; }
        
        List<FontItem> Fonts { get; }
    }
}
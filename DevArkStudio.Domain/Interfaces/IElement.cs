using System.Collections.Generic;

namespace DevArkStudio.Domain.Interfaces
{
    public interface IElement : INode
    {
        HashSet<string> ClassList { get; }
        string ClassName { get; set; }
        
        string StyleID { get; set; }
        
        string InnerHTML { get; }
        string OuterHTML { get; }
        
        string TagName { get; }
        
        Dictionary<string, string> Attributes { get; }

        IElement CloneNode();
        bool AddClass(string className);
        bool RemoveClass(string className);

        bool Append(INode node);
        bool Prepend(INode node);
        bool Before(INode node);
        bool After(INode node);

        string GetAttribute(string name);
        bool SetAttribute(string name, string value);
        bool RemoveAttribute(string name);
    }
}
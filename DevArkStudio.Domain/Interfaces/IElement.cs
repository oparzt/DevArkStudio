using System.Collections.Generic;
using System.Text;

namespace DevArkStudio.Domain.Interfaces
{
    public interface IElement : INode
    {
        List<string> ClassList { get; }
        string ClassName { get; set; }
        
        string StyleID { get; set; }
        
        string InnerHTML { get; }
        string OuterHTML { get; }
        
        string TagName { get; }
        
        Dictionary<string, string> Attributes { get; }

        IElement CloneNode();
        void AddClass(string className);
        bool RemoveClass(string className);

        void UpdateAttributes(Dictionary<string, string> attributes);

        public void RenderInnerHTML(StringBuilder sb, bool develop = true);

        public void RenderOuterHTML(StringBuilder sb, bool develop = true);
    }
}
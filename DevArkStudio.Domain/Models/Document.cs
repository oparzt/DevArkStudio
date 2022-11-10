using System.Collections.Generic;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models
{
    public class Document : IDocument
    {
        public string HTML => DocumentType + DocumentElement.OuterHTML;
        public string DocumentType { get; }
        public string DocumentURL { get; set; }
        public IElement DocumentElement { get; }
        public Dictionary<string, INode> AllNodes => new();
        public Dictionary<string, string> Files => new();

        public Document(IElement documentElement, string documentType = "<!DOCTYPE html>")
        {
            DocumentType = documentType;
            DocumentElement = documentElement;
        }
    }
}
using System.Collections.Generic;

namespace DevArkStudio.Domain.Interfaces;

public interface IStyleComponent
{
    public string Selector { get; set; } 
    public Dictionary<string, string> Styles { get; set; }
}
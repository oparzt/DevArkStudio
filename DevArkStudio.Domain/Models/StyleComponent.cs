using System.Collections.Generic;
using System.Text;
using DevArkStudio.Domain.Interfaces;

namespace DevArkStudio.Domain.Models;

public class StyleComponent : IStyleComponent
{
    public string StyleID { get; set; } = "";
    public string Selector { get; set; } = "";
    public Dictionary<string, string> Styles { get; set; } = new();

    public void RenderStyleComponent(StringBuilder sb, bool develop)
    {
        if (Selector.Length == 0) return;

        sb.Append(Selector);
        sb.Append(" {");
        foreach (var stylePair in Styles)
        {
            var value = develop ? stylePair.Value.Replace("url('", "url('/Develop/") : stylePair.Value;
            sb.Append(stylePair.Key);
            sb.Append(":");
            sb.Append(value);
            sb.Append(";");
        }
        sb.Append("}");
    }
}
using System;
using System.Linq;
using System.Text;

namespace DevArkStudio.Domain.Models;

public class FontItem
{
    public string FontName => FontFamily + ";" + FontStyle + ";" + FontWeight;
    public string FontFamily { get; set; }
    public string FontStyle { get; set; }
    public string FontWeight { get; set; }

    public string EotFontPath { get; set; } = "";
    public string Woff2FontPath { get; set; } = "";
    public string WoffFontPath { get; set; } = "";
    public string TTFFontPath { get; set; } = "";
    public string SVGFontPath { get; set; } = "";

    public static string LocalFontConnectPath => "local('')";
    public string EotFontConnectPath => EotFontPath.Length > 0 ? $"url('{EotFontPath}?#iefix') format('embedded-opentype')" : "";
    public string Woff2FontConnectPath => Woff2FontPath.Length > 0 ? $"url('{Woff2FontPath}') format('woff2')" : "";
    public string WoffFontConnectPath => WoffFontPath.Length > 0 ? $"url('{WoffFontPath}') format('woff')" : "";
    public string TTFFontConnectPath => TTFFontPath.Length > 0 ? $"url('{TTFFontPath}') format('truetype')" : "";
    public string SVGFontConnectPath => SVGFontPath.Length > 0 ? $"url('{SVGFontPath}') format('svg')" : "";


    public void RenderFontConnection(StringBuilder sb, bool develop)
    {
        sb.Append(" @font-face { ");
        sb.Append($" font-family: \"{FontFamily}\"; ");
        sb.Append($" font-style: {FontStyle}; ");
        sb.Append($" font-weight: {FontWeight}; ");
        if (EotFontPath.Length > 0) sb.Append($" src: url('{(develop ?  "/Develop/" : "") + EotFontPath}'); ");

        var paths = new[]
        {
            LocalFontConnectPath, EotFontConnectPath, Woff2FontConnectPath,
            WoffFontConnectPath, TTFFontConnectPath, SVGFontConnectPath
        }
            .Where(path => path.Length > 0)
            .Select(path => develop ? path.Replace("url('", "url('/Develop/") : path)
            .ToArray();

        sb.Append(" src: ");
        sb.AppendJoin(", ", paths);
        sb.Append(";} ");
    }
}
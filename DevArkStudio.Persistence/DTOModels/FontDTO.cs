using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class FontDTO
{
    [JsonPropertyName("fontFamily")]
    public string FontFamily { get; init; }
    
    [JsonPropertyName("fontStyle")]
    public string FontStyle { get; init; }
    
    [JsonPropertyName("fontWeight")]
    public string FontWeight { get; init; }
    

    [JsonPropertyName("fileName")]
    public string FileName { get; init; } = "";

    public FontDTO()
    {
        
    }

    public FontDTO(FontItem fontItem)
    {
        FontFamily = fontItem.FontFamily;
        FontStyle = fontItem.FontStyle;
        FontWeight = fontItem.FontWeight;
        
        var path = new[]
        {
            fontItem.EotFontConnectPath, fontItem.Woff2FontConnectPath,
            fontItem.WoffFontConnectPath, fontItem.TTFFontConnectPath, 
            fontItem.SVGFontConnectPath
        }.FirstOrDefault(path => path.Length > 0);
        
        
        FileName = Path.GetFileNameWithoutExtension(path);
    }
}
using System.Text;

namespace DevArkStudio.Domain.Models;

public class HTMLHead
{
    public HTMLPage? HTMLPage { get; set; }
    public string Title { get; set; } = "";
    
    
    public void RenderOuterHTML(StringBuilder sb, bool develop)
    {
        sb.Append("<head>");
        sb.Append("<meta charset=\"utf-8\"/>");
        sb.Append("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"/>");
        sb.Append("<title>");
        sb.Append(Title);
        sb.Append("</title>");
        if (HTMLPage is not null)
            foreach (var styleSheet in HTMLPage.StyleSheets)
                styleSheet.RenderStyleSheetConnect(sb, develop);
        sb.Append("<style>");
        if (HTMLPage is not null)
            foreach (var fontItem in HTMLPage.Fonts)
                fontItem.RenderFontConnection(sb, develop);
        sb.Append("</style>");
        sb.Append("</head>");
    }
}
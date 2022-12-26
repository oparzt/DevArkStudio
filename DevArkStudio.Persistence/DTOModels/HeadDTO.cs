using DevArkStudio.Domain.Models;

namespace DevArkStudio.Persistence.DTOModels;

public class HeadDTO
{
    public string Title { get; init; } = "";
    
    public HeadDTO() {}

    public HeadDTO(HTMLHead htmlHead)
    {
        Title = htmlHead.Title;
    }

    public HTMLHead CreateHTMLHead()
    {
        return new HTMLHead
        {
            Title = Title
        };
    }
}
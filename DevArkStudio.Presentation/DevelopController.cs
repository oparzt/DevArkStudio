using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace DevArkStudio.Presentation;

[Controller]
[Route("/[controller]/")]
public class DevelopController : Controller
{
    private readonly FileExtensionContentTypeProvider _fileExtensionHelper = new();
    
    [HttpGet()]
    public IActionResult Index([FromServices] ProjectService projectService)
    {
        return RenderFile("index.html", projectService);
    }
    
    [HttpGet("{*catchall}")]
    public IActionResult GetPage([FromRoute] string catchall, [FromServices] ProjectService projectService)
    {
        return RenderFile(catchall, projectService);
    }

    [NonAction]
    private IActionResult RenderFile(string path, ProjectService projectService)
    {
        RenderAnswer renderAnswer;
        if (Path.HasExtension(path))
        {
            var ext = Path.GetExtension(path);
            renderAnswer = ext switch
            {
                ".html" => projectService.RenderHTML(path),
                ".css" => projectService.RenderCSS(path),
                _ => projectService.RenderFile(path)
            };
        }
        else
        {
            if (path != null && path.EndsWith('/'))
            {
                renderAnswer = projectService.RenderHTML(path + "index.html");
            }
            else
            {
                return Redirect(path + "/");
            }

        }
        
        var foundContentType = _fileExtensionHelper.TryGetContentType(path, out var contentType);
        if (!renderAnswer.Ok || !foundContentType) return new NotFoundResult();
        var mediaContentType = MediaTypeHeaderValue.Parse(contentType);

        
        return renderAnswer switch
        {
            TextRenderAnswer textRenderAnswer => new FileStreamResult(new MemoryStream(
                Encoding.UTF8.GetBytes(textRenderAnswer.Answer)), mediaContentType),
            FileRenderAnswer fileRenderAnswer => new FileStreamResult(fileRenderAnswer.Answer!, mediaContentType),
            _ => new OkResult()
        };
    }
}
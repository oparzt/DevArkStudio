using System;
using System.Text.Json;
using DevArkStudio.Domain.Models;

namespace DevArkStudio.Domain
{
  public abstract class Program
    {
      public abstract string HtmlEncoder (string documentElement);

      public static void Main()
      {
        var documentElement = new HTMLElement(HtmlEncoder())

        var document = new Document(documentElement)
        {

        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(document, options);

        Console.WriteLine(jsonString);
      }

      private static string HtmlEncoder()
      {
        throw new NotImplementedException();
      }
    }
}

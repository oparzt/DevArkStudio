using System;
using System.Linq;
using System.Text.Json;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;
using DevArkStudio.Persistence.DTOModels;

namespace DevArkStudio.Infrastructure.JSON
{
    public static class Serializer
    {
        public static readonly JsonSerializerOptions Options = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        public static string SerializePage(IHTMLPage htmlPage)
        {
            return JsonSerializer.Serialize(new PageDTO(htmlPage), Options);
        }

        public static string SerializeNode(INode node)
        {
            var json = "";

            switch (node)
            {
                case HTMLElement element:
                    json += JsonSerializer.Serialize<NodeDTO>(new ElementDTO(element), Options);
                    break;
                case TextNode text:
                    json += JsonSerializer.Serialize<NodeDTO>(new TextDTO(text), Options);
                    break;
            }
            
            return json;
        }
    }
}
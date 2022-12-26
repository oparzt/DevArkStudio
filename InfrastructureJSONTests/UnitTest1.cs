using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;
using DevArkStudio.Infrastructure.JSON;
using DevArkStudio.Persistence.DTOModels;
using ExCSS;
using NUnit.Framework;

namespace InfrastructureJSONTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        // [Test]
        // public void ShowTest()
        // {
        //     var nodes = new List<NodeDTO>();
        //
        //     var docElm = new HTMLElement("html", "2");
        //     var textNode = new TextNode("Ahahaha", "0");
        //     var htmlNode = new HTMLElement("h1", "1");
        //
        //     var page = new HTMLPage(docElm, "def");
        //
        //     htmlNode.Append(textNode);
        //     
        //     nodes.Add(new TextDTO(textNode));
        //     nodes.Add(new ElementDTO(htmlNode));
        //
        //     var pageDTO = new PageDTO(page);
        //     
        //     var json = JsonSerializer.Serialize(pageDTO, new JsonSerializerOptions
        //     {
        //         WriteIndented = true,
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //     });
        //
        //     var desJson = JsonSerializer.Deserialize<PageDTO>(json, new JsonSerializerOptions
        //     {
        //         WriteIndented = true,
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //     });
        //
        //     Console.WriteLine(json);
        //     
        //     Assert.Pass();
        // }

        [Test]
        public void TestCSS()
        {
            var fontItemSrc = new FontItem()
            {
                FontFamily = "Inter",
                FontStyle = "normal",
                FontWeight = "400",
                EotFontPath = "/fonts/inter.eot",
                Woff2FontPath = "/fonts/inter.woff2",
                WoffFontPath = "/fonts/inter.woff",
                TTFFontPath = "/fonts/inter.ttf",
                SVGFontPath = "/fonts/inter.svg"

            };
            var sb = new StringBuilder();
            fontItemSrc.RenderFontConnection(sb, false);
            var fontItemText = sb.ToString();
            var cssParser = new StylesheetParser();
            var stylesheet = cssParser.Parse(fontItemText);
            sb.Clear();

            foreach (var fontface in stylesheet.FontfaceSetRules)
            {
                var sources = fontface.Source
                    .Split(',', ' ')
                    .Where(src => src.Contains("url"))
                    .Select(src => src
                        .Replace("url('", string.Empty)
                        .Replace("url(\"", string.Empty)
                        .Replace("?#iefix", string.Empty)
                        .Replace("')", string.Empty)
                        .Replace("\")", string.Empty))
                    .ToDictionary(Path.GetExtension, src => src);
                
                var fontItem = new FontItem()
                {
                    FontFamily = fontface.Family,
                    FontStyle = fontface.Style,
                    FontWeight = fontface.Weight,
                    
                    EotFontPath = sources.ContainsKey(".eot") ? sources[".eot"] : "",
                    Woff2FontPath = sources.ContainsKey(".woff2") ? sources[".woff2"] : "",
                    WoffFontPath = sources.ContainsKey(".woff") ? sources[".woff"] : "",
                    TTFFontPath = sources.ContainsKey(".ttf") ? sources[".ttf"] : "",
                    SVGFontPath = sources.ContainsKey(".svg") ? sources[".svg"] : ""
                };
                
                fontItem.RenderFontConnection(sb, false);
            }
            
            Assert.AreEqual(fontItemText, sb.ToString());
        }

        [TestCase("{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"\",\"childNodes\":[],\"tagName\":\"h1\"}", "h1")]
        [TestCase("{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"\",\"childNodes\":[],\"tagName\":\"h2\"}", "h2")]
        [TestCase("{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"\",\"childNodes\":[],\"tagName\":\"h3\"}", "h3")]
        public void SerializeElm_WithoutChildrenAndParent(string exp, string tag)
        {
            var elm = new HTMLElement(tag, "1");
            var json = Serializer.SerializeNode(elm);
            
            Console.WriteLine(json);
            Assert.AreEqual(Helper.FormatJson(exp), json);
        }

        [Test]
        public void SerializeElm_WithChildrenAndWithoutParent()
        {
            var exp = "{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"\",\"childNodes\":[\"2\",\"3\",\"4\"],\"tagName\":\"h1\"}";
            var h1 = new HTMLElement("h1", "1");
            var span1 = new HTMLElement("span", "2");
            var span2 = new HTMLElement("span", "3");
            var span3 = new HTMLElement("span", "4");
            h1.Append(span1);
            h1.Append(span2);
            h1.Append(span3);
            var json = Serializer.SerializeNode(h1);

            Console.WriteLine(json);
            Assert.AreEqual(Helper.FormatJson(exp), json);
        }
        
        [Test]
        public void SerializeElm_WithChildrenAndWithParent()
        {
            var exp = "{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"0\",\"childNodes\":[\"2\",\"3\",\"4\"],\"tagName\":\"h1\"}";
            var div = new HTMLElement("div", "0");
            var h1 = new HTMLElement("h1", "1");
            var span1 = new HTMLElement("span", "2");
            var span2 = new HTMLElement("span", "3");
            var span3 = new HTMLElement("span", "4");
            div.Append(h1);
            h1.Append(span1);
            h1.Append(span2);
            h1.Append(span3);
            var json = Serializer.SerializeNode(h1);

            Console.WriteLine(json);
            Assert.AreEqual(Helper.FormatJson(exp), json);
        }
        
        [Test]
        public void SerializeElm_WithoutChildrenAndWithParent()
        {
            var exp = "{\"nodeType\":1,\"nodeId\":\"1\",\"parentNode\":\"0\",\"childNodes\":[],\"tagName\":\"h1\"}";
            var div = new HTMLElement("div", "0");
            var h1 = new HTMLElement("h1", "1");
            div.Append(h1);
            var json = Serializer.SerializeNode(h1);

            Console.WriteLine(json);
            Assert.AreEqual(Helper.FormatJson(exp), json);
        }
    }
}
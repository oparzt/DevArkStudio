using System;
using DevArkStudio.Domain.Models;
using DevArkStudio.Infrastructure.JSON;
using NUnit.Framework;

namespace InfrastructureJSONTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
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
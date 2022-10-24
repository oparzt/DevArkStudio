using DevArkStudio.Domain.Interfaces;
using DevArkStudio.Domain.Models;
using NUnit.Framework;

namespace DomainModelsTests
{
    [TestFixture]
    public class TextTests
    {
        
        [Test]
        public void SimpleTextCreate()
        {
            var text = new Text();
            
            Assert.AreEqual(NodeType.TEXT_NODE,text.NodeType);
            Assert.AreEqual("#text",text.NodeName);
            Assert.AreEqual("",text.NodeValue);
            Assert.AreEqual("", text.TextContent);
            Assert.AreEqual(text.TextContent,text.NodeValue);
            Assert.AreEqual(0, text.NodeName.Length);
            Assert.AreEqual(null, text.ParentNode);
        }

        [TestCase("Hello")]
        [TestCase("World")]
        [TestCase("Hello World")]
        public void TextCreateWithString(string s)
        {
            var text = new Text(s);
            
            Assert.AreEqual(s, text.NodeValue);
            Assert.AreEqual(s, text.TextContent);
            Assert.AreEqual(text.TextContent, text.NodeValue);
        }

        [TestCase("Hello")]
        [TestCase("World")]
        [TestCase("Hello World")]
        public void CreateElementWithText(string s)
        {
            var div = new HTMLElement("div");
            var text = new Text(s);

            div.Append(text);

            Assert.AreEqual(s, div.TextContent);
        }
        
        [TestCase("Hello")]
        [TestCase("World")]
        [TestCase("Hello World")]
        public void CreateElementWithoutCreatingTextNode(string s)
        {
            var div = new HTMLElement("div");
            div.TextContent = s;
            Assert.AreEqual(s, div.TextContent);
        }
        
        [TestCase("Hello", "World", "Hello World")]
        public void CreateElementWithoutCreatingTextNodeAndReplaceText(params string[] s)
        {
            var div = new HTMLElement("div");
            
            foreach (var s1 in s)
            {
                div.TextContent = s1;
                Assert.AreEqual(s, div.TextContent);
            }
        }
        
        [TestCase("Hello", "World", "Hello World")]
        public void CreateElementWithoutCreatingTextNodeAndReplaceAllElms(params string[] s)
        {
            var div = new HTMLElement("div");
            
            foreach (var s1 in s)
            {
                div.Append(new HTMLElement("a"));
                div.TextContent = s1;
                Assert.AreEqual(s, div.TextContent);
                Assert.AreEqual(0, div.ChildNodes.Count);
            }
        }
    }
}
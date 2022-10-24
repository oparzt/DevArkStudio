using DevArkStudio.Domain.Models;
using NUnit.Framework;

namespace DomainModelsTests
{
    [TestFixture]
    public class HTMLElementTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("div")]
        [TestCase("a")]
        [TestCase("h1")]
        [TestCase("p")]
        [TestCase("ul")]
        public void RightTagNameTest(string tag)
        {
            var div = new HTMLElement(tag);
            Assert.AreEqual(tag, div.TagName);
        }

        [TestCase("div", "a h1 p", "<div>\n<a>\n\n</a>\n<h1>\n\n</h1>\n<p>\n\n</p>\n</div>")]
        [TestCase("p", "a span div", "<p>\n<a>\n\n</a>\n<span>\n\n</span>\n<div>\n\n</div>\n</p>")]
        [TestCase("div", "a", "<div>\n<a>\n\n</a>\n</div>")]
        [TestCase("a", "span img", "<a>\n<span>\n\n</span>\n<img>\n\n</img>\n</a>")]
        public void RightOuterHTML(string parentTag, string childTags, string outerHTML)
        {
            var parent = new HTMLElement(parentTag);
            foreach (var childTag in childTags.Split(" "))
                parent.Append(new HTMLElement(childTag));
            Assert.AreEqual(outerHTML, parent.OuterHTML);
        }
        
        [TestCase("div", "a h1 p", "<a>\n\n</a>\n<h1>\n\n</h1>\n<p>\n\n</p>")]
        [TestCase("p", "a span div", "<a>\n\n</a>\n<span>\n\n</span>\n<div>\n\n</div>")]
        [TestCase("div", "a", "<a>\n\n</a>")]
        [TestCase("a", "span img", "<span>\n\n</span>\n<img>\n\n</img>")]
        public void RightInnerHTML(string parentTag, string childTags, string innerHTML)
        {
            var parent = new HTMLElement(parentTag);
            foreach (var childTag in childTags.Split(" "))
                parent.Append(new HTMLElement(childTag));
            Assert.AreEqual(innerHTML, parent.InnerHTML);
        }

        [Test]
        public void False_BeforeAfter_Where_ParentNodeIsNull()
        {
            var div = new HTMLElement("div");
            var a = new HTMLElement("a");
            
            Assert.False(div.After(a));
            Assert.False(div.Before(a));
        }

        [Test]
        public void Right_BeforeAfter()
        {
            var div = new HTMLElement("div");
            var a = new HTMLElement("a");
            var span = new HTMLElement("span");
            var h1 = new HTMLElement("h1");
            var p = new HTMLElement("p");

            div.Append(span);
            span.Before(a);
            span.After(p);
            p.Before(h1);
            
            Assert.AreEqual(new[] {a, span, h1, p}, div.ChildNodes);
            foreach (var elm in new[] {a, span, h1, p})
            {
                Assert.AreEqual(elm.ParentNode, div);
            }
        }

        [Test]
        public void Right_AppendPrepend()
        {
            var div = new HTMLElement("div");
            var a = new HTMLElement("a");
            var span = new HTMLElement("span");
            var h1 = new HTMLElement("h1");
            var p = new HTMLElement("p");

            div.Append(a);
            a.Append(span);
            a.Prepend(p);
            div.Prepend(h1);
            
            Assert.AreEqual(new[] {h1, a}, div.ChildNodes);
            Assert.AreEqual(new[] {p, span}, a.ChildNodes);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting;
using Microsoft.BizTalk.Component.Interop;

namespace Winterdom.BizTalk.PipelineTesting.Tests {
   [TestFixture]
   public class InstConfigPropertyBagTests {

      [Test]
      public void CanLoadSimpleProperties() {
         string xml =
            @"<Properties>
               <AddXmlDeclaration vt='11'>0</AddXmlDeclaration>
               <PreserveBom vt='11'>1</PreserveBom>
              </Properties>";
         InstConfigPropertyBag bag = new InstConfigPropertyBag(Load(xml));
         Assert.AreEqual(false, bag.Read("AddXmlDeclaration"));
         Assert.AreEqual(true, bag.Read("PreserveBom"));
      }

      private XmlReader Load(string xml) {
         XmlReader reader = new XmlTextReader(new StringReader(xml));
         reader.Read();
         return reader;
      }
   }
}

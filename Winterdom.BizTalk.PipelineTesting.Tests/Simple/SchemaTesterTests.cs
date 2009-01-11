using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using Winterdom.BizTalk.PipelineTesting.Simple;
using SampleSchemas;

namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple {
   [TestFixture]
   public class SchemaTesterTests {

      [Test]
      public void CanTestFlatFileParsing() {
         Stream input = DocLoader.LoadStream("CSV_FF_RecvInput.txt");
         Stream output = SchemaTester<Schema3_FF>.ParseFF(input);
         try {
            XmlDocument doc = new XmlDocument();
            doc.Load(output);
         } catch ( Exception ex ) {
            Assert.Fail(ex.ToString());
         }
      }

      [Test]
      public void CanGetErrorInfoWhenFFParsingFails() {
         Stream input = DocLoader.LoadStream("CSV_FF_RecvInput_Bad.txt");
         try {
            Stream output = SchemaTester<Schema3_FF>.ParseFF(input);
            MessageHelper.ConsumeStream(output);
         } catch ( Exception ex ) {
            String msg = ErrorHelper.GetErrorMessage(ex);
            Assert.That(msg, new ContainsConstraint("Unexpected data"));
            return;
         }
         Assert.Fail("Parsing should've thrown an error");
      }

      [Test]
      public void CanTestFlatFileAssembling() {
         Stream input = DocLoader.LoadStream("CSV_XML_SendInput.xml");
         Stream output = SchemaTester<Schema3_FF>.AssembleFF(input);
         String text = new StreamReader(output).ReadToEnd();
         Assert.IsTrue(text.Contains(","), "Output contains no commas");
      }

      [Test]
      public void CanTestXmlParsing() {
         Stream input = DocLoader.LoadStream("SampleDocument.xml");
         Stream output = SchemaTester<Schema2_WPP>.ParseXml(input);
         try {
            XmlDocument doc = new XmlDocument();
            doc.Load(output);
         } catch ( Exception ex ) {
            Assert.Fail(ex.ToString());
         }
      }

      [Test]
      public void CanGetErrorWhenXmlParsingFails() {
         Stream input = DocLoader.LoadStream("SampleDocument_Bad.xml");
         try {
            Stream output = SchemaTester<Schema2_WPP>.ParseXml(input);
            MessageHelper.ConsumeStream(output);
         } catch ( Exception ex ) {
            String msg = ErrorHelper.GetErrorMessage(ex);
            Assert.That(msg, new ContainsConstraint("incomplete content"));
            return;
         }
         Assert.Fail("Parsing should've thrown an error");
      }

      [Test]
      public void CanTestXmlAssembling() {
         Stream input = DocLoader.LoadStream("SampleDocument.xml");
         Stream output = SchemaTester<Schema2_WPP>.AssembleXml(input);
         try {
            XmlDocument doc = new XmlDocument();
            doc.Load(output);
         } catch ( Exception ex ) {
            Assert.Fail(ex.ToString());
         }
      }
   }
}

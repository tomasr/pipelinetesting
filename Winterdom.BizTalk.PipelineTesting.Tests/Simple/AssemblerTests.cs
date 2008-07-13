
//
// AssemblerTests.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting.Simple;
using SampleSchemas;

namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple
{
   [TestFixture]
   public class AssemblerTests
   {
      #region Xml Assembler Tests
      //
      // Xml Assembler Tests
      //
      [Test]
      public void Xml_CanAddDocumentSpec()
      {
         IBaseComponent assembler = Assembler.Xml()
            .WithDocumentSpec<Schema1_NPP>().End();

         XmlAsmComp xml = assembler as XmlAsmComp;
         Assert.IsNotNull(xml);
         Assert.AreEqual(1, xml.DocumentSpecNames.Count);
      }

      [Test]
      public void Xml_CanAddEnvelopeSpec()
      {
         IBaseComponent assembler = Assembler.Xml()
            .WithEnvelopeSpec<SimpleEnv>().End();

         XmlAsmComp xml = assembler as XmlAsmComp;
         Assert.AreEqual(1, xml.EnvelopeDocSpecNames.Count);
      }

      [Test]
      public void Xml_WithXmlDeclaration()
      {
         IBaseComponent assembler = Assembler.Xml()
            .WithXmlDeclaration(true).End();

         XmlAsmComp xml = assembler as XmlAsmComp;
         Assert.AreEqual(true, xml.AddXMLDeclaration);
      }

      [Test]
      public void Xml_WithPreserveBom()
      {
         IBaseComponent assembler = Assembler.Xml()
            .WithPreserveBom(true).End();

         XmlAsmComp xml = assembler as XmlAsmComp;
         Assert.AreEqual(true, xml.PreserveBom);
      }
      #endregion // Xml Assembler Tests

      #region Flat File Assembler Tests
      //
      // Flat File Assembler Tests
      //
      [Test]
      public void FF_CanAddDocumentSpec()
      {
         IBaseComponent assembler = Assembler.FlatFile()
            .WithDocumentSpec<Schema1_NPP>().End();

         FFAsmComp ff = assembler as FFAsmComp;
         Assert.IsNotNull(ff.DocumentSpecName);
      }

      [Test]
      public void FF_CanAddHeaderSpec()
      {
         IBaseComponent assembler = Assembler.FlatFile()
            .WithHeaderSpec<Schema1_NPP>().End();

         FFAsmComp ff = assembler as FFAsmComp;
         Assert.IsNotNull(ff.HeaderSpecName);
      }

      [Test]
      public void FF_CanAddTrailerSpec()
      {
         IBaseComponent assembler = Assembler.FlatFile()
            .WithTrailerSpec<Schema1_NPP>().End();

         FFAsmComp ff = assembler as FFAsmComp;
         Assert.IsNotNull(ff.TrailerSpecName);
      }

      [Test]
      public void FF_WithPreserveBom()
      {
         IBaseComponent assembler = Assembler.FlatFile()
            .WithPreserveBom(true).End();

         FFAsmComp ff = assembler as FFAsmComp;
         Assert.AreEqual(true, ff.PreserveBom);
      }
      #endregion // Flat File Assembler Tests

   } // class AssemblerTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


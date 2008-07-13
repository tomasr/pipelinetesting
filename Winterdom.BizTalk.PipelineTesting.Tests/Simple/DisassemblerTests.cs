
//
// DisassemblerTests.cs
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
   public class DisassemblerTests
   {
      #region Xml Disassembler Tests
      //
      // Xml Disassembler Tests
      //
      [Test]
      public void Xml_CanAddDocumentSpec()
      {
         IBaseComponent disassembler = Disassembler.Xml()
            .WithDocumentSpec<Schema1_NPP>().End();

         XmlDasmComp xml = disassembler as XmlDasmComp;
         Assert.IsNotNull(xml);
         Assert.AreEqual(1, xml.DocumentSpecNames.Count);
      }

      [Test]
      public void Xml_CanAddEnvelopeSpec()
      {
         IBaseComponent disassembler = Disassembler.Xml()
            .WithEnvelopeSpec<SimpleEnv>().End();

         XmlDasmComp xml = disassembler as XmlDasmComp;
         Assert.AreEqual(1, xml.EnvelopeSpecNames.Count);
      }

      [Test]
      public void Xml_WithValidation()
      {
         IBaseComponent disassembler = Disassembler.Xml()
            .WithValidation(true).End();

         XmlDasmComp xml = disassembler as XmlDasmComp;
         Assert.AreEqual(true, xml.ValidateDocument);
      }

      [Test]
      public void Xml_WithRIP()
      {
         IBaseComponent disassembler = Disassembler.Xml()
            .WithRecoverableInterchange(true).End();

         XmlDasmComp xml = disassembler as XmlDasmComp;
         Assert.AreEqual(true, xml.RecoverableInterchangeProcessing);
      }
      #endregion // Xml Disassembler Tests

      #region Flat File Disassembler Tests
      //
      // Flat File Disassembler Tests
      //
      [Test]
      public void FF_CanAddDocumentSpec()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithDocumentSpec<Schema1_NPP>().End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.IsNotNull(ff.DocumentSpecName);
      }

      [Test]
      public void FF_CanAddHeaderSpec()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithHeaderSpec<Schema1_NPP>().End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.IsNotNull(ff.HeaderSpecName);
      }

      [Test]
      public void FF_CanAddTrailerSpec()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithTrailerSpec<Schema1_NPP>().End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.IsNotNull(ff.TrailerSpecName);
      }

      [Test]
      public void FF_WithValidation()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithValidation(true).End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.AreEqual(true, ff.ValidateDocumentStructure);
      }

      [Test]
      public void FF_WithPreserveHeader()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithPreserveHeader(true).End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.AreEqual(true, ff.PreserveHeader);
      }

      [Test]
      public void FF_WithRIP()
      {
         IBaseComponent disassembler = Disassembler.FlatFile()
            .WithRecoverableInterchange(true).End();

         FFDasmComp ff = disassembler as FFDasmComp;
         Assert.AreEqual(true, ff.RecoverableInterchangeProcessing);
      }
      #endregion // Flat File Disassembler Tests

   } // class DisassemblerTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


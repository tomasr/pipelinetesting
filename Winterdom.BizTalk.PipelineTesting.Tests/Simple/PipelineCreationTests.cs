
//
// PipelineCreationTests.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting.Simple;

namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple
{
   [TestFixture]
   public class PipelineCreationTests
   {
      [Test]
      public void CanCreateXmlReceive()
      {
         Assert.IsNotNull(Pipelines.Xml.Receive().End());
      }
      [Test]
      public void CanCreateXmlTransmit()
      {
         Assert.IsNotNull(Pipelines.Xml.Send().End());
      }

      #region Send Pipeline Tests
      //
      // Send Pipeline Tests
      //

      [Test]
      public void Send_CanCreateEmptySend()
      {
         Assert.IsNotNull(Pipelines.Send().End());
      }
      [Test]
      public void Send_CanAddEncoder()
      {
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithEncoder(new MIME_SMIME_Encoder());
      }
      [Test]
      public void Send_CanAddAssembler()
      {
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithAssembler(new XmlAsmComp()).End();
      }
      [Test]
      public void Send_CanAddPreAssembler()
      {
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithPreAssembler(new MIME_SMIME_Encoder());
      }
      [Test]
      public void Send_WithCertificate()
      {
         string thumbprint =
            "e8 3e ff 40 69 03 58 17 59 2d 3b f8 f7 56 58 90 5d 59 03 2a";
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithCertificate(thumbprint);

         Assert.AreEqual(thumbprint, pipeline.GroupSigningCertificate);
      }
      [Test]
      public void Send_CanCreateWithTypeParam()
      {
         Assert.IsNotNull(Pipelines.Send(typeof(XMLTransmit)).End());
      }
      [Test]
      public void Send_CanCreateWithTypeGeneric()
      {
         Assert.IsNotNull(Pipelines.Send<XMLTransmit>().End());
      }
      #endregion // Send Pipeline Tests


      #region Receive Pipeline Tests
      //
      // Receive Pipeline Tests
      //
      [Test]
      public void CanCreateEmptyReceive()
      {
         Assert.IsNotNull(Pipelines.Receive().End());
      }
      [Test]
      public void Receive_CanAddDecoder()
      {
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDecoder(new MIME_SMIME_Decoder());
      }
      [Test]
      public void Receive_CanAddDisassembler()
      {
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDisassembler(new XmlDasmComp());
      }
      [Test]
      public void Receive_CanAddValidator()
      {
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithValidator(new XmlValidator());
      }
      [Test]
      public void Receive_CanAddPartyResolver()
      {
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithPartyResolver(new PartyRes());
      }
      [Test]
      public void Receive_WithCertificate()
      {
         string thumbprint = 
            "e8 3e ff 40 69 03 58 17 59 2d 3b f8 f7 56 58 90 5d 59 03 2a";
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithCertificate(thumbprint);

         Assert.AreEqual(thumbprint, pipeline.GroupSigningCertificate);
      }
      [Test]
      public void Receive_CanCreateWithTypeParam()
      {
         Assert.IsNotNull(Pipelines.Receive(typeof(XMLReceive)).End());
      }
      [Test]
      public void Receive_CanCreateWithTypeGeneric()
      {
         Assert.IsNotNull(Pipelines.Receive<XMLReceive>().End());
      }
      #endregion // Receive Pipeline Tests

   } // class PipelineCreationTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


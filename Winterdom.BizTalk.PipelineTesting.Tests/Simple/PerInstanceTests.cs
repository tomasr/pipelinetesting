
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting.Simple;
using SampleSchemas;

namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple {

   [TestFixture]
   public class PerInstanceTests {
      [Test]
      public void CanGetIndividualComponents() {
         SendPipelineWrapper pipeline = Pipelines.Xml.Send()
            .WithAssembler(Assembler.FlatFile())
            .WithEncoder(new MIME_SMIME_Encoder());

         Assert.IsInstanceOfType(typeof(XmlAsmComp), 
            pipeline.GetComponent(PipelineStage.Assemble, 0));
         Assert.IsInstanceOfType(typeof(FFAsmComp),
            pipeline.GetComponent(PipelineStage.Assemble, 1));
         Assert.IsInstanceOfType(typeof(MIME_SMIME_Encoder),
            pipeline.GetComponent(PipelineStage.Encode, 0));
      }

      [Test]
      public void CanApplyConfigToPipeline() {
         XmlTextReader reader = new XmlTextReader(
            DocLoader.LoadStream("PipelineInstanceConfig.xml")
         );
         SendPipelineWrapper pipeline = Pipelines.Xml.Send()
            .WithAssembler(Assembler.Xml())
            .WithEncoder(new MIME_SMIME_Encoder())
            .WithInstanceConfig(reader);

         XmlAsmComp xmlassm = (XmlAsmComp)
            pipeline.GetComponent(PipelineStage.Assemble, 0);
         Assert.IsFalse(xmlassm.AddXMLDeclaration);
         Assert.IsFalse(xmlassm.PreserveBom);

         MIME_SMIME_Encoder enc = (MIME_SMIME_Encoder)
            pipeline.GetComponent(PipelineStage.Encode, 0);
         Assert.IsTrue(enc.EnableEncryption);
         Assert.AreEqual(MIME_SMIME_Encoder.MIMETransferEncodingType.SevenBit, 
            enc.ContentTransferEncoding);
      }
      [Test]
      public void CanApplyConfigToPipelineDifferentFormatting() {
         // make sure XML formatting is not causing trouble
         XmlTextReader reader = new XmlTextReader(
            DocLoader.LoadStream("PipelineInstanceConfig2.xml")
         );
         SendPipelineWrapper pipeline = Pipelines.Xml.Send()
            .WithAssembler(Assembler.Xml())
            .WithEncoder(new MIME_SMIME_Encoder())
            .WithInstanceConfig(reader);

         XmlAsmComp xmlassm = (XmlAsmComp)
            pipeline.GetComponent(PipelineStage.Assemble, 0);
         Assert.IsFalse(xmlassm.AddXMLDeclaration);
         Assert.IsFalse(xmlassm.PreserveBom);

         MIME_SMIME_Encoder enc = (MIME_SMIME_Encoder)
            pipeline.GetComponent(PipelineStage.Encode, 0);
         Assert.IsTrue(enc.EnableEncryption);
         Assert.AreEqual(MIME_SMIME_Encoder.MIMETransferEncodingType.SevenBit, 
            enc.ContentTransferEncoding);
      }
   } // class StreamingTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


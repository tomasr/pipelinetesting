
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
   } // class StreamingTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


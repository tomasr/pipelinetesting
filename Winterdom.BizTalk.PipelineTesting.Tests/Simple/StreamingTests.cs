
//
// StreamingTests.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

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

   // This test verifies a potential bug
   // with reading a Stream returned by the XMLAssembler,
   // reported by Bram Veldhoen
   //
   // RESULT: Not a bug. XML Assembler throws a COM Exception
   // (E_FAIL) when trying to read stream if:
   // - Schema was not found
   [TestFixture]
   public class StreamingTests {
      [Test]
      public void CanReadXmlAssemblerStream() {
         SendPipelineWrapper pipeline = Pipelines.Xml.Send()
            .WithSpec<Schema3_FF>();
         IBaseMessage input = MessageHelper.CreateFromStream(
            DocLoader.LoadStream("CSV_XML_SendInput.xml")
            );
         IBaseMessage output = pipeline.Execute(input);
         Assert.IsNotNull(output);
         // doc should load fine
         XmlDocument doc = new XmlDocument();
         doc.Load(output.BodyPart.Data);
         XmlNodeList fields = doc.SelectNodes("//*[local-name()='Field3']");
         Assert.Greater(fields.Count, 0);
      }
   } // class StreamingTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


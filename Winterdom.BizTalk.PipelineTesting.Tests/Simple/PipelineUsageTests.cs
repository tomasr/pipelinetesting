
//
// PipelineUsageTests.cs
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
using SampleSchemas;

namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple
{
   [TestFixture]
   public class PipelineUsageTests
   {
      #region Send Pipeline Tests
      //
      // Send Pipeline Tests
      //

      [Test]
      public void Send_XmlAssemblerSchemasAddedToPipeline()
      {
         XmlAssembler xml = Assembler.Xml()
            .WithDocumentSpec<Schema1_NPP.Root>();
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithAssembler(xml);

         string name = typeof(Schema1_NPP.Root).AssemblyQualifiedName;
         Assert.IsNotNull(pipeline.GetKnownDocSpecByName(name));
      }

      [Test]
      public void Send_FFAssemblerSchemasAddedToPipeline()
      {
         FFAssembler ff = Assembler.FlatFile()
            .WithDocumentSpec<Schema3_FF>();
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithAssembler(ff);

         string name = typeof(Schema3_FF).AssemblyQualifiedName;
         Assert.IsNotNull(pipeline.GetKnownDocSpecByName(name));
      }

      [Test]
      public void Send_FullPipeline()
      {
         XmlAssembler xml = XmlAssembler.Xml()
            .WithDocumentSpec<SimpleBody>()
            .WithEnvelopeSpec<SimpleEnv>();
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithAssembler(xml)
            .WithEncoder(new MIME_SMIME_Encoder());

         // Create the input message to pass through the pipeline
         string body = 
            @"<o:Body xmlns:o='http://SampleSchemas.SimpleBody'>
               this is a body</o:Body>";
         // Execute the pipeline, and check the output
         // we get a single message batched with all the 
         // messages grouped into the envelope's body
         IBaseMessage outputMessage = pipeline.Execute(
            MessageHelper.CreateFromString(body),
            MessageHelper.CreateFromString(body),
            MessageHelper.CreateFromString(body)
            );
         Assert.IsNotNull(outputMessage);
      }

      #endregion // Send Pipeline Tests


      #region Receive Pipeline Tests
      //
      // Receive Pipeline Tests
      //

      [Test]
      public void Receive_XmlDisassemblerSchemasAddedToPipeline()
      {
         XmlDisassembler xml = Disassembler.Xml()
            .WithDocumentSpec<Schema1_NPP.Root>();
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDisassembler(xml);

         string name = typeof(Schema1_NPP.Root).AssemblyQualifiedName;
         Assert.IsNotNull(pipeline.GetKnownDocSpecByName(name));
      }

      [Test]
      public void Receive_FFDisassemblerSchemasAddedToPipeline()
      {
         FFDisassembler ff = Disassembler.FlatFile()
            .WithDocumentSpec<Schema3_FF>();
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDisassembler(ff);

         string name = typeof(Schema3_FF).AssemblyQualifiedName;
         Assert.IsNotNull(pipeline.GetKnownDocSpecByName(name));
      }

      [Test]
      public void Receive_FullPipeline()
      {
         FFDisassembler ff = Disassembler.FlatFile()
            .WithDocumentSpec<Schema3_FF>();
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDisassembler(ff);

         IBaseMessage input = MessageHelper.CreateFromStream(
            DocLoader.LoadStream("CSV_FF_RecvInput.txt")
            );

         MessageCollection output = pipeline.Execute(input);

         Assert.AreEqual(1, output.Count);
      }

      [Test]
      public void Receive_WithPromotedProps()
      {
         ReceivePipelineWrapper pipeline = Pipelines.Xml.Receive()
            .WithSpec<Schema2_WPP>();

         IBaseMessage input = MessageHelper.CreateFromStream(
            DocLoader.LoadStream("SampleDocument.xml")
            );
         MessageCollection output = pipeline.Execute(input);

         Property2 prop = new Property2();
         object value = output[0].Context.Read(prop.QName.Name, prop.QName.Namespace);
         Assert.AreEqual("Field2_0", value);
      }
      #endregion // Receive Pipeline Tests

   } // class PipelineCreationTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests.Simple


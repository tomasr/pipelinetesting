
//
// ReceivePipelineTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.IO;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;

using Winterdom.BizTalk.PipelineTesting;
using SampleSchemas;
using System.Collections.Generic;

namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   /// <summary>
   /// Tests for the ReceivePipelineWrapper class
   /// </summary>
   [TestFixture]
   public class ReceivePipelineTests
   {

      #region Component & DocSpecs Tests
      //
      // Component & DocSpecs Tests
      //

      /// <summary>
      /// Test we can add components
      /// at all stages into an empty receive
      /// pipeline
      /// </summary>
      [Test]
      public void CanAddComponentToValidStage()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         IBaseComponent component = new XmlDasmComp();
         pipeline.AddComponent(component, PipelineStage.Disassemble);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenAddingNullComponent()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddComponent(null, PipelineStage.ResolveParty);
      }
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenCompponentAddedToNullStage()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         IBaseComponent partyResolution = new PartyRes();
         pipeline.AddComponent(partyResolution, null);
      }

      /// <summary>
      /// Tests we throw an error when we attempt to
      /// add a component to a stage that should not be in 
      /// a receive pipeline
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ThrowExceptionWhenComponentAddedToInvalidStage()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         IBaseComponent partyResolution = new PartyRes();
         pipeline.AddComponent(partyResolution, PipelineStage.PreAssemble);
      }

      /// <summary>
      /// Tests we fault when we pass null to AddDocSpec();
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenNullDocSpecAdded()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddDocSpec(null);
      }

      /// <summary>
      /// Tests that we can when we add a new document spec
      /// with a single root, it is correctly added to the 
      /// pipeline context
      /// </summary>
      [Test]
      public void CanAddSingleRootDocSpec()
      {
         ReceivePipelineWrapper pipeline = 
            PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddDocSpec(typeof(Schema2_WPP));

         IDocumentSpec docSpec = null;

         docSpec = pipeline.GetKnownDocSpecByName(typeof(Schema2_WPP).AssemblyQualifiedName);
         Assert.IsNotNull(docSpec);

         docSpec = pipeline.GetKnownDocSpecByType("http://SampleSchemas.Schema2_WPP#Root");
         Assert.IsNotNull(docSpec);
      }

      /// <summary>
      /// Tests that when we add a new Document Spec,
      /// all roots are correctly added to the pipeline
      /// context, as well as the schema by name
      /// </summary>
      [Test]
      public void CanAddMultiRootDocSpec()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddDocSpec(typeof(Schema1_NPP));

         IDocumentSpec docSpec = null;

         docSpec = pipeline.GetKnownDocSpecByName(typeof(Schema1_NPP.Root).AssemblyQualifiedName);
         Assert.IsNotNull(docSpec);
         docSpec = pipeline.GetKnownDocSpecByName(typeof(Schema1_NPP.Root2).AssemblyQualifiedName);
         Assert.IsNotNull(docSpec);

         docSpec = pipeline.GetKnownDocSpecByType("http://SampleSchemas.Schema1_NPP#Root");
         Assert.IsNotNull(docSpec);
         docSpec = pipeline.GetKnownDocSpecByType("http://SampleSchemas.Schema1_NPP#Root2");
         Assert.IsNotNull(docSpec);
      }

      /// <summary>
      /// Tests that we can add a doc spec using its name and not the type
      /// </summary>
      [Test]
      public void CanAddDocSpecByName()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddDocSpec("SampleSchemas.Schema1_NPP+Root", "SampleSchemas");

         IDocumentSpec docSpec = 
            pipeline.GetKnownDocSpecByName(typeof(Schema1_NPP.Root).AssemblyQualifiedName);
         Assert.IsNotNull(docSpec);

         docSpec = pipeline.GetKnownDocSpecByType("http://SampleSchemas.Schema1_NPP#Root");
         Assert.IsNotNull(docSpec);
      }

      /// <summary>
      /// Tests that we can add a schema with no targetNamespace 
      /// and get the correct results
      /// </summary>
      [Test]
      public void CanAddNoTargetNSDocSpec() {
         ReceivePipelineWrapper pipeline =
            PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddDocSpec(typeof(NoNS));

         IDocumentSpec docSpec = pipeline.GetKnownDocSpecByType("Root");
         Assert.IsNotNull(docSpec);
      }
      #endregion // Component & DocSpecs Tests

      #region Execute Tests
      //
      // Execute Tests
      //

      /// <summary>
      /// Tests we fault when we try to execute
      /// the pipeline with null argument
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenInputMsgIsNull()
      {
         ReceivePipelineWrapper pipeline =
            PipelineFactory.CreateReceivePipeline(typeof(ReceivePipeline1));

         pipeline.Execute(null);
      }

      /// <summary>
      /// Tests that we can execute successfully a loaded pipeline
      /// </summary>
      [Test]
      public void CanExecuteLoadedPipeline()
      {
         ReceivePipelineWrapper pipeline = 
            PipelineFactory.CreateReceivePipeline(typeof(ReceivePipeline1));

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("SampleDocument.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream); 

         // Add the necessary schemas to the pipeline, so that
         // disassembling works
         pipeline.AddDocSpec(typeof(Schema1_NPP));
         pipeline.AddDocSpec(typeof(Schema2_WPP));

         // Execute the pipeline, and check the output
         MessageCollection outputMessages = pipeline.Execute(inputMessage);

         Assert.IsNotNull(outputMessages);
         Assert.IsTrue(outputMessages.Count > 0);
         // check we promoted properties correctly
         const string ns = "http://SampleSchemas.PropSchema1";
         Assert.IsTrue(PropertyExists(outputMessages[0], ns, "Property1"));
         Assert.IsTrue(PropertyExists(outputMessages[0], ns, "Property1"));
      }

      /// <summary>
      /// Tests that we can execute successfully an empty pipeline
      /// </summary>
      [Test]
      public void CanExecuteEmptyPipeline()
      {
         ReceivePipelineWrapper pipeline =
            PipelineFactory.CreateEmptyReceivePipeline();

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("SampleDocument.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);

         // Execute the pipeline, and check the output
         MessageCollection outputMessages = pipeline.Execute(inputMessage);

         Assert.IsNotNull(outputMessages);
         Assert.IsTrue(outputMessages.Count > 0);
      }

      /// <summary>
      /// Tests that we can execute successfully a loaded pipeline
      /// with a flat file as input
      /// </summary>
      [Test]
      public void CanExecutePipelineWithFlatFile()
      {
         ReceivePipelineWrapper pipeline =
            PipelineFactory.CreateReceivePipeline(typeof(CSV_FF_RecvPipeline));

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("CSV_FF_RecvInput.txt");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);
         inputMessage.BodyPart.Charset = "UTF-8";

         // Add the necessary schemas to the pipeline, so that
         // disassembling works
         pipeline.AddDocSpec(typeof(Schema3_FF));

         // Execute the pipeline, and check the output
         MessageCollection outputMessages = pipeline.Execute(inputMessage);

         Assert.IsNotNull(outputMessages);
         Assert.IsTrue(outputMessages.Count > 0);
      }

      /// <summary>
      /// Tests we can execute a receive pipeline OK
      /// while doing debatching with the XML
      /// Disassembler component
      /// </summary>
      [Test]
      public void CanExecutePipelineWithMultiMsgOutput()
      {
         ReceivePipelineWrapper pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         pipeline.AddComponent(new XmlDasmComp(), PipelineStage.Disassemble);

         pipeline.AddDocSpec(typeof(SimpleBody));
         pipeline.AddDocSpec(typeof(SimpleEnv));

         Stream stream = DocLoader.LoadStream("Env_Batch_Input.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);

         MessageCollection outputMessages = pipeline.Execute(inputMessage);
         Assert.IsNotNull(outputMessages);
         Assert.AreEqual(3, outputMessages.Count);
      }

      /// <summary>
      /// Tests that when the pipeline stages are executed,
      /// IPipelineContext.StageID reflects the current
      /// stage.
      /// </summary>
      [Test]
      public void CanProvideStageIDsInContext()
      {
         var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
         var stages = new List<Guid>();
         pipeline.AddComponent(new ReceiveStageTest(stages), PipelineStage.Decode);
         pipeline.AddComponent(new ReceiveStageTest(stages), PipelineStage.Disassemble);
         pipeline.AddComponent(new ReceiveStageTest(stages), PipelineStage.Validate);
         pipeline.AddComponent(new ReceiveStageTest(stages), PipelineStage.ResolveParty);

         var inputMessage = MessageHelper.CreateFromString("<sample/>");
         var outputMessages = pipeline.Execute(inputMessage);
         Assert.AreEqual(4, stages.Count);
         Assert.AreEqual(PipelineStage.Decode.ID, stages[0]);
         Assert.AreEqual(PipelineStage.Disassemble.ID, stages[1]);
         Assert.AreEqual(PipelineStage.Validate.ID, stages[2]);
         Assert.AreEqual(PipelineStage.ResolveParty.ID, stages[3]);
      }

      #endregion // Execute Tests
      
      #region Private Methods
      //
      // Private Methods
      //


      private bool PropertyExists(IBaseMessage msg, string ns, string name)
      {
         object val = msg.Context.Read(name, ns);
         return (val != null);
      }

      #endregion // Private Methods

   } // class ReceivePipelineTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

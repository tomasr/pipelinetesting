
//
// SendPipelineTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.IO;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;

using Winterdom.BizTalk.PipelineTesting;
using SampleSchemas;

namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   /// <summary>
   /// Tests for the SendPipelineWrapper class
   /// </summary>
   [TestFixture]
   public class SendPipelineTests
   {
      #region Component Tests
      //
      // Component Tests
      //

      /// <summary>
      /// Test we can add components
      /// at all stages into an empty send
      /// pipeline
      /// </summary>
      [Test]
      public void CanAddComponentToValidStage()
      {
         SendPipelineWrapper pipeline = PipelineFactory.CreateEmptySendPipeline();
         IBaseComponent encoder = new MIME_SMIME_Encoder();
         pipeline.AddComponent(encoder, PipelineStage.Encode);
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
         SendPipelineWrapper pipeline = PipelineFactory.CreateEmptySendPipeline();
         IBaseComponent encoder = new MIME_SMIME_Encoder();
         pipeline.AddComponent(encoder, PipelineStage.ResolveParty);
      }

      /// <summary>
      /// Tests we can correctly iterate over the pipeline
      /// to enumerate the components on it.
      /// </summary>
      [Test]
      public void CanEnumerateComponents()
      {
         SendPipelineWrapper pipeline = PipelineFactory.CreateEmptySendPipeline();
         pipeline.AddComponent(new XmlAsmComp(), PipelineStage.Assemble);
         pipeline.AddComponent(new MIME_SMIME_Encoder(), PipelineStage.Encode);
         pipeline.AddComponent(new FFAsmComp(), PipelineStage.Assemble);

         int numComponents = 0;
         foreach ( IBaseComponent component in pipeline )
         {
            numComponents++;
         }
         Assert.AreEqual(3, numComponents);
      }


      /// <summary>
      /// Tests we can set the signing certificate to use
      /// </summary>
      [Test]
      public void CanSetSigningCertificate()
      {
         SendPipelineWrapper pipeline = PipelineFactory.CreateEmptySendPipeline();
         pipeline.GroupSigningCertificate = "whatever";
         Assert.IsNotNull(pipeline.GroupSigningCertificate);
      }

      #endregion // Component Tests

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
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(XMLTransmit));

         pipeline.Execute((MessageCollection)null);
      }

      /// <summary>
      /// Tests we fault when we try to execute
      /// the pipeline with an empty collection
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ThrowExceptionWhenInputMsgCollectionIsEmpty()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(XMLTransmit));

         pipeline.Execute(new MessageCollection());
      }

      /// <summary>
      /// Tests we can execute a send pipeline OK.
      /// </summary>
      [Test]
      public void CanExecuteDefaultPipeline()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(XMLTransmit));

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("SampleDocument.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);

         // Add the necessary schemas to the pipeline, so that
         // disassembling works
         pipeline.AddDocSpec(typeof(Schema1_NPP));
         pipeline.AddDocSpec(typeof(Schema2_WPP));

         MessageCollection inputMessages = new MessageCollection();
         inputMessages.Add(inputMessage);

         // Execute the pipeline, and check the output
         IBaseMessage outputMessage = pipeline.Execute(inputMessages);

         Assert.IsNotNull(outputMessage);
      }

      /// <summary>
      /// Tests we can execute a send pipeline OK with an empty pipeline
      /// </summary>
      [Test]
      public void CanExecuteEmptyPipeline()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateEmptySendPipeline();

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("SampleDocument.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);

         MessageCollection inputMessages = new MessageCollection();
         inputMessages.Add(inputMessage);

         // Execute the pipeline, and check the output
         IBaseMessage outputMessage = pipeline.Execute(inputMessages);

         Assert.IsNotNull(outputMessage);
      }

      /// <summary>
      /// Tests we can execute a send pipeline OK
      /// generating a flat file as output
      /// </summary>
      [Test]
      public void CanExecutePipelineWithFlatFile()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(CSV_FF_SendPipeline));

         // Create the input message to pass through the pipeline
         Stream stream = DocLoader.LoadStream("CSV_XML_SendInput.xml");
         IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);
         inputMessage.BodyPart.Charset = "UTF-8";

         // Add the necessary schemas to the pipeline, so that
         // assembling works
         pipeline.AddDocSpec(typeof(Schema3_FF));

         MessageCollection inputMessages = new MessageCollection();
         inputMessages.Add(inputMessage);

         // Execute the pipeline, and check the output
         IBaseMessage outputMessage = pipeline.Execute(inputMessages);

         Assert.IsNotNull(outputMessage);
      }


      /// <summary>
      /// Tests we can execute a send pipeline with
      /// multiple input messages and an envelope
      /// </summary>
      [Test]
      public void CanExecutePipelineWithMultiInputMsgs()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(Env_SendPipeline));

         // Create the input message to pass through the pipeline
         string body = 
            @"<o:Body xmlns:o='http://SampleSchemas.SimpleBody'>
               this is a body</o:Body>";
         // Add the necessary schemas to the pipeline, so that
         // assembling works
         pipeline.AddDocSpec(typeof(SimpleBody));
         pipeline.AddDocSpec(typeof(SimpleEnv));

         // original code:
         // MessageCollection inputMessages = new MessageCollection();
         // inputMessages.Add(MessageHelper.CreateFromString(body));
         // inputMessages.Add(MessageHelper.CreateFromString(body));
         // inputMessages.Add(MessageHelper.CreateFromString(body));

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

      [Test]
      public void CanCreateTransaction()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateSendPipeline(typeof(XMLTransmit));

         using ( TransactionControl control = pipeline.EnableTransactions() )
         {
            // Create the input message to pass through the pipeline
            Stream stream = DocLoader.LoadStream("SampleDocument.xml");
            IBaseMessage inputMessage = MessageHelper.CreateFromStream(stream);

            // Add the necessary schemas to the pipeline, so that
            // disassembling works
            pipeline.AddDocSpec(typeof(Schema1_NPP));
            pipeline.AddDocSpec(typeof(Schema2_WPP));

            MessageCollection inputMessages = new MessageCollection();
            inputMessages.Add(inputMessage);

            // Execute the pipeline, and check the output
            IBaseMessage outputMessage = pipeline.Execute(inputMessages);

            Assert.IsNotNull(outputMessage);
            control.SetComplete();
         }
      }

      #endregion // Execute Tests

      #region BTF Assembler Tests

      public void CanExecuteBtfAssembler()
      {
         SendPipelineWrapper pipeline =
            PipelineFactory.CreateEmptySendPipeline();
         pipeline.GroupSigningCertificate = "9302859B216AB1E97A2EB4F94E894A128E4A3B6E";

         MIME_SMIME_Encoder mime = new MIME_SMIME_Encoder();
         mime.SignatureType = MIME_SMIME_Encoder.SMIME_SignatureType.BlobSign;
         mime.SendBodyPartAsAttachment = true;
         mime.AddSigningCertToMessage = true;
         mime.EnableEncryption = false;
         mime.ContentTransferEncoding = MIME_SMIME_Encoder.MIMETransferEncodingType.SevenBit;
         pipeline.AddComponent(mime, PipelineStage.Encode);

         BTFAsmComp asm = new BTFAsmComp();
         asm.DesignProp_epsFromAddress = "asdasd";
         asm.DesignProp_epsFromAddressType = "asdad";
         asm.DesignProp_epsToAddress = "eweww";
         asm.DesignProp_epsToAddressType = " asdd";
         asm.DesignProp_isReliable = true;
         asm.DesignProp_propTopic = "wewew";
         asm.DesignProp_svcDeliveryRctRqtSendBy = 4;
         asm.DesignProp_svcDeliveryRctRqtSendToAddress = "ddd";
         asm.DesignProp_svcDeliveryRctRqtSendToAddressType = "sss";

         pipeline.AddComponent(asm, PipelineStage.Assemble);
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_endpoints_header));
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_envelope));
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_manifest_header));
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_process_header));
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_receipt_header));
         pipeline.AddDocSpec(typeof(BTF2Schemas.btf2_services_header));
         pipeline.AddDocSpec(typeof(SampleSchemas.SimpleBody));

         string body =
            @"<o:Body xmlns:o='http://SampleSchemas.SimpleBody'>
               this is a body</o:Body>";
         MessageCollection inputMessages = new MessageCollection();
         IBaseMessage inputMsg = MessageHelper.CreateFromString(body);
         inputMsg.Context.Write("PassThroughBTF", "http://schemas.microsoft.com/BizTalk/2003/mime-properties", false);
         inputMessages.Add(inputMsg);
         inputMsg.BodyPart.PartProperties.Write("ContentTransferEncoding", "http://schemas.microsoft.com/BizTalk/2003/mime-properties", "7bit");
         IBaseMessage output = pipeline.Execute(inputMessages);

         byte[] buffer = new byte[64 * 1024];
         Stream input = output.BodyPart.Data;
         int bytesRead;

         Stream outputs = new FileStream("c:\\temp\\t.xml", 
            FileMode.Truncate, FileAccess.Write);
         using ( outputs )
         {
            while ( (bytesRead = input.Read(buffer, 0, buffer.Length)) > 0 )
               outputs.Write(buffer, 0, bytesRead);
         }

      }
      #endregion // BTF Assembler tests

   } // class SendPipelineTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

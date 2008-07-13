
//
// PipelineFactoryTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using NUnit.Framework;

using Winterdom.BizTalk.PipelineTesting;

namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   /// <summary>
   /// Tests for the PipelineFactory class
   /// </summary>
   [TestFixture]
   public class PipelineFactoryTests
   {
      /// <summary>
      /// Tests that we can create an empty receive pipeline
      /// successfully.
      /// </summary>
      [Test]
      public void CanCreateEmptyReceivePipeline()
      {
         ReceivePipelineWrapper receivePipeline = 
            PipelineFactory.CreateEmptyReceivePipeline();
         
         Assert.IsNotNull(receivePipeline);
      }

      /// <summary>
      /// Tests we can create a new empty send pipeline successfully
      /// </summary>
      [Test]
      public void CanCreateEmptySendPipeline()
      {
         SendPipelineWrapper sendPipeline =
            PipelineFactory.CreateEmptySendPipeline();

         Assert.IsNotNull(sendPipeline);
      }

      /// <summary>
      /// Tests we can create a new receive pipeline from
      /// a biztalk pipeline type
      /// </summary>
      [Test]
      public void CanCreateReceivePipelineFromType()
      {
         ReceivePipelineWrapper receivePipeline =
            PipelineFactory.CreateReceivePipeline(typeof(XMLReceive));

         Assert.IsNotNull(receivePipeline);
      }

      /// <summary>
      /// Tests we fault when we try to create
      /// a receive pipeline and pass in null as the value
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenNullReceivePipelineCreated()
      {
         PipelineFactory.CreateReceivePipeline(null);
      }

      /// <summary>
      /// Tests we fault when we try to create
      /// a receive pipeline with a send pipeline type
      /// </summary>
      [Test]
      [ExpectedException(typeof(InvalidOperationException))]
      public void ThrowExceptionWhenInvalidTypeForReceivePipeline()
      {
         PipelineFactory.CreateReceivePipeline(typeof(XMLTransmit));
      }

      /// <summary>
      /// Tests we fault when we try to create
      /// a send pipeline and pass in null as the value
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenNullSendPipelineCreated()
      {
         PipelineFactory.CreateSendPipeline(null);
      }

      /// <summary>
      /// Tests we can create a new Send pipeline from
      /// a biztalk pipeline type
      /// </summary>
      [Test]
      public void CanCreateSendPipelineFromType()
      {
         SendPipelineWrapper sendPipeline =
            PipelineFactory.CreateSendPipeline(typeof(XMLTransmit));

         Assert.IsNotNull(sendPipeline);
      }

      /// <summary>
      /// Tests we fault when we try to create
      /// a send pipeline with a receive pipeline type
      /// </summary>
      [Test]
      [ExpectedException(typeof(InvalidOperationException))]
      public void ThrowExceptionWhenInvalidTypeForSendPipeline()
      {
         PipelineFactory.CreateSendPipeline(typeof(XMLReceive));
      }


   } // class PipelineFactoryTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

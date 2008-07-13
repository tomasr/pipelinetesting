
//
// PipelineFactory.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
//using Microsoft.Test.BizTalk.PipelineObjects;

using IPipeline = Microsoft.Test.BizTalk.PipelineObjects.IPipeline;
using PipelineHelper = Microsoft.Test.BizTalk.PipelineObjects.PipelineFactory;

namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Factory class to create pipeline objects. 
   /// </summary>
   /// <remarks>
   /// This class can create either empty (with no components)
   /// Receive and Send pipelines, as well as initialize pipeline objects
   /// from the corresponding types in compiled BizTalk assemblies.
   /// </remarks>
   public static class PipelineFactory
   {

      #region Receive Pipeline Methods
      //
      // Receive Pipeline Methods
      //

      /// <summary>
      /// Creates a receive pipeline with no components
      /// on any stage
      /// </summary>
      /// <returns>An empty receive pipeline</returns>
      public static ReceivePipelineWrapper CreateEmptyReceivePipeline()
      {
         PipelineHelper helper = new PipelineHelper();
         IPipeline pipeline = helper.CreateReceivePipeline();
         return new ReceivePipelineWrapper(pipeline);
      }

      /// <summary>
      /// Creates a receive pipeline from a given biztalk
      /// pipeline type
      /// </summary>
      /// <param name="type">BizTalk pipeline type</param>
      /// <returns>A receive Pipeline</returns>
      public static ReceivePipelineWrapper CreateReceivePipeline(Type type)
      {
         if ( type == null )
            throw new ArgumentNullException("type");

         if ( !type.IsSubclassOf(typeof(ReceivePipeline)) )
            throw new InvalidOperationException("Type must specify a Receive Pipeline");

         PipelineHelper helper = new PipelineHelper();
         IPipeline pipeline = helper.CreatePipelineFromType(type);
         return new ReceivePipelineWrapper(pipeline);
      }

      #endregion // Receive Pipeline Methods


      #region Send Pipeline Methods
      //
      // Send Pipeline Methods
      //

      /// <summary>
      /// Creates a send pipeline with no components
      /// on any stage
      /// </summary>
      /// <returns>An empty send pipeline</returns>
      public static SendPipelineWrapper CreateEmptySendPipeline()
      {
         PipelineHelper helper = new PipelineHelper();
         IPipeline pipeline = helper.CreateSendPipeline();
         return new SendPipelineWrapper(pipeline);
      }


      /// <summary>
      /// Creates a send pipeline from a given biztalk
      /// pipeline type
      /// </summary>
      /// <param name="type">BizTalk pipeline type</param>
      /// <returns>A send Pipeline</returns>
      public static SendPipelineWrapper CreateSendPipeline(Type type)
      {
         if ( type == null )
            throw new ArgumentNullException("type");

         if ( !type.IsSubclassOf(typeof(SendPipeline)) )
            throw new InvalidOperationException("Type must specify a Send Pipeline");

         PipelineHelper helper = new PipelineHelper();
         IPipeline pipeline = helper.CreatePipelineFromType(type);
         return new SendPipelineWrapper(pipeline);
      }

      #endregion // Send Pipeline Methods

   } // class PipelineFactory

} // namespace Winterdom.BizTalk.PipelineTesting

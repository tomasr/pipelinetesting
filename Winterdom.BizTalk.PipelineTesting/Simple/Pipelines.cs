
//
// Pipelines.cs
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
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.DefaultPipelines;

using IPipeline = Microsoft.Test.BizTalk.PipelineObjects.IPipeline;
using PipelineHelper = Microsoft.Test.BizTalk.PipelineObjects.PipelineFactory;

namespace Winterdom.BizTalk.PipelineTesting.Simple
{
   /// <summary>
   /// Simple interface for creating pipelines
   /// </summary>
   public static class Pipelines
   {
      private static XmlPipelines _xmlPipelines = new XmlPipelines();

      /// <summary>
      /// Standard BizTalk XML pipelines
      /// </summary>
      public static XmlPipelines Xml
      {
         get { return _xmlPipelines; }
      }

      /// <summary>
      /// Prepares a new Send pipeline for construction
      /// </summary>
      /// <returns>A pipeline builder</returns>
      public static SendPipelineBuilder Send()
      {
         return new SendPipelineBuilder();
      }
      
      /// <summary>
      /// Creates a new send pipeline with an explicit type
      /// </summary>
      /// <param name="type">The pipeline type</param>
      /// <returns>A pipeline builder</returns>
      public static SendPipelineBuilder Send(Type type)
      {
         return new SendPipelineBuilder(type);
      }

      /// <summary>
      /// Creates a new send pipeline with an explicit type
      /// as a generic parameter
      /// </summary>
      /// <typeparam name="T">The pipeline type</typeparam>
      /// <returns>A pipeline builder</returns>
      public static SendPipelineBuilder Send<T>()
      {
         return new SendPipelineBuilder(typeof(T));
      }

      /// <summary>
      /// Prepares a new Receive pipeline for construction
      /// </summary>
      /// <returns>A pipeline builder</returns>
      public static ReceivePipelineBuilder Receive()
      {
         return new ReceivePipelineBuilder();
      }

      /// <summary>
      /// Creates a new receive pipeline with an explicit
      /// type parameter
      /// </summary>
      /// <param name="type">The pipeline type</param>
      /// <returns>A pipeline Builder</returns>
      public static ReceivePipelineBuilder Receive(Type type)
      {
         return new ReceivePipelineBuilder(type);
      }

      /// <summary>
      /// Creates a new receive pipeline with an explicit
      /// generic type parameter
      /// </summary>
      /// <typeparam name="T">The pipeline type</typeparam>
      /// <returns>A pipeline builder</returns>
      public static ReceivePipelineBuilder Receive<T>()
      {
         return new ReceivePipelineBuilder(typeof(T));
      }
   } // class Pipelines

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


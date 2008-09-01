
//
// SendPipelineBuilder.cs
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
   /// Constructs a new SendPipelineWrapper object
   /// </summary>
   public class SendPipelineBuilder
   {
      private SendPipelineWrapper _pipeline;

      internal SendPipelineBuilder()
      {
         _pipeline = PipelineFactory.CreateEmptySendPipeline();
      }
      internal SendPipelineBuilder(Type type)
      {
         _pipeline = PipelineFactory.CreateSendPipeline(type);
      }

      /// <summary>
      /// Finishes construction
      /// </summary>
      /// <returns>The pipeline wrapper object</returns>
      public SendPipelineWrapper End()
      {
         return _pipeline;
      }

      /// <summary>
      /// Converts this builder to the actual pipeline
      /// wrapper object; making it unnecessary to call
      /// End() directly.
      /// </summary>
      /// <param name="builder">The builder object</param>
      /// <returns>The pipeline wrapper object</returns>
      public static implicit operator SendPipelineWrapper(SendPipelineBuilder builder)
      {
         return builder.End();
      }

      /// <summary>
      /// Adds a pipeline component to the Encode stage
      /// </summary>
      /// <param name="encoder">The component to add</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithEncoder(IBaseComponent encoder)
      {
         if ( encoder == null )
            throw new ArgumentNullException("encoder");
         _pipeline.AddComponent(encoder, PipelineStage.Encode);
         return this;
      }
      
      /// <summary>
      /// Adds a pipeline component to the Assemble stage
      /// </summary>
      /// <param name="assembler">The component to add</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithAssembler(IBaseComponent assembler)
      {
         if ( assembler == null )
            throw new ArgumentNullException("assembler");
         _pipeline.AddComponent(assembler, PipelineStage.Assemble);
         return this;
      }
      
      /// <summary>
      /// Adds an Assembler component to the pipeline
      /// </summary>
      /// <param name="assembler">The component to add</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithAssembler(Assembler assembler)
      {
         if ( assembler == null )
            throw new ArgumentNullException("assembler");
         WithAssembler(assembler.End());

         foreach ( Type schemaType in assembler.KnownSchemas )
            _pipeline.AddDocSpec(schemaType);

         return this;
      }
      
      /// <summary>
      /// Adds a pipeline component to the preassemble stage
      /// </summary>
      /// <param name="preAssembler">The compononent to add</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithPreAssembler(IBaseComponent preAssembler)
      {
         if ( preAssembler == null )
            throw new ArgumentNullException("preAssembler");
         _pipeline.AddComponent(preAssembler, PipelineStage.PreAssemble);
         return this;
      }

      /// <summary>
      /// Adds a document specification (schema) to the pipeline
      /// </summary>
      /// <param name="docSpecType">The schema type</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithSpec(Type docSpecType)
      {
         _pipeline.AddDocSpec(docSpecType);
         return this;
      }

      /// <summary>
      /// Adds a document specification (schema) to the pipeline
      /// </summary>
      /// <param name="typeName">The fully qualified (namespace.class) name of 
      /// the schema</param>
      /// <param name="assemblyName">The partial or full name of the assembly
      /// containing the schema</param>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithSpec(string typeName, string assemblyName)
      {
         _pipeline.AddDocSpec(typeName, assemblyName);
         return this;
      }

      /// <summary>
      /// Adds a document specification (schema) to the pipeline
      /// </summary>
      /// <typeparam name="T">The schema type</typeparam>
      /// <returns>This instance</returns>
      public SendPipelineBuilder WithSpec<T>()
      {
         _pipeline.AddDocSpec(typeof(T));
         return this;
      }

      /// <summary>
      /// Configures the pipeline with a Group Signing Certificate
      /// </summary>
      /// <param name="certificate">The certificate thumbprint</param>
      /// <returns>This instance </returns>
      public SendPipelineBuilder WithCertificate(string certificate)
      {
         _pipeline.GroupSigningCertificate = certificate;
         return this;
      }

   } // class SendPipelineBuilder

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


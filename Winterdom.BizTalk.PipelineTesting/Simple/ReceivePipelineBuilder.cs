
//
// ReceivePipelineBuilder.cs
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
   /// Constructs a new ReceivePipelineWrapper object
   /// </summary>
   public class ReceivePipelineBuilder
   {
      private ReceivePipelineWrapper _pipeline;

      internal ReceivePipelineBuilder()
      {
         _pipeline = PipelineFactory.CreateEmptyReceivePipeline();
      }
      internal ReceivePipelineBuilder(Type type)
      {
         _pipeline = PipelineFactory.CreateReceivePipeline(type);
      }

      /// <summary>
      /// Finishes construction
      /// </summary>
      /// <returns>The pipeline wrapper object</returns>
      public ReceivePipelineWrapper End()
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
      public static implicit operator ReceivePipelineWrapper(ReceivePipelineBuilder builder)
      {
         return builder.End();
      }

      /// <summary>
      /// Adds a pipeline component to the decode stage
      /// </summary>
      /// <param name="decoder">The component to add</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithDecoder(IBaseComponent decoder)
      {
         if ( decoder == null )
            throw new ArgumentNullException("decoder");
         _pipeline.AddComponent(decoder, PipelineStage.Decode);
         return this;
      }

      /// <summary>
      /// Adds a pipeline component to the disassembling stage
      /// </summary>
      /// <param name="disassembler">The component to add</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithDisassembler(IBaseComponent disassembler)
      {
         if ( disassembler == null )
            throw new ArgumentNullException("disassembler");
         _pipeline.AddComponent(disassembler, PipelineStage.Disassemble);
         return this;
      }

      /// <summary>
      /// Adds a pipeline component to the disassembling stage
      /// </summary>
      /// <param name="disassembler">The component to add</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithDisassembler(Disassembler disassembler)
      {
         if ( disassembler == null )
            throw new ArgumentNullException("disassembler");
         _pipeline.AddComponent(disassembler.End(), PipelineStage.Disassemble);
         
         foreach ( Type schemaType in disassembler.KnownSchemas )
            _pipeline.AddDocSpec(schemaType);
         
         return this;
      }
      /// <summary>
      /// Adds a pipeline component to the validating stage
      /// </summary>
      /// <param name="validator">The component to add</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithValidator(IBaseComponent validator)
      {
         if ( validator == null )
            throw new ArgumentNullException("validator");
         _pipeline.AddComponent(validator, PipelineStage.Validate);
         return this;
      }

      /// <summary>
      /// Adds a pipeline component to the party resolution stage
      /// </summary>
      /// <param name="resolver">The component to add</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithPartyResolver(IBaseComponent resolver)
      {
         if ( resolver == null )
            throw new ArgumentNullException("resolver");
         _pipeline.AddComponent(resolver, PipelineStage.ResolveParty);
         return this;
      }

      /// <summary>
      /// Adds a document specification (schema) to the pipeline
      /// </summary>
      /// <param name="docSpecType">The schema type</param>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithSpec(Type docSpecType)
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
      public ReceivePipelineBuilder WithSpec(string typeName, string assemblyName)
      {
         _pipeline.AddDocSpec(typeName, assemblyName);
         return this;
      }

      /// <summary>
      /// Adds a document specification (schema) to the pipeline
      /// </summary>
      /// <typeparam name="T">The schema type</typeparam>
      /// <returns>This instance</returns>
      public ReceivePipelineBuilder WithSpec<T>()
      {
         _pipeline.AddDocSpec(typeof(T));
         return this;
      }

      /// <summary>
      /// Configures the pipeline with a Group Signing Certificate
      /// </summary>
      /// <param name="certificate">The certificate thumbprint</param>
      /// <returns>This instance </returns>
      public ReceivePipelineBuilder WithCertificate(string certificate)
      {
         _pipeline.GroupSigningCertificate = certificate;
         return this;
      }

   } // class ReceivePipelineBuilder

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


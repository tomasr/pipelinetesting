
//
// Assembler.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component.Utilities;

namespace Winterdom.BizTalk.PipelineTesting.Simple
{
   /// <summary>
   /// Factory class for standard assembler components
   /// </summary>
   /// <remarks>
   /// The Assembler class makes it easy to create standard
   /// assembler components and configure them with the proper
   /// document specifications (schema) and other properties before
   /// adding them to an empty pipeline; without having to reference
   /// assemblies like Microsoft.BizTalk.Component.Utilities, which
   /// are only located in the GAC.
   /// </remarks>
   /// <example><![CDATA[
   /// XmlAssembler xml = Assembler.Xml()
   ///   .WithDocumentSpec<MySchema>()
   ///   .WithPreserveBom(true);
   /// //...
   /// SendPipelineWrapper pipeline = Pipelines.Send().WithAssembler(xml);
   /// ]]>
   /// </example>
   public abstract class Assembler
   {
      private IList<Type> _knownSchemas = new List<Type>();

      internal IList<Type> KnownSchemas
      {
         get { return _knownSchemas; }
      }

      /// <summary>
      /// Creates a new XML Assembler component
      /// </summary>
      /// <returns>The Xml assembler</returns>
      public static XmlAssembler Xml()
      {
         return new XmlAssembler();
      }

      /// <summary>
      /// Creates a new Flat File Assembler component
      /// </summary>
      /// <returns>The flat file assembler</returns>
      public static FFAssembler FlatFile()
      {
         return new FFAssembler();
      }

      /// <summary>
      /// Returns the actual pipeline component
      /// created once you've configured it as necessary
      /// </summary>
      /// <returns>The IBaseComponent object</returns>
      public abstract IBaseComponent End();

      /// <summary>
      /// Adds a Schema to the list of known 
      /// schemas by this component. Schemas added here
      /// will be passed to the pipeline context
      /// </summary>
      /// <param name="schemaType">Schema to add</param>
      protected void AddSchema(Type schemaType)
      {
         _knownSchemas.Add(schemaType);
      }

   } // class Assembler

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


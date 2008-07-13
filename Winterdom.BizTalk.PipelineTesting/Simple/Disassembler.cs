
//
// Disassembler.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
using System;
using System.Collections;
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
   /// Factory class for standard disassembler components
   /// </summary>
   /// <remarks>
   /// The Disassembler class makes it easy to create standard
   /// disassembler components and configure them with the proper
   /// document specifications (schema) and other properties before
   /// adding them to an empty pipeline; without having to reference
   /// assemblies like Microsoft.BizTalk.Component.Utilities, which
   /// are only located in the GAC.
   /// </remarks>
   /// <example><![CDATA[
   /// XmlDisassembler xml = Disassembler.Xml()
   ///   .WithDocumentSpec<MySchema>()
   ///   .WithValidation(true);
   /// //...
   /// ReceivePipelineWrapper pipeline = Pipelines.Receive().WithDisassembler(xml);
   /// ]]>
   /// </example>
   public abstract class Disassembler
   {
      private IList<Type> _knownSchemas = new List<Type>();

      internal IList<Type> KnownSchemas
      {
         get { return _knownSchemas; }
      }

      /// <summary>
      /// Creates a new XML Disassembler component
      /// </summary>
      /// <returns>The Xml disassembler</returns>
      public static XmlDisassembler Xml()
      {
         return new XmlDisassembler();
      }

      /// <summary>
      /// Creates a new Flat File Disassembler component
      /// </summary>
      /// <returns>The flat file disassembler</returns>
      public static FFDisassembler FlatFile()
      {
         return new FFDisassembler();
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

   } // class Disassembler

} // namespace Winterdom.BizTalk.PipelineTesting.Simple



//
// XmlDisassembler.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
using System;
using System.Collections;
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
   /// Creates and configures an XML disassembler component.
   /// </summary>
   public class XmlDisassembler : Disassembler
   {
      private XmlDasmComp _disassembler;

      internal XmlDisassembler()
      {
         _disassembler = new XmlDasmComp();
      }

      /// <summary>
      /// Adds a new Document Specification (Schema) to the 
      /// component configuration
      /// </summary>
      /// <param name="schemaType">The BizTalk schema type</param>
      /// <returns>This instance</returns>
      public XmlDisassembler WithDocumentSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName; 
         _disassembler.DocumentSpecNames.Add(new Schema(name));
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Adds a new Document Specification (Schema) to the 
      /// component configuration
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema Type</typeparam>
      /// <returns>This instance</returns>
      public XmlDisassembler WithDocumentSpec<T>()
      {
         return WithDocumentSpec(typeof(T));
      }

      /// <summary>
      /// Adds an new Envelope Specification (Schema) to the
      /// component configuration
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public XmlDisassembler WithEnvelopeSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _disassembler.EnvelopeSpecNames.Add(new Schema(name));
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Adds an new Envelope Specification (Schema) to the
      /// component configuration
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public XmlDisassembler WithEnvelopeSpec<T>()
      {
         return WithEnvelopeSpec(typeof(T));
      }

      /// <summary>
      /// Configures the component to validate the incoming
      /// document against the schema
      /// </summary>
      /// <param name="validateMessage">If true, validate the message</param>
      /// <returns>This instance</returns>
      public XmlDisassembler WithValidation(bool validateMessage)
      {
         _disassembler.ValidateDocument = validateMessage;
         return this;
      }

      /// <summary>
      /// Configures the component to use recoverable 
      /// interchange processing
      /// </summary>
      /// <param name="withRip">If true, do a recoverable interchange</param>
      /// <returns>This instance</returns>
      public XmlDisassembler WithRecoverableInterchange(bool withRip)
      {
         _disassembler.RecoverableInterchangeProcessing = withRip;
         return this;
      }

      /// <summary>
      /// Configures the component to allow unrecognized messages
      /// </summary>
      /// <param name="allow">If true, allows messages without schema</param>
      /// <returns>This instance</returns>
      public XmlDisassembler AllowUnrecognized(bool allow)
      {
         _disassembler.AllowUnrecognizedMessage = allow;
         return this;
      }

      /// <summary>
      /// Return the constructed component
      /// </summary>
      /// <returns>The XmlDasmComp component</returns>
      public override IBaseComponent End()
      {
         return _disassembler;
      }

   } // class XmlDisassembler

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


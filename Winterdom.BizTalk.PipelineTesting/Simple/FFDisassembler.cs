
//
// FFDisassembler.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

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
   /// Creates and configures a Flat File disassembler component
   /// </summary>
   public class FFDisassembler : Disassembler
   {
      private FFDasmComp _disassembler;

      internal FFDisassembler()
      {
         _disassembler = new FFDasmComp();
      }

      /// <summary>
      /// Sets the document specification (Schema) to use for 
      /// assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema Type</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithDocumentSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _disassembler.DocumentSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the document specification (Schema) to use for 
      /// assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFDisassembler WithDocumentSpec<T>()
      {
         return WithDocumentSpec(typeof(T));
      }

      /// <summary>
      /// Sets the Header schema to use for assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithHeaderSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _disassembler.HeaderSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the Header schema to use for assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFDisassembler WithHeaderSpec<T>()
      {
         return WithHeaderSpec(typeof(T));
      }

      /// <summary>
      /// Sets the footer schema to use when assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithTrailerSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _disassembler.TrailerSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the footer schema to use when assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFDisassembler WithTrailerSpec<T>()
      {
         return WithTrailerSpec(typeof(T));
      }

      /// <summary>
      /// Configures the component to validate the incoming
      /// document against the schema
      /// </summary>
      /// <param name="validateMessage">If true, validate the message</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithValidation(bool validateMessage)
      {
         _disassembler.ValidateDocumentStructure = validateMessage;
         return this;
      }

      /// <summary>
      /// Configures the component to use recoverable 
      /// interchange processing
      /// </summary>
      /// <param name="withRip">If true, do a recoverable interchange</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithRecoverableInterchange(bool withRip)
      {
         _disassembler.RecoverableInterchangeProcessing = withRip;
         return this;
      }

      /// <summary>
      /// Configures the component to preserve the message header
      /// </summary>
      /// <param name="preserveHeader">If true, preserve the message header</param>
      /// <returns>This instance</returns>
      public FFDisassembler WithPreserveHeader(bool preserveHeader)
      {
         _disassembler.PreserveHeader = preserveHeader;
         return this;
      }

      /// <summary>
      /// Returns the constructed component
      /// </summary>
      /// <returns>The Flat File assembler</returns>
      public override IBaseComponent End()
      {
         return _disassembler;
      }

   } // class FFDisassembler


} // namespace Winterdom.BizTalk.PipelineTesting.Simple


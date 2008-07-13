
//
// FFAssembler.cs
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
   /// Creates and configures a Flat File Assembler component
   /// </summary>
   public class FFAssembler : Assembler
   {
      private FFAsmComp _assembler;

      internal FFAssembler()
      {
         _assembler = new FFAsmComp();
      }

      /// <summary>
      /// Sets the document specification (Schema) to use for 
      /// assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema Type</param>
      /// <returns>This instance</returns>
      public FFAssembler WithDocumentSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _assembler.DocumentSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the document specification (Schema) to use for 
      /// assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFAssembler WithDocumentSpec<T>()
      {
         return WithDocumentSpec(typeof(T));
      }

      /// <summary>
      /// Sets the Header schema to use for assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public FFAssembler WithHeaderSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _assembler.HeaderSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the Header schema to use for assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFAssembler WithHeaderSpec<T>()
      {
         return WithHeaderSpec(typeof(T));
      }

      /// <summary>
      /// Sets the footer schema to use when assembling
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public FFAssembler WithTrailerSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _assembler.TrailerSpecName = new SchemaWithNone(name);
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Sets the footer schema to use when assembling
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public FFAssembler WithTrailerSpec<T>()
      {
         return WithTrailerSpec(typeof(T));
      }

      /// <summary>
      /// Configures whether to put a Byte-Order Mark (BOM)
      /// on the resulting message
      /// </summary>
      /// <param name="preserveBom">If true, add a BOM</param>
      /// <returns>This instance</returns>
      public FFAssembler WithPreserveBom(bool preserveBom)
      {
         _assembler.PreserveBom = preserveBom;
         return this;
      }

      /// <summary>
      /// Returns the constructed component
      /// </summary>
      /// <returns>The Flat File assembler</returns>
      public override IBaseComponent End()
      {
         return _assembler;
      }

   } // class FFAssembler


} // namespace Winterdom.BizTalk.PipelineTesting.Simple


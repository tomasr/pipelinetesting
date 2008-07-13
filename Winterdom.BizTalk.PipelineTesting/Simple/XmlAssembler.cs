
//
// XmlAssembler.cs
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
   /// Creates and configures an XML assembler component.
   /// </summary>
   public class XmlAssembler : Assembler
   {
      private XmlAsmComp _assembler;

      internal XmlAssembler()
      {
         _assembler = new XmlAsmComp();
      }

      /// <summary>
      /// Adds a new Document Specification (Schema) to the 
      /// component configuration
      /// </summary>
      /// <param name="schemaType">The BizTalk schema type</param>
      /// <returns>This instance</returns>
      public XmlAssembler WithDocumentSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName; 
         _assembler.DocumentSpecNames.Add(new Schema(name));
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Adds a new Document Specification (Schema) to the 
      /// component configuration
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema Type</typeparam>
      /// <returns>This instance</returns>
      public XmlAssembler WithDocumentSpec<T>()
      {
         return WithDocumentSpec(typeof(T));
      }

      /// <summary>
      /// Adds an new Envelope Specification (Schema) to the
      /// component configuration
      /// </summary>
      /// <param name="schemaType">The BizTalk Schema type</param>
      /// <returns>This instance</returns>
      public XmlAssembler WithEnvelopeSpec(Type schemaType)
      {
         string name = schemaType.AssemblyQualifiedName;
         _assembler.EnvelopeDocSpecNames.Add(new Schema(name));
         AddSchema(schemaType);
         return this;
      }
      /// <summary>
      /// Adds an new Envelope Specification (Schema) to the
      /// component configuration
      /// </summary>
      /// <typeparam name="T">The BizTalk Schema type</typeparam>
      /// <returns>This instance</returns>
      public XmlAssembler WithEnvelopeSpec<T>()
      {
         return WithEnvelopeSpec(typeof(T));
      }

      /// <summary>
      /// Configures the component to add the XML declaration
      /// to the resulting message
      /// </summary>
      /// <param name="addXmlDeclaration">If true, add the XML declaration</param>
      /// <returns>This instance</returns>
      public XmlAssembler WithXmlDeclaration(bool addXmlDeclaration)
      {
         _assembler.AddXMLDeclaration = addXmlDeclaration;
         return this;
      }

      /// <summary>
      /// Configures the component to preserve a Byte Order Mark (BOM)
      /// in the outgoing message
      /// </summary>
      /// <param name="preserveBom">If true, add a BOM</param>
      /// <returns>This instance</returns>
      public XmlAssembler WithPreserveBom(bool preserveBom)
      {
         _assembler.PreserveBom = preserveBom;
         return this;
      }

      /// <summary>
      /// Return the constructed component
      /// </summary>
      /// <returns>The XmlAsmComp component</returns>
      public override IBaseComponent End()
      {
         return _assembler;
      }

   } // class XmlAssembler

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


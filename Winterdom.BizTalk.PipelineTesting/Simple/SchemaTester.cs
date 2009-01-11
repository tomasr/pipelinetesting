using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;

namespace Winterdom.BizTalk.PipelineTesting.Simple {
   public static class SchemaTester<T> {
      /// <summary>
      /// Parses stream using the Flat File 
      /// disassembler and the specified schema.
      /// </summary>
      /// <param name="inputDocument">Contains the flat file to parse</param>
      /// <returns>The resulting XML output from the parsing</returns>
      /// <remarks>
      /// Use <c ref='ErrorHelper.GetErrorMessage'/> to
      /// get detailed error information when parsing fails.
      /// </remarks>
      public static Stream ParseFF(Stream inputDocument) {
         FFDisassembler ff = Disassembler.FlatFile().WithDocumentSpec<T>();
         return Parse(ff, inputDocument);
      }
      /// <summary>
      /// Parses a stream using the XML disassembler with
      /// the specified schema and validation enabled.
      /// </summary>
      /// <param name="inputDocument">Contains the XML doc to parse</param>
      /// <returns>The resulting parsed XML tree</returns>
      /// <remarks>
      /// Use <c ref='ErrorHelper.GetErrorMessage'/> to
      /// get detailed error information when parsing fails.
      /// </remarks>
      public static Stream ParseXml(Stream inputDocument) {
         XmlDisassembler xml = Disassembler.Xml().WithDocumentSpec<T>()
            .WithValidation(true);
         return Parse(xml, inputDocument);
      }
      /// <summary>
      /// Generates a flat file from an XML document with the
      /// specified schema
      /// </summary>
      /// <param name="inputDocument">The XML document</param>
      /// <returns>The generated flat file</returns>
      public static Stream AssembleFF(Stream inputDocument) {
         FFAssembler ff = Assembler.FlatFile()
            .WithDocumentSpec<T>();
         return Assemble(ff, inputDocument);
      }
      /// <summary>
      /// Generates an XML file from an XML document with the
      /// specified schema
      /// </summary>
      /// <param name="inputDocument">The XML document</param>
      /// <returns>The generated XML</returns>
      public static Stream AssembleXml(Stream inputDocument) {
         XmlAssembler xml = Assembler.Xml()
            .WithDocumentSpec<T>();
         return Assemble(xml, inputDocument);
      }

      private static Stream Parse(Disassembler component, Stream inputDocument) {
         ReceivePipelineWrapper pipeline = Pipelines.Receive()
            .WithDisassembler(component);
         MessageCollection output = pipeline.Execute(
            MessageHelper.CreateFromStream(inputDocument)
         );
         if ( output.Count > 0 )
            return output[0].BodyPart.GetOriginalDataStream();
         return null;
      }
      private static Stream Assemble(Assembler component, Stream inputDocument) {
         SendPipelineWrapper pipeline = Pipelines.Send()
            .WithAssembler(component);
         IBaseMessage output = pipeline.Execute(
            MessageHelper.CreateFromStream(inputDocument)
         );
         return output.BodyPart.GetOriginalDataStream();
      }
   }

   /// <summary>
   /// Helper class to extract detailed error information
   /// from parsing exceptions thrown by <c ref='SchemaTester'/> methods.
   /// </summary>
   public static class ErrorHelper {
      /// <summary>
      /// Extracts a detailed error message from the specified exception.
      /// </summary>
      /// <param name="ex">Exception to examine</param>
      /// <returns>The error message</returns>
      public static String GetErrorMessage(Exception ex) {
         XmlException xmlex = ex as XmlException;
         if ( xmlex != null ) {
            // a flat file disassembler, maybe
            return (xmlex.InnerException ?? xmlex).Message;
         }
         XmlDasmException xdasmex = ex as XmlDasmException;
         if ( xdasmex != null ) {
            return xdasmex.GetArgument(0);
         }
         return ex.Message;
      }
   }
}

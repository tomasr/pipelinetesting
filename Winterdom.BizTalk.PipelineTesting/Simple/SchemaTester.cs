using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component;

namespace Winterdom.BizTalk.PipelineTesting.Simple {

   /// <summary>
   /// Makes it easier to do basic testing of BizTalk schema
   /// parsing and assembling functionality.
   /// </summary>
   /// <remarks>
   /// SchemaTester works by creating a dynamic pipeline with
   /// either the XML or Flat File components and running your
   /// input documents through it.
   /// </remarks>
   /// <typeparam name="T">The BizTalk Schema to use</typeparam>
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
      /// Parses the file using the Flat File 
      /// disassembler and the specified schema and writes the result
      /// to another file
      /// </summary>
      /// <param name="inputPath">Path to the flat file to parse</param>
      /// <param name="outputPath">File to write output to</param>
      /// <remarks>
      /// Use <c ref='ErrorHelper.GetErrorMessage'/> to
      /// get detailed error information when parsing fails.
      /// </remarks>
      public static void ParseFF(String inputPath, String outputPath) {
         using ( Stream input = new FileStream(inputPath, FileMode.Open) ) {
            using ( Stream output = ParseFF(input) ) {
               WriteToFile(output, outputPath);
            }
         }
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
      /// Parses a file using the XML disassembler with
      /// the specified schema and validation enabled, and writes
      /// the result to another file
      /// </summary>
      /// <param name="inputPath">The path to the XML doc to parse</param>
      /// <param name="outputPath">The path to write the results to</param>
      /// <remarks>
      /// Use <c ref='ErrorHelper.GetErrorMessage'/> to
      /// get detailed error information when parsing fails.
      /// </remarks>
      public static void ParseXml(String inputPath, String outputPath) {
         using ( Stream input = new FileStream(inputPath, FileMode.Open) ) {
            using ( Stream output = ParseXml(input) ) {
               WriteToFile(output, outputPath);
            }
         }
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
      /// Generates a flat file from an XML document with the
      /// specified schema and writes the result to another file
      /// </summary>
      /// <param name="inputPath">The path to the XML document</param>
      /// <param name="outputPath">The path to write the output flat file to</param>
      public static void AssembleFF(String inputPath, String outputPath) {
         using ( Stream input = new FileStream(inputPath, FileMode.Open) ) {
            using ( Stream output = AssembleFF(input) ) {
               WriteToFile(output, outputPath);
            }
         }
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

      /// <summary>
      /// Generates an XML file from an XML document with the
      /// specified schema
      /// </summary>
      /// <param name="inputPath">The path to the XML document</param>
      /// <param name="outputPath">The path to write the output to</param>
      public static void AssembleXml(String inputPath, String outputPath) {
         using ( Stream input = new FileStream(inputPath, FileMode.Open) ) {
            using ( Stream output = AssembleXml(input) ) {
               WriteToFile(output, outputPath);
            }
         }
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
      private static void WriteToFile(Stream output, string outputPath) {
         Stream outStream = new FileStream(outputPath, FileMode.OpenOrCreate);
         using ( outStream ) {
            byte[] buf = new byte[4096];
            int read = 0;
            while ( (read = output.Read(buf, 0, buf.Length)) > 0 )
               outStream.Write(buf, 0, read);
         }
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
            StringBuilder errors = new StringBuilder();
            for ( int i = 0; i < xdasmex.ArgumentCount; i++ ) {
               errors.AppendLine(xdasmex.GetArgument(i));
            }
            return errors.ToString();
         }
         return ex.Message;
      }
   }
}

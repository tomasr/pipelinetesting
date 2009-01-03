using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.BizTalk.Message.Interop;

namespace Winterdom.BizTalk.PipelineTesting.Simple {
   public static class SchemaTester<T> {
      public static Stream ParseFF(Stream inputDocument) {
         FFDisassembler ff = Disassembler.FlatFile().WithDocumentSpec<T>();
         return Parse(ff, inputDocument);
      }
      public static Stream ParseXml(Stream inputDocument) {
         XmlDisassembler xml = Disassembler.Xml().WithDocumentSpec<T>();
         return Parse(xml, inputDocument);
      }
      public static Stream AssembleFF(Stream inputDocument) {
         FFAssembler ff = Assembler.FlatFile()
            .WithDocumentSpec<T>();
         return Assemble(ff, inputDocument);
      }
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
}


//
// XmlPipelines.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.DefaultPipelines;

namespace Winterdom.BizTalk.PipelineTesting.Simple
{
   /// <summary>
   /// Creates instances of the standard XML pipelines in BizTalk.
   /// </summary>
   public class XmlPipelines
   {
      /// <summary>
      /// Creates a new XMLTransmit pipeline
      /// </summary>
      /// <returns>The Pipeline Builder instance</returns>
      public SendPipelineBuilder Send()
      {
         return Pipelines.Send<XMLTransmit>();
      }

      /// <summary>
      /// Creates a new XMLReceive pipeline
      /// </summary>
      /// <returns>The Pipeline Builder instance</returns>
      public ReceivePipelineBuilder Receive()
      {
         return Pipelines.Receive<XMLReceive>();
      }
   } // class XmlPipelines

} // namespace Winterdom.BizTalk.PipelineTesting.Simple


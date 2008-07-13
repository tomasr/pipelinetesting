
//
// SendPipelineWrapper.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using IPipeline = Microsoft.Test.BizTalk.PipelineObjects.IPipeline;
using PStage = Microsoft.Test.BizTalk.PipelineObjects.Stage;

namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Wrapper around a send pipeline you can execute
   /// </summary>
   public class SendPipelineWrapper : BasePipelineWrapper
   {
      internal SendPipelineWrapper(IPipeline pipeline)
         : base (pipeline, false)
      {
         FindStage(PipelineStage.PreAssemble);
         FindStage(PipelineStage.Assemble);
         FindStage(PipelineStage.Encode);
      }


      /// <summary>
      /// Execute the send pipeline
      /// </summary>
      /// <param name="inputMessages">Set of input messages to feed to the pipeline</param>
      /// <returns>The output message</returns>
      public IBaseMessage Execute(MessageCollection inputMessages)
      {
         if ( inputMessages == null )
            throw new ArgumentNullException("inputMessages");
         if ( inputMessages.Count <= 0 )
            throw new ArgumentException("Must provide at least one input message", "inputMessages");

         foreach ( IBaseMessage inputMessage in inputMessages )
         {
            Pipeline.InputMessages.Add(inputMessage);
         }

         MessageCollection output = new MessageCollection();
         Pipeline.Execute(Context);

         IBaseMessage om = Pipeline.GetNextOutputMessage(Context);
         return om;
      }

      /// <summary>
      /// Executes the send pipeline with all messages
      /// provided as inputs
      /// </summary>
      /// <param name="inputMessages">One or more input messages to the pipeline</param>
      /// <returns>The single output message</returns>
      public IBaseMessage Execute(params IBaseMessage[] inputMessages)
      {
         MessageCollection inputs = new MessageCollection();
         inputs.AddRange(inputMessages);
         return Execute(inputs);
      }

   } // class SendPipelineWrapper

} // namespace Winterdom.BizTalk.PipelineTesting

using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   public class SendStageTest : IBaseComponent, IComponent, IAssemblerComponent
   {
      private IList<Guid> _stagesSeen;
      private IBaseMessage _inputMessage;

      public string Description
      {
         get { return ""; }
      }

      public string Name
      {
         get { return "SendStageTest"; }
      }

      public string Version
      {
         get { return "1.0"; }
      }

      public SendStageTest(IList<Guid> stages)
      {
         _stagesSeen = stages;
      }

      public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
      {
         _stagesSeen.Add(pContext.StageID);
         return pInMsg;
      }

      public void AddDocument(IPipelineContext pContext, IBaseMessage pInMsg)
      {
         _inputMessage = pInMsg;
      }

      public IBaseMessage Assemble(IPipelineContext pContext)
      {
         var message = _inputMessage;
         _inputMessage = null;
         if ( message != null )
         {
            _stagesSeen.Add(pContext.StageID);
         }
         return message;
      }
   }
}


//
// PipelineStage.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.BizTalk.PipelineObjects;
using Microsoft.BizTalk.Component.Interop;


namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Represents a stage in a pipeline
   /// where you can add components
   /// </summary>
   public sealed class PipelineStage
   {
      private Guid _id;
      private string _name;
      private ExecuteMethod _executeMethod;
      private bool _isReceiveStage;


      #region Properties
      //
      // Properties
      //

      /// <summary>
      /// ID of the pipeline Stage
      /// </summary>
      public Guid ID
      {
         get { return _id; }
      }

      /// <summary>
      /// Name of the stage
      /// </summary>
      public string Name
      {
         get { return _name; }
      }

      /// <summary>
      /// ExecuteMethod of the stage
      /// </summary>
      public ExecuteMethod ExecuteMethod
      {
         get { return _executeMethod; }
      }

      /// <summary>
      /// Indicates if this is a stage of a receive pipeline
      /// </summary>
      public bool IsReceiveStage
      {
         get { return _isReceiveStage; }
      }
      #endregion // Properties

      private PipelineStage(string id, string name, ExecuteMethod method, bool isReceiveStage)
      {
         _id = new Guid(id);
         _name = name;
         _executeMethod = method;
         _isReceiveStage = isReceiveStage;
      }

      /// <summary>
      /// Decode stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage Decode = new PipelineStage(CategoryTypes.CATID_Decoder, "Decode", ExecuteMethod.All, true);
      /// <summary>
      /// Disassemble stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage Disassemble = new PipelineStage(CategoryTypes.CATID_DisassemblingParser, "Disassemble", ExecuteMethod.FirstMatch, true);
      /// <summary>
      /// Validation stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage Validate = new PipelineStage(CategoryTypes.CATID_Validate, "Validate", ExecuteMethod.All, true);
      /// <summary>
      /// Party Resolution stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage ResolveParty = new PipelineStage(CategoryTypes.CATID_PartyResolver, "ResolveParty", ExecuteMethod.All, true);

      /// <summary>
      /// Pre-Assemble stage of send pipelines
      /// </summary>
      public static readonly PipelineStage PreAssemble = new PipelineStage(CategoryTypes.CATID_Any, "Pre-Assemble", ExecuteMethod.All, false);
      /// <summary>
      /// Assemble stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage Assemble = new PipelineStage(CategoryTypes.CATID_AssemblingSerializer, "Assemble", ExecuteMethod.All, false);
      /// <summary>
      /// Encode stage of receive pipelines
      /// </summary>
      public static readonly PipelineStage Encode = new PipelineStage(CategoryTypes.CATID_Encoder, "Encode", ExecuteMethod.All, false);

   } // class PipelineStage

} // namespace Winterdom.BizTalk.PipelineTesting

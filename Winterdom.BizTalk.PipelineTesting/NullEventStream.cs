
//
// NullEventStream.cs
//
// Author:
//    Tomas Restrepo (tomas@winterdom.com)
//

using System;
using System.Text;

using Microsoft.BizTalk.Bam.EventObservation;

namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// EventStream subclass that doesn't do anything
   /// </summary>
   internal class NullEventStream : EventStream
   {
      public override void AddReference(string activityName, string activityID, string referenceType, string referenceName, string referenceData)
      {
      }
      public override void AddReference(string activityName, string activityID, string referenceType, string referenceName, string referenceData, string longreferenceData)
      {
      }
      public override void AddRelatedActivity(string activityName, string activityID, string relatedActivityName, string relatedTraceID)
      {
      }
      public override void BeginActivity(string activityName, string activityInstance)
      {
      }
      public override void Clear()
      {
      }
      public override void EnableContinuation(string activityName, string activityInstance, string continuationToken)
      {
      }
      public override void EndActivity(string activityName, string activityInstance)
      {
      }
      public override void Flush()
      {
      }
      public override void Flush(System.Data.SqlClient.SqlConnection connection)
      {
      }
      public override void StoreCustomEvent(IPersistQueryable singleEvent)
      {
      }
      public override void UpdateActivity(string activityName, string activityInstance, params object[] data)
      {
      }
   } // class NullEventStream
} // class Winterdom.BizTalk.PipelineTesting

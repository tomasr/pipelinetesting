
//
// Message Collection
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Microsoft.BizTalk.Message.Interop;

namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Represents a collection of IBaseMessage
   /// objects
   /// </summary>
   public class MessageCollection : Collection<IBaseMessage>
   {
      /// <summary>
      /// Adds multiple messages to this collection
      /// </summary>
      /// <param name="list">List of messages to add</param>
      public void AddRange(IEnumerable<IBaseMessage> list)
      {
         foreach ( IBaseMessage msg in list )
            Add(msg);
      }
   } // class MessageCollection

} // namespace Winterdom.BizTalk.PipelineTesting

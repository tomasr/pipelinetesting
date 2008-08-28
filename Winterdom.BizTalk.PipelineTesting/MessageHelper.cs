
//
// MessageFactory.cs
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
using Microsoft.Test.BizTalk.PipelineObjects;


namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Helper class you can use to create 
   /// IBaseMessage instances to pass on
   /// to the pipeline testing classes
   /// </summary>
   public static class MessageHelper
   {
      private static IBaseMessageFactory _factory = new MessageFactory();

      /// <summary>
      /// Creates a new message with the specified string
      /// as the body part.
      /// </summary>
      /// <param name="body">Content of the body</param>
      /// <returns>A new message</returns>
      public static IBaseMessage CreateFromString(string body)
      {
         if ( body == null )
            throw new ArgumentNullException("body");

         byte[] content = Encoding.Unicode.GetBytes(body);
         Stream stream = new MemoryStream(content);

         IBaseMessage msg = CreateFromStream(stream);
         msg.BodyPart.Charset = "UTF-16";
         return msg;
      }


      /// <summary>
      /// Create a new message with the specified stream as 
      /// the body part.
      /// </summary>
      /// <param name="body">Body of the message</param>
      /// <returns>A new message object</returns>
      public static IBaseMessage CreateFromStream(Stream body)
      {
         if ( body == null )
            throw new ArgumentNullException("body");

         IBaseMessage message = _factory.CreateMessage();
         message.Context = _factory.CreateMessageContext();

         IBaseMessagePart bodyPart = CreatePartFromStream(body);

         message.AddPart("body", bodyPart, true);

         return message;
      }

      /// <summary>
      /// Creates a new message part with the specified data
      /// </summary>
      /// <param name="body">Data of the part</param>
      /// <returns>The new part</returns>
      public static IBaseMessagePart CreatePartFromString(string body)
      {
         if ( body == null )
            throw new ArgumentNullException("body");

         byte[] content = Encoding.Unicode.GetBytes(body);
         Stream stream = new MemoryStream(content);

         IBaseMessagePart part = CreatePartFromStream(stream);
         part.Charset = "UTF-16";
         return part;
      }

      /// <summary>
      /// Creates a new message part
      /// </summary>
      /// <param name="body">Body of the part</param>
      /// <returns>The new part</returns>
      public static IBaseMessagePart CreatePartFromStream(Stream body)
      {
         if ( body == null )
            throw new ArgumentNullException("body");

         IBaseMessagePart part = _factory.CreateMessagePart();
         part.Data = body;
         return part;
      }

      /// <summary>
      /// Creates a multi-part message from an array
      /// of strings. The first string in the array will be marked
      /// as the message body part
      /// </summary>
      /// <param name="parts">One string for each part</param>
      /// <returns>The new message</returns>
      public static IBaseMessage Create(params String[] parts) 
      {
         if ( parts == null || parts.Length < 1 )
            throw new ArgumentException("Need to specify at least one part", "parts");

         IBaseMessage message = CreateFromString(parts[0]);
         for ( int i=1; i < parts.Length; i++ )
            message.AddPart("part" + i, CreatePartFromString(parts[i]), false);
         return message;
      }

      /// <summary>
      /// Creates a multi-part message from an array
      /// of streams. The first stream in the array will be marked
      /// as the message body part
      /// </summary>
      /// <param name="parts">One stream for each part</param>
      /// <returns>The new message</returns>
      public static IBaseMessage Create(params Stream[] parts) 
      {
         if ( parts == null || parts.Length < 1 )
            throw new ArgumentException("Need to specify at least one part", "parts");

         IBaseMessage message = CreateFromStream(parts[0]);
         for ( int i=1; i < parts.Length; i++ )
            message.AddPart("part" + i, CreatePartFromStream(parts[i]), false);
         return message;
      }

      /// <summary>
      /// Helper method to consume a stream
      /// </summary>
      /// <param name="stream">Stream to consume</param>
      public static void ConsumeStream(Stream stream)
      {
         if ( stream == null )
            throw new ArgumentNullException("stream");
         byte[] buffer = new byte[4096];
         int read = 0;
         while ( (read = stream.Read(buffer, 0, buffer.Length)) > 0 )
            ;
      }
      /// <summary>
      /// Helper method to consume the message body part stream
      /// </summary>
      /// <param name="message">Message to consume</param>
      public static void ConsumeStream(IBaseMessage message) 
      {
         ConsumeStream(message.BodyPart);
      }
      /// <summary>
      /// Helper method to consume the part stream
      /// </summary>
      /// <param name="part">Part to consume</param>
      public static void ConsumeStream(IBaseMessagePart part)
      {
         ConsumeStream(part.Data);
      }
   } // class MessageFactory

} // namespace Winterdom.BizTalk.PipelineTesting

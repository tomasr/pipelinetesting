
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
using System.Xml;
using System.Xml.XPath;

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
         if ( message == null )
            throw new ArgumentNullException("message");
         ConsumeStream(message.BodyPart);
      }
      /// <summary>
      /// Helper method to consume the part stream
      /// </summary>
      /// <param name="part">Part to consume</param>
      public static void ConsumeStream(IBaseMessagePart part)
      {
         if ( part == null )
            throw new ArgumentNullException("part");
         ConsumeStream(part.Data);
      }
      /// <summary>
      /// Helper method to read back a stream as a string
      /// </summary>
      /// <param name="stream">Stream to consume</param>
      /// <param name="encoding">Expected encoding of the stream contents</param>
      public static string ReadString(Stream stream, Encoding encoding)
      {
         if ( stream == null )
            throw new ArgumentNullException("stream");
         if ( encoding == null )
            throw new ArgumentNullException("encoding");
         using ( StreamReader reader = new StreamReader(stream, encoding) )
            return reader.ReadToEnd(); 
      }
      /// <summary>
      /// Helper method to read back a stream as a string
      /// </summary>
      /// <param name="message">Message to consume</param>
      public static string ReadString(IBaseMessage message)
      {
         if ( message == null )
            throw new ArgumentNullException("message");
         return ReadString(message.BodyPart);
      }
      /// <summary>
      /// Helper method to read back a stream as a string
      /// </summary>
      /// <param name="part">Part to consume</param>
      public static String ReadString(IBaseMessagePart part)
      {
         if ( part == null )
            throw new ArgumentNullException("part");
         Encoding enc = Encoding.UTF8;
         if ( !String.IsNullOrEmpty(part.Charset) )
            enc = Encoding.GetEncoding(part.Charset);
         return ReadString(part.Data, enc);
      }

      /// <summary>
      /// Loads a BizTalk message from the set of files exported from
      /// the BizTalk Admin Console or HAT
      /// </summary>
      /// <param name="contextFile">Path to the *_context.xml file</param>
      /// <returns>The loaded message</returns>
      /// <remarks>
      /// Context files have no type information for properties
      /// in the message context, so all properties are 
      /// added as strings to the context.
      /// </remarks>
      public static IBaseMessage LoadMessage(string contextFile)
      {
         IBaseMessage msg = _factory.CreateMessage();
         IBaseMessageContext ctxt = _factory.CreateMessageContext();
         msg.Context = ctxt;

         XPathDocument doc = new XPathDocument(contextFile);
         XPathNavigator nav = doc.CreateNavigator();
         XPathNodeIterator props = nav.Select("//Property");
         foreach ( XPathNavigator prop in props )
         {
            ctxt.Write(
               prop.GetAttribute("Name", ""),
               prop.GetAttribute("Namespace", ""), 
               prop.GetAttribute("Value", "")
               );
         }

         XPathNodeIterator parts = nav.Select("//MessagePart");
         foreach ( XPathNavigator part in parts )
         {
            LoadPart(msg, part, contextFile);
         }
         return msg;
      }

      private static void LoadPart(IBaseMessage msg, XPathNavigator node, string contextFile)
      {
         // don't care about the id because we can't set it anyway
         string name = node.GetAttribute("Name", "");
         string filename = node.GetAttribute("FileName", "");
         string charset = node.GetAttribute("Charset", "");
         string contentType = node.GetAttribute("ContentType", "");
         bool isBody = XmlConvert.ToBoolean(node.GetAttribute("IsBodyPart", ""));

         XmlResolver resolver = new XmlUrlResolver();
         Uri realfile = resolver.ResolveUri(new Uri(contextFile), filename);
         IBaseMessagePart part = CreatePartFromStream(File.OpenRead(realfile.LocalPath));
         part.Charset = charset;
         part.ContentType = contentType;
         msg.AddPart(name, part, isBody);
      }
   } // class MessageFactory

} // namespace Winterdom.BizTalk.PipelineTesting

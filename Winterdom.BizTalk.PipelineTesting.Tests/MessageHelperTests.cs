
//
// MessageHelperTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;

using NUnit.Framework;

using Winterdom.BizTalk.PipelineTesting;
using SampleSchemas;


namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   /// <summary>
   /// Tests for the MessageHelper class
   /// </summary>
   [TestFixture]
   public class MessageHelperTests
   {
      /// <summary>
      /// Test we fault with a null argument
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenBodyStringIsNull()
      {
         MessageHelper.CreateFromString(null);
      }

      /// <summary>
      /// Test we fault with a null argument
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenBodyStreamIsNull()
      {
         MessageHelper.CreateFromStream(null);
      }

      /// <summary>
      /// Test we fault with a null argument
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenPartStringIsNull()
      {
         MessageHelper.CreatePartFromString(null);
      }

      /// <summary>
      /// Test we fault with a null argument
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenPartStreamIsNull()
      {
         MessageHelper.CreatePartFromStream(null);
      }

      /// <summary>
      /// Tests we can create a new message from a string
      /// </summary>
      [Test]
      public void CanCreateMessageFromBodyString()
      {
         string body = "<body>Some message content</body>";

         IBaseMessage message = MessageHelper.CreateFromString(body);
         Assert.IsNotNull(message);
         Assert.IsNotNull(message.BodyPart);
         Assert.IsNotNull(message.BodyPart.Data);
         Assert.IsTrue(message.BodyPart.Data.Length > 0);
         Assert.IsNotNull(message.BodyPart.GetOriginalDataStream());
      }

      /// <summary>
      /// Tests we can create a new multipart message
      /// correctly
      /// </summary>
      [Test]
      public void CanCreateMultipartMessage()
      {
         string body = "<body>Some message content</body>";

         IBaseMessage message = MessageHelper.CreateFromString(body);
         Assert.IsNotNull(message);
         Assert.IsNotNull(message.BodyPart);
         Assert.IsNotNull(message.BodyPart.Data);
         Assert.IsTrue(message.BodyPart.Data.Length > 0);

         IBaseMessagePart part1 = MessageHelper.CreatePartFromString(body);
         message.AddPart("part1", part1, false);
         Assert.AreEqual(2, message.PartCount);
         Assert.IsNotNull(message.GetPart("part1"));
      }

      /// <summary>
      /// Tests that we can create a multipart message
      /// using the simple method and a bunch of strings
      /// </summary>
      [Test]
      public void CanCreateMultipartFromStringsSimple() 
      {
         IBaseMessage message = MessageHelper.Create(
            "<body>This is the body part (part1)</body>",
            "<body>This is the part2</body>",
            "<body>This is the part3</body>"
         );
         Assert.AreEqual(3, message.PartCount);
         
         string name = null;
         message.GetPartByIndex(0, out name);
         Assert.AreEqual("body", name);
         message.GetPartByIndex(1, out name);
         Assert.AreEqual("part1", name);
         message.GetPartByIndex(2, out name);
         Assert.AreEqual("part2", name);
      }

      /// <summary>
      /// Tests that we can create a multipart message
      /// using the simple method and a bunch of Streams
      /// </summary>
      [Test]
      public void CanCreateMultipartFromStreamSimple()
      {
         IBaseMessage message = MessageHelper.Create(
            DocLoader.LoadStream("Env_Batch_Input.xml"),
            DocLoader.LoadStream("CSV_FF_RecvInput.txt")
         );
         Assert.AreEqual(2, message.PartCount);
      }
      /// <summary>
      /// Test we can consume an entire message stream
      /// </summary>
      [Test]
      public void CanConsumeStream()
      {
         string body = "<body>Some message content</body>";
         IBaseMessage message = MessageHelper.CreateFromString(body);
         MessageHelper.ConsumeStream(message);
         Assert.AreEqual(message.BodyPart.Data.Length, message.BodyPart.Data.Position);
      }
      /// <summary>
      /// Test we can read back an entire message stream
      /// </summary>
      [Test]
      public void CanReadStream()
      {
         string body = "<body>Some message content</body>";
         IBaseMessage message = MessageHelper.CreateFromString(body);
         Assert.AreEqual(body, MessageHelper.ReadString(message));
      }
   } // class MessageHelperTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

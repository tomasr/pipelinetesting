
//
// PipelineContextTests.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;

using NUnit.Framework;

using Winterdom.BizTalk.PipelineTesting;
using SampleSchemas;


namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   /// <summary>
   /// Tests for the PipelineContext class
   /// </summary>
   [TestFixture]
   public class PipelineContextTests
   {

      /// <summary>
      /// Test we can create a default empty context with
      /// the correct values
      /// </summary>
      [Test]
      public void CanCreateDefaultContext()
      {
         PipelineContext context = new PipelineContext();
         Assert.AreEqual(0, context.ComponentIndex);
         Assert.AreEqual(Guid.Empty, context.PipelineID);
         Assert.IsFalse(String.IsNullOrEmpty(context.PipelineName));
         Assert.IsNotNull(context.ResourceTracker);
         Assert.AreEqual(Guid.Empty, context.StageID);
         Assert.AreEqual(0, context.StageIndex);

         Assert.IsNotNull(context.GetMessageFactory());
         Assert.IsFalse(context.AuthenticationRequiredOnReceivePort);
      }

      /// <summary>
      /// Test we can add a document spec by name
      /// to an empty context
      /// </summary>
      [Test]
      public void CanAddDocumentSpecByName()
      {
         PipelineContext context = new PipelineContext();
         DocSpecLoader loader = new DocSpecLoader();
         context.AddDocSpecByName("spec1", loader.LoadDocSpec(typeof(Schema1_NPP)));

         Assert.IsNotNull(context.GetDocumentSpecByName("spec1"));
      }

      /// <summary>
      /// Test we throw the correct exception
      /// when a docSpec is not found
      /// </summary>
      [Test]
      [ExpectedException(typeof(COMException))]
      public void ThrowExceptionIfNoDocSpecFoundByName()
      {
         IPipelineContext context = new PipelineContext();
         context.GetDocumentSpecByName("non-existant-spec");
      }

      /// <summary>
      /// Test we can add a document spec by type
      /// to an empty context
      /// </summary>
      [Test]
      public void CanAddDocumentSpecByType()
      {
         PipelineContext context = new PipelineContext();
         DocSpecLoader loader = new DocSpecLoader();
         context.AddDocSpecByType("spec1", loader.LoadDocSpec(typeof(Schema1_NPP)));

         Assert.IsNotNull(context.GetDocumentSpecByType("spec1"));
      }

      /// <summary>
      /// Test we throw the correct exception
      /// when a docSpec is not found
      /// </summary>
      [Test]
      [ExpectedException(typeof(COMException))]
      public void ThrowExceptionIfNoDocSpecFoundByType()
      {
         IPipelineContext context = new PipelineContext();
         context.GetDocumentSpecByType("non-existant-spec");
      }


      /// <summary>
      /// Tests we can modify the AuthenticationRequiredOnReceivePort 
      /// property
      /// </summary>
      [Test]
      public void CanSetReceivePortAuthentication()
      {
         PipelineContext context = new PipelineContext();
         Assert.IsFalse(context.AuthenticationRequiredOnReceivePort);

         context.SetAuthenticationRequiredOnReceivePort(true);
         Assert.IsTrue(context.AuthenticationRequiredOnReceivePort);
      }

      /// <summary>
      /// Tests we can modify the group
      /// signing certificate
      /// </summary>
      [Test]
      public void CanSetGroupSigningCertificate()
      {
         string certificate = 
            "ee a5 bc 5f 19 14 3c 01 b0 41 d2 83 e6 92 68 a0 51 3d fb a1"; 
         
         PipelineContext context = new PipelineContext();
         Assert.IsNull(context.GetGroupSigningCertificate());

         context.SetGroupSigningCertificate(certificate);
         Assert.AreEqual(certificate, context.GetGroupSigningCertificate());
      }

      /// <summary>
      /// Tests we can enable the context
      /// to support IPipelineContextEx.GetTransaction()
      /// </summary>
      [Test]
      public void CanEnableTransactionSupport()
      {
         PipelineContext context = new PipelineContext();
         Assert.IsNull(context.GetTransaction());

         context.EnableTransactionSupport();
         object transaction = context.GetTransaction();
         Assert.IsNotNull(transaction);

         object transaction2 = context.GetTransaction();
         Assert.AreSame(transaction, transaction2);
      }

      /// <summary>
      /// Tests that a context has an EventStream associated with it
      /// </summary>
      [Test]
      public void HasEventStream()
      {
         PipelineContext context = new PipelineContext();
         Assert.IsNotNull(context.GetEventStream());
      }

   } // class PipelineContextTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

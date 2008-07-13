
//
// DocSpecLoaderTests.cs
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
   /// Tests for the DocSpecLoader class
   /// </summary>
   [TestFixture]
   public class DocSpecLoaderTests
   {
      /// <summary>
      /// Tests we fail when we pass in null 
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ThrowExceptionWhenDocSpecIsNull()
      {
         DocSpecLoader loader = new DocSpecLoader();
         loader.LoadDocSpec(null);
      }

      /// <summary>
      /// Tests we fail when we pass in a type that
      /// does not represent a document schema 
      /// </summary>
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ThrowExceptionWhenDocSpecIsInvalidType()
      {
         DocSpecLoader loader = new DocSpecLoader();
         loader.LoadDocSpec(typeof(string));
      }

      /// <summary>
      /// Tests we can load a new DocSpec from a Schema type
      /// (biztalk assembly) that contains no promoted properties.
      /// </summary>
      [Test]
      public void CanLoadDocSpecWithNoPromotedProperties()
      {
         DocSpecLoader loader = new DocSpecLoader();
         IDocumentSpec docSpec = loader.LoadDocSpec(typeof(Schema1_NPP));
         Assert.IsNotNull(docSpec);
      }

      /// <summary>
      /// Tests we can load a new DocSpec from a Schema type
      /// (biztalk assembly) that contains promoted properties in 
      /// a property schema in the same assembly.
      /// </summary>
      [Test]
      public void CanLoadDocSpecWithPromotedProperties()
      {
         DocSpecLoader loader = new DocSpecLoader();
         IDocumentSpec docSpec = loader.LoadDocSpec(typeof(Schema2_WPP));
         Assert.IsNotNull(docSpec);
      }

   } // class DocSpecLoaderTests

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

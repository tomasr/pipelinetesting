
//
// DocLoader.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Winterdom.BizTalk.PipelineTesting.Tests
{
   internal static class DocLoader
   {
      /// <summary>
      /// Loads a document instance from a resource
      /// </summary>
      /// <param name="name">Name of the resource</param>
      /// <returns></returns>
      public static Stream LoadStream(string name)
      {
         string resName = typeof(DocLoader).Namespace + "." + name;
         Assembly assembly = Assembly.GetExecutingAssembly();
         return assembly.GetManifestResourceStream(resName);
      }
   
   } // class DocLoader

} // namespace Winterdom.BizTalk.PipelineTesting.Tests

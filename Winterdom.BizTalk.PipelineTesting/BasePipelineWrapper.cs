
//
// BasePipelineWrapper.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Microsoft.BizTalk.PipelineOM;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;
using IPipeline = Microsoft.Test.BizTalk.PipelineObjects.IPipeline;
using PStage = Microsoft.Test.BizTalk.PipelineObjects.Stage;
using PCallEventArgs = Microsoft.Test.BizTalk.PipelineObjects.CallEventArgs;


namespace Winterdom.BizTalk.PipelineTesting
{
   /// <summary>
   /// Wrapper around a pipeline you can execute
   /// </summary>
   public abstract class BasePipelineWrapper : IEnumerable<IBaseComponent>
   {
      private IPipeline _pipeline;
      private IPipelineContext _pipelineContext;
      private bool _isReceivePipeline;

      #region Properties
      //
      // Properties
      //

      internal IPipeline Pipeline
      {
         get { return _pipeline; }
      }
      internal IPipelineContext Context
      {
         get { return _pipelineContext; }
      }

      /// <summary>
      /// Gets or Set the thumbprint for the Group
      /// Signing Certificate. Null by default
      /// </summary>
      public string GroupSigningCertificate
      {
         get { return _pipelineContext.GetGroupSigningCertificate(); }
         set
         {
            IConfigurePipelineContext ctxt = (IConfigurePipelineContext)_pipelineContext;
            ctxt.SetGroupSigningCertificate(value);
         }
      }

      #endregion // Properties

      /// <summary>
      /// Initializes an instance
      /// </summary>
      /// <param name="pipeline">Pipeline object to wrap</param>
      /// <param name="isReceivePipeline">True if it's a receive pipeline</param>
      protected BasePipelineWrapper(IPipeline pipeline, bool isReceivePipeline)
      {
         if ( pipeline == null )
            throw new ArgumentNullException("pipeline");
         _pipeline = pipeline;
         pipeline.Calling += OnCallingStage;
         _pipelineContext = CreatePipelineContext();
         _isReceivePipeline = isReceivePipeline;
      }

      /// <summary>
      /// Adds a component to the specified stage
      /// </summary>
      /// <param name="component">Component to add to the stage</param>
      /// <param name="stage">Stage to add it to</param>
      public void AddComponent(IBaseComponent component, PipelineStage stage)
      {
         if ( component == null )
            throw new ArgumentNullException("component");
         if ( stage == null )
            throw new ArgumentNullException("stage");

         if ( stage.IsReceiveStage != _isReceivePipeline )
            throw new ArgumentException("Invalid Stage", "stage");

         PStage theStage = FindStage(stage);
         theStage.AddComponent(component);
      }

      /// <summary>
      /// Adds a new document specification to the list
      /// of Known Schemas for this pipeline.
      /// </summary>
      /// <remarks>
      /// Adding known schemas is necessary so that
      /// document type resolution works in the disassembler/assembler
      /// stages
      /// </remarks>
      /// <param name="schemaType">Type of the document schema to add</param>
      public void AddDocSpec(Type schemaType)
      {
         if ( schemaType == null )
            throw new ArgumentNullException("schemaType");

         DocSpecLoader loader = new DocSpecLoader();

         Type[] roots = GetSchemaRoots(schemaType);
         foreach ( Type root in roots )
         {
            IDocumentSpec docSpec = loader.LoadDocSpec(root);
            AddDocSpecToContext(docSpec);
         }
      }

      /// <summary>
      /// Adds a new document specification to the list
      /// of Known Schemas for this pipeline.
      /// </summary>
      /// <remarks>
      /// Adding known schemas is necessary so that
      /// document type resolution works in the disassembler/assembler
      /// stages. Notice that this overload does NOT do automatic checking
      /// for multiple roots.
      /// </remarks>
      /// <param name="typeName">The fully qualified (namespace.class) name of 
      /// the schema</param>
      /// <param name="assemblyName">The partial or full name of the assembly
      /// containing the schema</param>
      public void AddDocSpec(string typeName, string assemblyName)
      {
         if ( String.IsNullOrEmpty(typeName) )
            throw new ArgumentNullException("typeName");
         if ( String.IsNullOrEmpty(assemblyName) )
            throw new ArgumentNullException("assemblyName");

         DocSpecLoader loader = new DocSpecLoader();
         IDocumentSpec spec = loader.LoadDocSpec(typeName, assemblyName);
         AddDocSpecToContext(spec);
      }

      /// <summary>
      /// Returns the document spec object for a known doc
      /// spec given the fully qualified type name
      /// </summary>
      /// <param name="name">Typename of the schema</param>
      /// <returns>The docSpec object</returns>
      public IDocumentSpec GetKnownDocSpecByName(string name)
      {
         return Context.GetDocumentSpecByName(name);
      }

      /// <summary>
      /// Returns the document spec object for a known doc
      /// spec given the name of the root (namespace#root)
      /// </summary>
      /// <param name="name">Name of the root</param>
      /// <returns>The docSpec object</returns>
      public IDocumentSpec GetKnownDocSpecByType(string name)
      {
         return Context.GetDocumentSpecByType(name);
      }

      /// <summary>
      /// Enables transactional support for the pipeline
      /// execution, so that the pipeline context
      /// returns a valid transaction
      /// </summary>
      /// <returns>An object to control the transaction lifetime and result</returns>
      public TransactionControl EnableTransactions()
      {
         IConfigurePipelineContext ctxt = (IConfigurePipelineContext)_pipelineContext;
         return ctxt.EnableTransactionSupport();
      }

      /// <summary>
      /// Looks up a component in the pipeline
      /// </summary>
      /// <param name="stage">The stage the component is in</param>
      /// <param name="index">The 0-based index inside the stage</param>
      /// <returns>The component, or null if it was not found</returns>
      public IBaseComponent GetComponent(PipelineStage stage, int index)
      {
         foreach ( PStage st in _pipeline.Stages )
         {
            if ( st.Id == stage.ID )
            {
               IEnumerator enumerator = st.GetComponentEnumerator();
               while ( enumerator.MoveNext() )
               {
                  if ( index-- == 0 )
                  {
                     return (IBaseComponent)enumerator.Current;
                  }
               }
            }
         }
         return null;
      }

      /// <summary>
      /// Apply per-instance pipeline configuration
      /// </summary>
      /// <param name="file">Path to the XML file with the configuration</param>
      /// <remarks>
      /// Per-instance pipeline configuration uses the same XML format used
      /// by the BizTalk Admin console that gets exported to binding files,
      /// for example, in the &lt;SendPipelineData&gt; element
      /// </remarks>
      public void ApplyInstanceConfig(string file)
      {
         using ( XmlReader reader = new XmlTextReader(file) )
            ApplyInstanceConfig(reader);
      }

      /// <summary>
      /// Apply per-instance pipeline configuration
      /// </summary>
      /// <param name="reader">XML reader with the configuration</param>
      /// <remarks>
      /// Per-instance pipeline configuration uses the same XML format used
      /// by the BizTalk Admin console that gets exported to binding files,
      /// for example, in the &lt;SendPipelineData&gt; element
      /// </remarks>
      public void ApplyInstanceConfig(XmlReader reader)
      {
         Guid stageId = Guid.Empty;
         int index = 0;
         while ( reader.Read() )
         {
            if ( reader.NodeType == XmlNodeType.Element )
            {
               if ( reader.LocalName == "Stage" )
               {
                  stageId = new Guid(reader.GetAttribute("CategoryId"));
                  index = 0;
               } else if ( reader.LocalName == "Component" )
               {
                  string name = reader.GetAttribute("Name");
                  reader.ReadToDescendant("Properties");
                  XmlReader propReader = reader.ReadSubtree();
                  propReader.Read();
                  ApplyComponentConfig(stageId, name, index, propReader);
                  index++;
               }
            }
         }
      }

      #region Protected Methods
      //
      // Protected Methods
      //

      /// <summary>
      /// Finds a stage in the pipeline
      /// </summary>
      /// <param name="stage">Stage definition</param>
      /// <returns>The stage, if found, or a new stage if necessary</returns>
      protected PStage FindStage(PipelineStage stage)
      {
         PStage theStage = null;
         foreach ( PStage pstage in _pipeline.Stages )
         {
            if ( pstage.Id == stage.ID )
            {
               theStage = pstage;
               break;
            }
         }
         if ( theStage == null )
         {
            theStage = new PStage(stage.Name, stage.ExecuteMethod, stage.ID, _pipeline);
            _pipeline.Stages.Add(theStage);
         }
         return theStage;
      }

      /// <summary>
      /// Creates a new pipeline context for the execution
      /// </summary>
      /// <returns>The new pipeline context.</returns>
      protected IPipelineContext CreatePipelineContext()
      {
         return new PipelineContext();
      }

      #endregion // Protected Methods

      #region Private Methods
      //
      // Private Methods
      //

      /// <summary>
      /// Gets the namespace#root name for a schema.
      /// If the schema has multiple roots, all are returned.
      /// </summary>
      /// <param name="schemaType">Type of the schema</param>
      /// <returns>Roots of the schema</returns>
      private Type[] GetSchemaRoots(Type schemaType)
      {
         string root = GetSchemaRoot(schemaType);
         if ( root != null )
         {
            return new Type[] { schemaType };
         } else
         {
            Type[] rts = schemaType.GetNestedTypes();
            return rts;
         }
      }

      /// <summary>
      /// Gets the root name (namespace#root) for a schema type
      /// </summary>
      /// <param name="schemaType">Type of the schema</param>
      /// <returns>Roots of the schema</returns>
      private string GetSchemaRoot(Type schemaType)
      {
         SchemaAttribute[] attrs = (SchemaAttribute[])
            schemaType.GetCustomAttributes(typeof(SchemaAttribute), true);
         if ( attrs.Length > 0 )
         {
            if ( String.IsNullOrEmpty(attrs[0].TargetNamespace) )
               return attrs[0].RootElement;
            return string.Format("{0}#{1}", attrs[0].TargetNamespace, attrs[0].RootElement);
         }
         return null;
      }

      /// <summary>
      /// Adds a document specification to the context
      /// </summary>
      /// <param name="docSpec">Specification to add</param>
      private void AddDocSpecToContext(IDocumentSpec docSpec)
      {
         IConfigurePipelineContext ctxt = (IConfigurePipelineContext)Context;
         ctxt.AddDocSpecByType(docSpec.DocType, docSpec);
         // Pipelines referencing local schemas in the same
         // assembly don't have use the assembly qualified name
         // of the schema when trying to find it.
         ctxt.AddDocSpecByName(docSpec.DocSpecStrongName, docSpec);
         ctxt.AddDocSpecByName(docSpec.DocSpecName, docSpec);
      }

      /// <summary>
      /// Applies the loaded configuration to a component
      /// </summary>
      /// <param name="stageId">The stage the component is in</param>
      /// <param name="name">The component name</param>
      /// <param name="index">The index of the component within the pipeline</param>
      /// <param name="reader">The per-instance configuration</param>
      private void ApplyComponentConfig(Guid stageId, string name, int index, XmlReader reader)
      {
         PipelineStage stage = PipelineStage.Lookup(stageId);
         IPersistPropertyBag component = GetComponent(stage, index) 
            as IPersistPropertyBag;
         if ( component != null )
         {
            String compName = component.GetType().FullName;
            if ( compName != name )
               throw new InvalidOperationException(String.Format(
                  "Component in stage '{0}', index {1} is '{2}', expected '{3}'", 
                  stage.Name, index, compName, name));

            IPropertyBag bag = new InstConfigPropertyBag(reader);
            component.Load(bag, 1);
         }
      }

      /// <summary>
      /// Fired when a stage is processed or when a component
      /// is going to be called
      /// </summary>
      /// <param name="sender">stage or component called</param>
      /// <param name="args">stage message</param>
      private void OnCallingStage(object sender, PCallEventArgs args)
      {
         PStage stage = sender as PStage;
         if ( stage != null )
         {
            var configure = _pipelineContext as IConfigurePipelineContext;
            if ( configure != null )
            {
               configure.SetCurrentStage(stage.Id);
            }
         }
      }

      #endregion // Private Methods


      #region IEnumerable<IBaseComponent> Members

      IEnumerator<IBaseComponent> IEnumerable<IBaseComponent>.GetEnumerator()
      {
         foreach ( PStage stage in _pipeline.Stages )
         {
            IEnumerator enumerator = stage.GetComponentEnumerator();
            while ( enumerator.MoveNext() )
            {
               yield return (IBaseComponent)enumerator.Current;
            }
         }
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
         return ((IEnumerable<IBaseComponent>)this).GetEnumerator();
      }

      #endregion

   } // class BasePipelineWrapper

} // namespace Winterdom.BizTalk.PipelineTesting

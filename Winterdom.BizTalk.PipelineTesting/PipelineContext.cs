
//
// PipelineContext.cs
//
// Author:
//    Tomas Restrepo (tomasr@mvps.org)
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Transactions;

using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Bam.EventObservation;

using IPipeline = Microsoft.Test.BizTalk.PipelineObjects.IPipeline;
using MessageFactory = Microsoft.Test.BizTalk.PipelineObjects.MessageFactory;

namespace Winterdom.BizTalk.PipelineTesting
{

   /// <summary>
   /// Controls the lifetime and result of a 
   /// pipeline transaction.
   /// </summary>
   /// <remarks>
   /// Using it is similar to using a TransactionScope object.
   /// </remarks>
   /// <example><![CDATA[
   /// using ( TransactionControl control = pipeline.EnableTransactions() )
   /// {
   ///    // do stuff
   ///    control.SetComplete(); // commit transaction if all ok
   /// }
   /// ]]></example>
   public class TransactionControl : IDisposable
   {
      private CommittableTransaction _transaction;
      private bool _complete;

      internal TransactionControl(CommittableTransaction transaction)
      {
         if ( transaction == null )
            throw new ArgumentNullException("transaction");

         _transaction = transaction;
      }

      /// <summary>
      /// Marks the transaction to attempt to commit
      /// during disposal.
      /// </summary>
      public void SetComplete()
      {
         _complete = true;
      }

      /// <summary>
      /// Remove the transaction context and attempt
      /// to commit or rollback the transaction.
      /// </summary>
      public void Dispose()
      {
         if ( _complete )
            _transaction.Commit();
         else
            _transaction.Rollback();
         _transaction.Dispose();
      }
      
   } // class TransactionControl

   /// <summary>
   /// Interface used to configure and update
   /// the pipeline context mock object
   /// </summary>
   internal interface IConfigurePipelineContext : IPipelineContext, IPipelineContextEx
   {
      void AddDocSpecByName(string name, IDocumentSpec documentSpec);
      void AddDocSpecByType(string type, IDocumentSpec documentSpec);
      void SetAuthenticationRequiredOnReceivePort(bool value);
      void SetGroupSigningCertificate(string certificate);
      TransactionControl EnableTransactionSupport();
      void SetCurrentStage(Guid stageId);
   } // IConfigurePipelineContext

   /// <summary>
   /// Mock class that represents the pipeline
   /// execution context.
   /// </summary>
   /// <remarks>
   /// We mock this class explicitly in order to implement
   /// some functionality that the PipelineObjects library currently
   /// does not implement, including Transaction and Certificate object 
   /// support
   /// </remarks>
   public class PipelineContext : IPipelineContext, IPipelineContextEx, IConfigurePipelineContext
   {
      private int _componentIndex = 0;
      private Guid _pipelineId = Guid.Empty;
      private string _pipelineName = "Pipeline";
      private IResourceTracker _resourceTracker = new ResourceTracker();
      private Guid _stageId = Guid.Empty;
      private int _stageIndex = 0;
      private bool _authenticationRequiredOnReceivePort = false;
      private string _signingCertificate;
      private object _transaction = null;
      private IBaseMessageFactory _messageFactory = new MessageFactory();
      private Dictionary<string, IDocumentSpec> _docSpecsByName = 
         new Dictionary<string, IDocumentSpec>();
      private Dictionary<string, IDocumentSpec> _docSpecsByType =
         new Dictionary<string, IDocumentSpec>();
      private NullEventStream _eventStream;

      #region IPipelineContext Members
      //
      // IPipelineContext Members
      //

      /// <summary>
      /// Index of the currently executing component
      /// </summary>
      public int ComponentIndex
      {
         get { return _componentIndex; }
      }

      /// <summary>
      /// ID of the currently executing pipeline
      /// </summary>
      public Guid PipelineID
      {
         get { return _pipelineId; }
      }

      /// <summary>
      /// The pipeline name
      /// </summary>
      public string PipelineName
      {
         get { return _pipelineName; }
      }

      /// <summary>
      /// The Resource Tracker object associated with this pipeline
      /// </summary>
      public IResourceTracker ResourceTracker
      {
         get { return _resourceTracker; }
      }

      /// <summary>
      /// The ID of the stage currently executing
      /// </summary>
      public Guid StageID
      {
         get { return _stageId; }
      }

      /// <summary>
      /// The Index of the stage currently executing
      /// </summary>
      public int StageIndex
      {
         get { return _stageIndex; }
      }

      /// <summary>
      /// Finds a document specification for a schema added
      /// to the context
      /// </summary>
      /// <param name="docSpecName">CLR type name of the schema</param>
      /// <returns>The document spec, if it exists</returns>
      public IDocumentSpec GetDocumentSpecByName(string docSpecName)
      {
         if ( _docSpecsByName.ContainsKey(docSpecName) )
         {
            return _docSpecsByName[docSpecName];
         }
         throw new COMException("Could not locate document specification with name: " + docSpecName);
      }

      /// <summary>
      /// Finds a document specification for a schema added
      /// to the context
      /// </summary>
      /// <param name="docType">The XML namespace#root of the schema</param>
      /// <returns>The document spec, if it exists</returns>
      public IDocumentSpec GetDocumentSpecByType(string docType)
      {
         if ( _docSpecsByType.ContainsKey(docType) )
         {
            return _docSpecsByType[docType];
         } 
         throw new COMException("Could not locate document specification with type: " + docType);
      }

      /// <summary>
      /// Gets the BAM Event Stream for the pipeline.
      /// </summary>
      /// <returns>An empty stream</returns>
      public EventStream GetEventStream()
      {
         if ( _eventStream == null )
            _eventStream = new NullEventStream();
         return _eventStream;
      }

      /// <summary>
      /// Gets the thumbprint of the X.509 group
      /// signing certificate
      /// </summary>
      /// <returns>The certificate thumbprint, or null</returns>
      public string GetGroupSigningCertificate()
      {
         return _signingCertificate;
      }

      /// <summary>
      /// Gets the message factory object
      /// </summary>
      /// <returns>The mesage factory</returns>
      public IBaseMessageFactory GetMessageFactory()
      {
         return _messageFactory;
      }
      #endregion // IPipelineContext Members


      #region IPipelineContextEx Members
      //
      // IPipelineContextEx Members
      //

      /// <summary>
      /// If true, indicates authentication on the 
      /// receive port was enabled
      /// </summary>
      public bool AuthenticationRequiredOnReceivePort
      {
         get { return _authenticationRequiredOnReceivePort; }
      }

      /// <summary>
      /// Gets the transaction object associated with the process
      /// </summary>
      /// <returns>The ITransaction object</returns>
      public object GetTransaction()
      {
         return _transaction;
      }

      #endregion // IPipelineContextEx Members


      #region IConfigurePipelineContext Members
      //
      // IConfigurePipelineContext Members
      //

      /// <summary>
      /// Adds a new document specification to the context
      /// </summary>
      /// <param name="name">CLR Type name</param>
      /// <param name="documentSpec">Document Spec</param>
      public void AddDocSpecByName(string name, IDocumentSpec documentSpec)
      {
         if ( name == null )
            throw new ArgumentNullException("name");
         if ( documentSpec == null )
            throw new ArgumentNullException("documentSpec");

         if ( !_docSpecsByName.ContainsKey(name) )
            _docSpecsByName.Add(name, documentSpec);
      }

      /// <summary>
      /// Adds a new document specification to the context
      /// </summary>
      /// <param name="type">XML namespace#root</param>
      /// <param name="documentSpec">Document Spec</param>
      public void AddDocSpecByType(string type, IDocumentSpec documentSpec)
      {
         if ( type == null )
            throw new ArgumentNullException("type");
         if ( documentSpec == null )
            throw new ArgumentNullException("documentSpec");

         if ( !_docSpecsByType.ContainsKey(type) )
            _docSpecsByType.Add(type, documentSpec);
      }

      /// <summary>
      /// Configures the AuthenticationRequiredOnReceivePort option
      /// </summary>
      /// <param name="value">New value</param>
      public void SetAuthenticationRequiredOnReceivePort(bool value)
      {
         _authenticationRequiredOnReceivePort = value;
      }

      /// <summary>
      /// Sets the group signing certificate to use
      /// </summary>
      /// <param name="certificate">The certificate thumbprint</param>
      public void SetGroupSigningCertificate(string certificate)
      {
         _signingCertificate = certificate;
      }

      /// <summary>
      /// Enables a transaction for the pipeline execution
      /// </summary>
      /// <returns>Object to control the transaction lifetime</returns>
      public TransactionControl EnableTransactionSupport()
      {
         CommittableTransaction tx = new CommittableTransaction();
         _transaction = TransactionInterop.GetDtcTransaction(tx);
         return new TransactionControl(tx);
      }

      public void SetCurrentStage(Guid stageId)
      {
         _stageId = stageId;
      }

      #endregion // IConfigurePipelineContext Members

   } // class PipelineContext

} // namespace Winterdom.BizTalk.PipelineTesting

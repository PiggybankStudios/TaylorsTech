using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Operations;

namespace TaylorsTech
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class TextviewCreationListener : IVsTextViewCreationListener, IWpfTextViewConnectionListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }

        [Import]
        public IEditorOperationsFactoryService _editorOperationsFactory { get; set; }

        void IVsTextViewCreationListener.VsTextViewCreated(IVsTextView textViewAdapter)
        {
            // Console.WriteLine("TextView was created!");
            DTE2 dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
            IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            textView.Properties.GetOrCreateSingletonProperty(() => new CommandFilter(ServiceProvider, textViewAdapter, textView, dte, _editorOperationsFactory));
        }

        void IWpfTextViewConnectionListener.SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
        }

        void IWpfTextViewConnectionListener.SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
        }
    }
}

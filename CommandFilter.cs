using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
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
	internal class CommandFilter : IOleCommandTarget
	{
		protected IOleCommandTarget _nextCommandTarget;
		protected IWpfTextView _view;
		protected IVsTextView _viewAdapter;
		protected DTE2 _dte;
		protected IEditorOperations _editorOperations;

		public CommandFilter(IVsTextView textViewAdapter, IWpfTextView view, DTE2 dte, IEditorOperationsFactoryService editorOperationsFactory)
		{
			textViewAdapter.AddCommandFilter(this, out _nextCommandTarget);
			_viewAdapter = textViewAdapter;
			_view = view;
			_dte = dte;
			_editorOperations = editorOperationsFactory.GetEditorOperations(view);
		}

		int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return _nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
		}

		int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (pguidCmdGroup == TaylorsTechPackage.CommandSetGuid)
			{
				switch (nCmdID)
				{
					case TaylorsTechPackage.TestTextviewCommandId: Commands.Test_Command(_view, _editorOperations); break;
					case TaylorsTechPackage.InsertTodoCommandId: Commands.InsertTodo_Command(_view, _editorOperations); break;
					case TaylorsTechPackage.OpenSublimeCommandId: Commands.OpenSublime_Command(_viewAdapter, _view, _editorOperations); break;
					default:
					{
						Console.WriteLine("Unknown command ID in our package group!");
					} break;
				}
				
			}

			return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
		}
	}
}

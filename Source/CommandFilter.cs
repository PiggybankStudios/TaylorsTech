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
		public System.IServiceProvider ServiceProvider;
		public IWpfTextView View;
		public IVsTextView ViewAdapter;
		public DTE2 Dte;
		public IEditorOperations EditorOps;
		
		protected IOleCommandTarget _nextCommandTarget;

		public CommandFilter(System.IServiceProvider serviceProvider, IVsTextView textViewAdapter, IWpfTextView view, DTE2 dte, IEditorOperationsFactoryService editorOperationsFactory)
		{
			textViewAdapter.AddCommandFilter(this, out _nextCommandTarget);
			ServiceProvider = serviceProvider;
			ViewAdapter = textViewAdapter;
			View = view;
			Dte = dte;
			EditorOps = editorOperationsFactory.GetEditorOperations(view);
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
					case TaylorsTechPackage.TestTextviewCommandId: Commands.Test_Command(this); break;
					case TaylorsTechPackage.InsertTodoCommandId: Commands.InsertTodo_Command(this); break;
					case TaylorsTechPackage.OpenSublimeCommandId: Commands.OpenSublime_Command(this); break;
					case TaylorsTechPackage.GenerateNumbers0CommandId: Commands.GenerateNumbers0_Command(this); break;
					case TaylorsTechPackage.GenerateNumbers1CommandId: Commands.GenerateNumbers1_Command(this); break;
					case TaylorsTechPackage.GenerateNumbersDialogCommandId: Commands.GenerateNumbersDialog_Command(this); break;
					case TaylorsTechPackage.AlignCursorsCommandId: Commands.AlignCursors_Command(this); break;
					case TaylorsTechPackage.GotoPrevEmptyLineCommandId: Commands.GotoPrevEmptyLine_Command(this); break;
					case TaylorsTechPackage.GotoNextEmptyLineCommandId: Commands.GotoNextEmptyLine_Command(this); break;
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

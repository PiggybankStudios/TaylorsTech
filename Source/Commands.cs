//using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaylorsTech
{
    internal class Commands
    {
		public static void Test_Command(IWpfTextView view, IEditorOperations editorOps)
        {
			using (ITextEdit edit = view.TextBuffer.CreateEdit())
			{
                ITextSnapshot snap = edit.Snapshot;
                int lIndex = 0;
                foreach (ITextSnapshotLine line in snap.Lines)
                {
                    edit.Replace(line.Extent, $"Line {lIndex}!");
                    lIndex++;
                }
                edit.Apply();
			}
		}

        public static void InsertTodo_Command(IWpfTextView view, IEditorOperations editorOps)
        {
			using (ITextEdit edit = view.TextBuffer.CreateEdit())
			{
				List<ITrackingPoint> endingSelections = new List<ITrackingPoint>();

				foreach (VirtualSnapshotSpan vspan in view.Selection.VirtualSelectedSpans)
				{
					endingSelections.Add(view.TextSnapshot.CreateTrackingPoint(vspan.Start.Position, PointTrackingMode.Negative));
					string indentation = editorOps.GetWhitespaceForVirtualSpace(vspan.Start);
					edit.Replace(vspan.SnapshotSpan, indentation + "//###trobbins $TODO [NOCHECKIN]");
				}

				view.Caret.MoveTo(endingSelections[0].GetPoint(view.TextSnapshot));

				edit.Apply();
			}
		}

		public static void OpenSublime_Command(IVsTextView viewAdapter, IWpfTextView view, IEditorOperations editorOps)
        {
			ThreadHelper.ThrowIfNotOnUIThread();
			//.GetContainingLineNumber() is only availabe in 2022+
			int lineNumber = view.Caret.Position.BufferPosition.GetContainingLine().LineNumber; //this is 0 indexed
			viewAdapter.GetBuffer(out IVsTextLines textLines);
			(textLines as IPersistFileFormat).GetCurFile(out string filePath, out uint formatIndex);
			System.Diagnostics.Process.Start("subl.exe", $"\"{filePath}\":{lineNumber+1}");
		}

		private static void GenerateNumbers(IWpfTextView view, IEditorOperations editorOps, int startingIndex, int step)
        {
			using (ITextEdit edit = view.TextBuffer.CreateEdit())
			{
				List<ITrackingPoint> endingSelections = new List<ITrackingPoint>();

				int selectionIndex = 0;
				foreach (VirtualSnapshotSpan vspan in view.Selection.VirtualSelectedSpans)
				{
					endingSelections.Add(view.TextSnapshot.CreateTrackingPoint(vspan.Start.Position, PointTrackingMode.Negative));
					string indentation = editorOps.GetWhitespaceForVirtualSpace(vspan.Start);
					edit.Replace(vspan.SnapshotSpan, indentation + $"{startingIndex + selectionIndex*step}");
					selectionIndex++;
				}

				view.Caret.MoveTo(endingSelections[0].GetPoint(view.TextSnapshot));

				edit.Apply();
			}
		}

		public static void GenerateNumbers0_Command(IWpfTextView view, IEditorOperations editorOps)
		{
			GenerateNumbers(view, editorOps, 0, 1);
		}
		public static void GenerateNumbers1_Command(IWpfTextView view, IEditorOperations editorOps)
		{
			GenerateNumbers(view, editorOps, 1, 1);
		}
		public static void GenerateNumbersDialog_Command(IServiceProvider serviceProvider, IWpfTextView view, IEditorOperations editorOps)
		{
			int result = VsShellUtilities.ShowMessageBox(serviceProvider, "This command is currently unimplemented", "Unimplemented", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			Console.WriteLine($"ShowMessageBox result: {result}");
		}
	}
}

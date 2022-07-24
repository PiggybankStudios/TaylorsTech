using Community.VisualStudio.Toolkit;
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

		public static void OpenSublime_Command(IWpfTextView view, IEditorOperations editorOps)
        {
			DocumentView document = view.ToDocumentView();
			System.Diagnostics.Process.Start("subl.exe", $"\"{document.FilePath}\"");
        }
	}
}

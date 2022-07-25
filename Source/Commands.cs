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
		// -------- Helpers --------
		private static void WriteText(IWpfTextView view, IEditorOperations editorOps, Func<VirtualSnapshotSpan, int, string> callback, bool moveCaret = true)
		{
			using (ITextEdit edit = view.TextBuffer.CreateEdit())
			{
				List<ITrackingPoint> endingSelections = new List<ITrackingPoint>();

				int selectionIndex = 0;
				foreach (VirtualSnapshotSpan vspan in view.Selection.VirtualSelectedSpans)
				{
					if (moveCaret) { endingSelections.Add(view.TextSnapshot.CreateTrackingPoint(vspan.Start.Position, PointTrackingMode.Negative)); }
					string indentation = editorOps.GetWhitespaceForVirtualSpace(vspan.Start);
					edit.Replace(vspan.SnapshotSpan, indentation + callback(vspan, selectionIndex));
					selectionIndex++;
				}

				if (moveCaret) { view.Caret.MoveTo(endingSelections[0].GetPoint(view.TextSnapshot)); }

				edit.Apply();
			}
		}
		private static void WriteText(IWpfTextView view, IEditorOperations editorOps, string text, bool moveCaret = true)
		{
			WriteText(view, editorOps, (vspan, sIndex) => text, moveCaret);
		}
		private static void GenerateNumbers(IWpfTextView view, IEditorOperations editorOps, int startingIndex, int step)
		{
			WriteText(view, editorOps, (vspan, sIndex) => $"{startingIndex + (sIndex * step)}", false);
		}
		private static int GetColumnIndexForLeadingStr(string leadingStr, int tabWidth = 4)
		{
			int result = 0;
			for (int cIndex = 0; cIndex < leadingStr.Length; cIndex++)
			{
				if (leadingStr[cIndex] == '\t')
				{
					result += tabWidth;
				}
				else
				{
					result++;
				}
			}
			return result;
		}
		private static int GetColumnIndexForPoint(ITextSnapshot snapshot, SnapshotPoint point, int tabWidth = 4)
		{
			ITextSnapshotLine line = snapshot.GetLineFromPosition(point.Position);
			int charIndex = point.Position - line.Start.Position;
			string charsBeforeCursor = line.GetText().Substring(0, charIndex);
			return GetColumnIndexForLeadingStr(charsBeforeCursor, tabWidth);
		}
		private static string RepeatChar(char c, int count)
		{
			StringBuilder result = new StringBuilder(count);
			for (int cIndex = 0; cIndex < count; cIndex++) { result.Append(c); }
			return result.ToString();
		}
		private static string GetCurrentFilePath(IVsTextView viewAdapter)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			viewAdapter.GetBuffer(out IVsTextLines textLines);
			(textLines as IPersistFileFormat).GetCurFile(out string filePath, out uint formatIndex);
			return filePath;
		}
		private static int GetCurrentLineIndex(IWpfTextView view)
		{
			//.GetContainingLineNumber() is only availabe in 2022+
			return view.Caret.Position.BufferPosition.GetContainingLine().LineNumber;
		}
		private static bool IsAllWhitespace(string str)
		{
			foreach (char c in str)
			{
				if (c != ' ' && c != '\t' && c != '\n' && c != '\r') { return false; }
			}
			return true;
		}

		// -------- Commands --------
		public static void Test_Command(CommandFilter context)
		{
			WriteText(context.View, context.EditorOps, (vspan, selectionIndex) => $"Selection {selectionIndex}!", false);
		}

		public static void InsertTodo_Command(CommandFilter context)
		{
			WriteText(context.View, context.EditorOps, "//###trobbins $TODO [NOCHECKIN]");
		}

		public static void OpenSublime_Command(CommandFilter context)
		{
			string filePath = GetCurrentFilePath(context.ViewAdapter);
			int lineNumber = GetCurrentLineIndex(context.View) + 1;
			System.Diagnostics.Process.Start("subl.exe", $"\"{filePath}\":{lineNumber}");
		}

		public static void GenerateNumbers0_Command(CommandFilter context)
		{
			GenerateNumbers(context.View, context.EditorOps, 0, 1);
		}
		public static void GenerateNumbers1_Command(CommandFilter context)
		{
			GenerateNumbers(context.View, context.EditorOps, 1, 1);
		}
		public static void GenerateNumbersDialog_Command(CommandFilter context)
		{
			int result = VsShellUtilities.ShowMessageBox(context.ServiceProvider, "This command is currently unimplemented", "Unimplemented", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			Console.WriteLine($"ShowMessageBox result: {result}");
		}

		public static void AlignCursors_Command(CommandFilter context)
		{
			//TODO: Make this take into account the user setting for tab width!
			int tabWidth = 4;

			int maxColumn = 0;
			foreach (VirtualSnapshotSpan vspan in context.View.Selection.VirtualSelectedSpans)
			{
				int columnIndex = GetColumnIndexForPoint(vspan.Snapshot, vspan.End.Position, tabWidth);
				if (maxColumn < columnIndex) { maxColumn = columnIndex; }
			}

			WriteText(context.View, context.EditorOps, (vspan, selectionIndex) =>
			{
				int columnIndex = GetColumnIndexForPoint(vspan.Snapshot, vspan.End.Position, tabWidth);
				int numSpacesToInsert = maxColumn - columnIndex;
				return RepeatChar(' ', numSpacesToInsert);
			}, false);
		}

		public static void GotoPrevEmptyLine_Command(CommandFilter context)
		{
			ITextSnapshotLine foundLine = null;
			int currentLineIndex = GetCurrentLineIndex(context.View);
			while (currentLineIndex > 0)
			{
				currentLineIndex--;
				var currentLine = context.View.TextSnapshot.GetLineFromLineNumber(currentLineIndex);
				if (IsAllWhitespace(currentLine.GetText()))
				{
					foundLine = currentLine;
					break;
				}
			}
			if (foundLine != null) { context.View.Caret.MoveTo(foundLine.End); }
			else if (GetCurrentLineIndex(context.View) > 0)
            {
				context.View.Caret.MoveTo(context.View.TextSnapshot.GetLineFromLineNumber(0).Start);
			}
		}
		public static void GotoNextEmptyLine_Command(CommandFilter context)
		{
			int lineCount = context.View.TextSnapshot.LineCount;
			ITextSnapshotLine foundLine = null;
			int currentLineIndex = GetCurrentLineIndex(context.View);
			while (currentLineIndex < lineCount - 1)
			{
				currentLineIndex++;
				var currentLine = context.View.TextSnapshot.GetLineFromLineNumber(currentLineIndex);
				if (IsAllWhitespace(currentLine.GetText()))
				{
					foundLine = currentLine;
					break;
				}
			}
			if (foundLine != null) { context.View.Caret.MoveTo(foundLine.End); }
			else if (GetCurrentLineIndex(context.View) < lineCount - 1)
			{
				context.View.Caret.MoveTo(context.View.TextSnapshot.GetLineFromLineNumber(lineCount - 1).End);
			}
		}
	}
}

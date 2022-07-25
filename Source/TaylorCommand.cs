//using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TaylorsTech
{
	//[Command(TaylorsTechPackage.CommandSetGuidString, TaylorCommand.CommandId)]
	internal sealed class TaylorCommand //: BaseCommand<TaylorCommand>
	{
		public const int CommandId = 0x0100;
		private readonly AsyncPackage ownerPackage;

		public static TaylorCommand Instance { get; private set; }
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider { get { return this.ownerPackage; } }

		private TaylorCommand(AsyncPackage package, OleMenuCommandService commandService)
		{
			if (package == null) { throw new ArgumentNullException(nameof(package)); }
			if (commandService == null) { throw new ArgumentNullException(nameof(commandService)); }

			this.ownerPackage = package;

			var menuCommandID = new CommandID(TaylorsTechPackage.CommandSetGuid, CommandId);
			var menuItem = new MenuCommand(this.Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new TaylorCommand(package, commandService);
		}

		private void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			string title = "You ran the Taylor Command!";
			string message = $"Inside {this.GetType().Name}.Execute()";

			VsShellUtilities.ShowMessageBox(this.ownerPackage, message, title, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}

		//protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		//{
		//	DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
		//	NormalizedSnapshotSpanCollection selections = docView.TextView?.Selection.SelectedSpans;
		//	if (selections == null) { return; }

		//	using (ITextEdit edit = docView.TextBuffer.CreateEdit())
		//	{
		//		string guidString = Guid.NewGuid().ToString();

		//		foreach (SnapshotSpan selection in selections.Reverse())
		//		{
		//			edit.Replace(selection, guidString);
		//		}

		//		edit.Apply();
		//	}
		//}
	}
}

using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
//using Community.VisualStudio.Toolkit;

namespace TaylorsTech
{
    // ToolkitPackage comes from Community.VisualStudio.Toolkit.16 and is an AsyncPackage
    // AsyncPackage is also:
    //   IAsyncServiceProvider
    //   IAsyncServiceProvider2
    //   Microsoft.VisualStudio.Shell.Interop.COMAsyncServiceProvider.IAsyncServiceProvider
    //   IAsyncLoadablePackageInitialize
    //   IAsyncServiceContainer
    //   IVsAsyncToolWindowFactory
    //   IVsAsyncToolWindowFactoryProvider

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(TaylorsTechPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TaylorsTechPackage : AsyncPackage
    {
        /// <summary>
        /// TaylorsTechPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d255f1e7-a810-43ac-9165-e3b82fd6d249";
        public const string CommandSetGuidString = "8c9fce6c-6090-4e26-9e5d-6eaf8dc6ee08";
        public static readonly Guid CommandSetGuid = new Guid(CommandSetGuidString);

        public const uint TestTextviewCommandId = 0x0101;
        public const uint InsertTodoCommandId = 0x0102;
        public const uint OpenSublimeCommandId = 0x0103;
        public const uint GenerateNumbers0CommandId = 0x0104;
        public const uint GenerateNumbers1CommandId = 0x0105;
        public const uint GenerateNumbersDialogCommandId = 0x0106;
        public const uint AlignCursorsCommandId = 0x0107;
        public const uint GotoPrevEmptyLineCommandId = 0x0108;
        public const uint GotoNextEmptyLineCommandId = 0x0109;

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
			//await this.RegisterCommandsAsync();

			Console.WriteLine("Before await...");

			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			Console.WriteLine("After await...");
			await TaylorCommand.InitializeAsync(this);
		}

        #endregion
    }
}

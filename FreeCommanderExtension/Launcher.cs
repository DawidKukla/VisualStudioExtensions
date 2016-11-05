using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using EnvDTE;
using FreeCommanderExtension.Utils;
using Microsoft.VisualStudio;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace FreeCommanderExtension
{
    internal class Launcher
    {
        private readonly DTE _dte;
        private readonly Options _options;

        internal Launcher(DTE dte, Options options)
        {
            _dte = dte;
            _options = options;
        }

        internal void Launch()
        {
            var process = Process.Start(new ProcessStartInfo
                                        {
                                            FileName = GetFileName(),
                                            Arguments = GetArguments()
                                        });

            Thread.Sleep(250);

            if ((process != null) && !process.HasExited)
                NativeMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        private string GetFileName()
        {
            return _options.Path;
        }

        private string GetArguments()
        {
            var argumentsBuilder = new StringBuilder();

            var activePanelCommand = _options.ActivePanel == ActivePanel.Left
                ? FreeCommanderparameters.LEFT_WINDOW
                : FreeCommanderparameters.RIGHT_WINDOW;
            argumentsBuilder.AppendFormat(" /{0} \"{1}\"", activePanelCommand,
                Path.GetDirectoryName(GetActiveItemPath()));

            if (_options.ReuseExistingInstance)
                argumentsBuilder.AppendFormat(" /{0}", FreeCommanderparameters.REUSE_INSTANCE);
            if (_options.CreateNewTabs)
            {
                argumentsBuilder.AppendFormat(" /{0}", FreeCommanderparameters.NEW_TAB);
            }
               

            if (!string.IsNullOrEmpty(_options.CommandLineOptions))
                argumentsBuilder.AppendFormat(" {0}", _options.CommandLineOptions);

            var arguments = argumentsBuilder.ToString();
            return arguments;
        }

        private string GetActiveItemPath()
        {
            string path;
            var selectedItem = _dte.SelectedItems.Item(1);

            if ((selectedItem.Project != null) &&
                (selectedItem.Project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}"))
                path = selectedItem.Project.Properties.Item("FullPath").Value + "\\";
            else if ((selectedItem.Project != null) &&
                     (selectedItem.Project.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}") &&
                     // Excludes Solution Folders.
                     !string.IsNullOrEmpty(selectedItem.Project.FullName))
                path = selectedItem.Project.FullName;
            else if ((selectedItem.ProjectItem != null) &&
                     (
                         (Guid.Parse(selectedItem.ProjectItem.Kind) == VSConstants.GUID_ItemType_PhysicalFile) ||
                         (Guid.Parse(selectedItem.ProjectItem.Kind) == VSConstants.GUID_ItemType_PhysicalFolder)
                     ) &&
                     (selectedItem.ProjectItem.Properties != null) &&
                     (selectedItem.ProjectItem.Properties.Item("FullPath") != null))
                path = selectedItem.ProjectItem.Properties.Item("FullPath").Value.ToString();
            else
                path = _dte.Solution.FullName;

            if (string.IsNullOrEmpty(path))
                return GetDefaultWorkingDirectory();

            return path;
        }

        private string GetDefaultWorkingDirectory()
        {
            var defaultWorkingDirectory = string.IsNullOrEmpty(_options.DefaultWorkingDirectory)
                ? "%HOMEDRIVE%%HOMEPATH%"
                : _options.DefaultWorkingDirectory;
            return Environment.ExpandEnvironmentVariables(defaultWorkingDirectory);
        }
    }
}
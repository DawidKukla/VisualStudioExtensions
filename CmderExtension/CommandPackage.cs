﻿//------------------------------------------------------------------------------
// <copyright file="CommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CmderExtension
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 3)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.Package)]
    [ProvideOptionPage(typeof(Options), "Cmder Launcher", "General", 0, 0, false)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
         Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class CommandPackage : Package
    {
        private IMenuCommandService _menuCommandService;
        private IVsUIShell _uiShell;

        #region Package Members

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            _uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;

            AddMenuCommand(CommandIds.Cmder, HandleCmdrMenuCommand, options => true);
        }

        #endregion

        private void AddMenuCommand(uint commandId, EventHandler invokeHandler, Func<Options, bool> visible)
        {
            var commandID = new CommandID(Guids.MenuGroup, (int) commandId);
            var menuCommand = new OleMenuCommand(invokeHandler, commandID);

            menuCommand.BeforeQueryStatus +=
                (s, e) => { menuCommand.Visible = visible(GetDialogPage(typeof(Options)) as Options); };

            _menuCommandService.AddCommand(menuCommand);
        }

        private void HandleCmdrMenuCommand(object sender, EventArgs ea)
        {
            var dte = GetGlobalService(typeof(DTE)) as DTE;
            var options = GetDialogPage(typeof(Options)) as Options;

            var validationErrors = options.GetValidationErrors();

            if (validationErrors != null)
            {
                DisplayErrorAndSuggestOptions(validationErrors);
                return;
            }

            var launcher = new Launcher(dte, options);

            try
            {
                launcher.Launch();
            }
            catch (Exception e)
            {
                DisplayErrorAndSuggestOptions(e.Message);
            }
        }

        private void DisplayErrorAndSuggestOptions(string errorMessage)
        {
            var comp = Guid.Empty;
            int result;

            _uiShell.ShowMessageBox(
                0,
                ref comp,
                errorMessage,
                "Do you want to visit the Options page for Cmder Extension now?",
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                0,
                out result);

            if (result == 6)
            {
                var optionsCommandId = new CommandID(VSConstants.GUID_VSStandardCommandSet97,
                                           VSConstants.cmdidToolsOptions);
                ((MenuCommandService) _menuCommandService).GlobalInvoke(optionsCommandId, Guids.Options);
            }
        }
    }
}
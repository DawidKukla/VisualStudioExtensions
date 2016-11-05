using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FreeCommanderExtension.Utils;
using Microsoft.VisualStudio.Shell;

namespace FreeCommanderExtension
{
    [Guid(Guids.Options)]
    public class Options : DialogPage
    {
        public Options()
        {
            Path = "FreeCommander.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            ReuseExistingInstance = true;
            ActivePanel=ActivePanel.Right;
            CreateNewTabs = true;
        }

        [Category("Application")]
        [DisplayName("FreeCommander Path")]
        [Description("The full path to the FreeCommander executable file.")]
        public string Path { get; set; }

        [Category("Directories")]
        [DisplayName("Default Working Directory")]
        [Description(
             "FreeCommander will open in this folder in case it cannot guess the working directory from the current Solution Explorer selection. This is passed to the /start command line option."
         )]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Settings")]
        [DisplayName("Command Line Options")]
        [Description(
             "Parameters other than /L /R /T /C to pass to FreeCommander when launching it. Switches provided here take priority over the ones from other options."
         )]
        public string CommandLineOptions { get; set; }

        [Category("Settings")]
        [DisplayName("Reuse Existing Instance")]
        [Description(
             "Activate an already running FreeCommander instance passing it a task to execute. The same as the /C command line option."
         )]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ReuseExistingInstance { get; set; }

        [Category("Settings")]
        [DisplayName("Active Panel")]
        [Description("FreeCommander panel to activate. The same as the /L or /R command line option.")]
        [TypeConverter(typeof(ActivePanelConverter))]
        public ActivePanel ActivePanel { get; set; }


        [Category("Settings")]
        [DisplayName("Create New Tabs")]
        [Description(
             "Create new FreeCommander tabs for the directories to open. The same as the /T command line option.")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool CreateNewTabs { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "FreeCommander Path was not set.";

            if (!Helpers.FileExists(Path))
                return "FreeCommander Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !Helpers.PathExists(DefaultWorkingDirectory))
                return "Default Working Directory points to a non-existent path.";

            if (CommandLineOptions.ContainsParameter("/" + FreeCommanderparameters.LEFT_WINDOW))
                return string.Format("Command Line Options cannot contain the /{0} parameter.",
                    FreeCommanderparameters.LEFT_WINDOW);
            if (CommandLineOptions.ContainsParameter("/" + FreeCommanderparameters.RIGHT_WINDOW))
                return string.Format("Command Line Options cannot contain the /{0} parameter.",
                    FreeCommanderparameters.RIGHT_WINDOW);
            if (CommandLineOptions.ContainsParameter("/" + FreeCommanderparameters.REUSE_INSTANCE))
                return string.Format("Command Line Options cannot contain the /{0} parameter.",
                    FreeCommanderparameters.REUSE_INSTANCE);
            if (CommandLineOptions.ContainsParameter("/" + FreeCommanderparameters.NEW_TAB))
                return string.Format("Command Line Options cannot contain the /{0} parameter.",
                    FreeCommanderparameters.NEW_TAB);

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "FreeCommander.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            CommandLineOptions = null;
            ReuseExistingInstance = true;
            ActivePanel=ActivePanel.Right;
            CreateNewTabs = true;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            var validationErrors = GetValidationErrors();

            if (validationErrors != null)
            {
                MessageBox.Show(validationErrors, "FreeCommander Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.ApplyBehavior = ApplyKind.CancelNoNavigate;
            }
            else
                base.OnApply(e);
        }

        #endregion
    }
}
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExtensionUtils;
using Microsoft.VisualStudio.Shell;

namespace CmderExtension
{
    [Guid(Guids.Options)]
    public class Options : DialogPage
    {
        public Options()
        {
            Path = "cmder.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            ReuseExistingInstance = true;
        }

        [Category("Application")]
        [DisplayName("Cmder Path")]
        [Description("The full path to the Cmder executable file.")]
        public string Path { get; set; }

        [Category("Directories")]
        [DisplayName("Default Working Directory")]
        [Description("Cmder will open in this folder in case it cannot guess the working directory from the current Solution Explorer selection. This is passed to the /start command line option.")]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Settings")]
        [DisplayName("Command Line Options")]
        [Description("Parameters other than /start to pass to Cmder when launching it. Switches provided here take priority over the ones from other options.")]
        public string CommandLineOptions { get; set; }

        [Category("Settings")]
        [DisplayName("Reuse Existing Instance")]
        [Description("Activate an already running Cmder instance passing it a task to execute. The same as the /single command line option.")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ReuseExistingInstance { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "Cmder Path was not set.";

            if (!Helpers.FileExists(Path))
                return "Cmder Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !Helpers.PathExists(DefaultWorkingDirectory))
                return "Default Working Directory points to a non-existent path.";

            if (CommandLineOptions.ContainsParameter("/start"))
                return "Command Line Options cannot contain the /start parameter.";

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "cmder.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            CommandLineOptions = null;
            ReuseExistingInstance = true;
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            var validationErrors = GetValidationErrors();

            if (validationErrors != null)
            {
                MessageBox.Show(validationErrors, "Cmder Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.ApplyBehavior = ApplyKind.CancelNoNavigate;
            }
            else
                base.OnApply(e);
        }

        #endregion
    }
}

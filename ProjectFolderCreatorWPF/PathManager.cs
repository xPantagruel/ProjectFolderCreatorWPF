using System;
using System.IO;
using System.Windows.Controls;

namespace ProjectFolderCreatorWPF
{
    public class PathManager
    {
        public string GetDefaultTemplatesPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Templates"));
        }

        public void BrowseForFolder(TextBox targetTextBox)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                targetTextBox.Text = folderDialog.SelectedPath;
            }
        }
    }
}

using System;
using System.IO;
using System.Windows;

namespace ProjectFolderCreatorWPF
{
    public partial class MainWindow : Window
    {
        private TemplateManager _templateManager;
        private PathManager _pathManager;

        public MainWindow()
        {
            InitializeComponent();
            _pathManager = new PathManager();
            _templateManager = new TemplateManager(StatusTextBlock, TemplateComboBox, TemplatesPathTextBox);

            _templateManager.SetDefaultTemplatesPath();
        }

        // Event handlers
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            _pathManager.BrowseForFolder(BasePathTextBox);
        }

        private void BrowseTemplatesButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select the Templates Folder",
                ShowNewFolderButton = true,
                SelectedPath = TemplatesPathTextBox.Text.Trim()
            };

            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TemplatesPathTextBox.Text = folderDialog.SelectedPath;
                _templateManager.LoadTemplates(folderDialog.SelectedPath);
            }
        }


        private void ShowTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            _templateManager.ShowTemplate();
        }

        private void EditTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            _templateManager.EditTemplate();
        }

        private void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            _templateManager.CreateTemplate();
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            string projectName = ProjectNameTextBox.Text.Trim();
            string clientName = ClientNameTextBox.Text.Trim();
            string basePath = BasePathTextBox.Text.Trim();
            string templateName = TemplateComboBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(basePath))
            {
                StatusTextBlock.Text = "Please fill in all required fields.";
                return;
            }

            string projectPath = Path.Combine(basePath, $"{clientName}_{projectName}");
            string templatesFolderPath = TemplatesPathTextBox.Text.Trim();

            _templateManager.CreateProject(projectPath, templateName, templatesFolderPath);
        }

        // Help button click handler to show help information
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage = "How to Use the Project Folder Creator:\n\n" +
                                 "1. Enter the project name and client name.\n" +
                                 "2. Select the base path where the project folders should be created.\n" +
                                 "3. Browse and select the templates folder if needed, or use the default.\n" +
                                 "4. Choose a template from the list. You can also view, edit, or create new templates.\n" +
                                 "5. Click 'Create Project' to generate the folder structure.\n\n" +
                                 "Additional Features:\n" +
                                 "- 'Show Template': Displays the selected template's folder structure.\n" +
                                 "- 'Edit Template': Allows editing of the folder names in the selected template.\n" +
                                 "- 'Create Template': Adds a new template with a custom folder structure.";
            MessageBox.Show(helpMessage, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

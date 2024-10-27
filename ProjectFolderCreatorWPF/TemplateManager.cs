using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFolderCreatorWPF
{
    public class TemplateManager
    {
        private TextBlock _statusTextBlock;
        private ComboBox _templateComboBox;
        private TextBox _templatesPathTextBox;

        public TemplateManager(TextBlock statusTextBlock, ComboBox templateComboBox, TextBox templatesPathTextBox)
        {
            _statusTextBlock = statusTextBlock;
            _templateComboBox = templateComboBox;
            _templatesPathTextBox = templatesPathTextBox;
        }

        // Method to set the default templates path and create the directory if needed
        public void SetDefaultTemplatesPath()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string defaultTemplatesPath = Path.Combine(exeDirectory, "Templates");

            if (!Directory.Exists(defaultTemplatesPath))
            {
                Directory.CreateDirectory(defaultTemplatesPath);
                _statusTextBlock.Text = "Templates folder was not found, so it has been created.";
            }
            else
            {
                _statusTextBlock.Text = "Templates loaded from the default location.";
            }

            _templatesPathTextBox.Text = defaultTemplatesPath;

            LoadTemplates(defaultTemplatesPath);
        }

        // Method to load all templates from a specified folder
        public void LoadTemplates(string templatesFolderPath)
        {
            try
            {
                var templateFiles = Directory.GetFiles(templatesFolderPath, "*.json");
                _templateComboBox.Items.Clear();

                foreach (var templateFile in templateFiles)
                {
                    _templateComboBox.Items.Add(Path.GetFileNameWithoutExtension(templateFile));
                }

                if (_templateComboBox.Items.Count > 0)
                {
                    _templateComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _statusTextBlock.Text = $"Error loading templates: {ex.Message}";
            }
        }

        // Method to show the content of the selected template
        public void ShowTemplate()
        {
            string templateName = _templateComboBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(templateName))
            {
                _statusTextBlock.Text = "Please select a template to view.";
                return;
            }

            string templatesFolderPath = _templatesPathTextBox.Text.Trim();
            string templateFilePath = Path.Combine(templatesFolderPath, templateName + ".json");

            if (!File.Exists(templateFilePath))
            {
                _statusTextBlock.Text = $"Template file '{templateFilePath}' not found.";
                return;
            }

            try
            {
                string templateContent = File.ReadAllText(templateFilePath);
                MessageBox.Show(templateContent, $"Template: {templateName}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _statusTextBlock.Text = $"Error displaying template: {ex.Message}";
            }
        }

        // Method to edit the selected template
        public void EditTemplate()
        {
            string templateName = _templateComboBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(templateName))
            {
                _statusTextBlock.Text = "Please select a template to edit.";
                return;
            }

            string templatesFolderPath = _templatesPathTextBox.Text.Trim();
            string templateFilePath = Path.Combine(templatesFolderPath, templateName + ".json");

            if (!File.Exists(templateFilePath))
            {
                _statusTextBlock.Text = $"Template file '{templateFilePath}' not found.";
                return;
            }

            try
            {
                string templateContent = File.ReadAllText(templateFilePath);
                var templateData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(templateContent);

                if (templateData == null || !templateData.ContainsKey("ProjectRoot"))
                {
                    _statusTextBlock.Text = "Invalid template format.";
                    return;
                }

                List<string> folderNames = templateData["ProjectRoot"];
                string folderNamesInput = string.Join(", ", folderNames);
                string editedFolderNames = Microsoft.VisualBasic.Interaction.InputBox("Edit the folder names (comma separated):", "Edit Template", folderNamesInput);

                if (string.IsNullOrWhiteSpace(editedFolderNames))
                {
                    _statusTextBlock.Text = "Template editing canceled.";
                    return;
                }

                List<string> updatedFolderNames = new List<string>(
                    editedFolderNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                );
                templateData["ProjectRoot"] = updatedFolderNames;

                string updatedJsonContent = JsonConvert.SerializeObject(templateData, Formatting.Indented);
                File.WriteAllText(templateFilePath, updatedJsonContent);

                _statusTextBlock.Text = $"Template '{templateName}' edited successfully.";
            }
            catch (Exception ex)
            {
                _statusTextBlock.Text = $"Error editing template: {ex.Message}";
            }
        }

        // Method to create a new template
        public void CreateTemplate()
        {
            string newTemplateName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name for the new template:", "Create New Template", "NewTemplate");
            if (string.IsNullOrWhiteSpace(newTemplateName))
            {
                _statusTextBlock.Text = "Template creation canceled.";
                return;
            }

            string folderNamesInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the folder names (comma separated):", "Folder Structure", "Documentation, Materials, Reports");
            if (string.IsNullOrWhiteSpace(folderNamesInput))
            {
                _statusTextBlock.Text = "No folder structure provided. Template creation canceled.";
                return;
            }

            List<string> folderStructure = new List<string>(folderNamesInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            var templateData = new Dictionary<string, List<string>> { { "ProjectRoot", folderStructure } };
            string templatesFolderPath = _templatesPathTextBox.Text.Trim();
            string newTemplateFilePath = Path.Combine(templatesFolderPath, newTemplateName + ".json");

            try
            {
                string jsonContent = JsonConvert.SerializeObject(templateData, Formatting.Indented);
                File.WriteAllText(newTemplateFilePath, jsonContent);

                LoadTemplates(templatesFolderPath);
                _statusTextBlock.Text = $"Template '{newTemplateName}' created successfully.";
            }
            catch (Exception ex)
            {
                _statusTextBlock.Text = $"Error creating template: {ex.Message}";
            }
        }

        // Method to create project folders based on a selected template
        public void CreateProject(string projectPath, string templateName, string templatesFolderPath)
        {
            string configFilePath = Path.Combine(templatesFolderPath, templateName + ".json");

            if (!File.Exists(configFilePath))
            {
                _statusTextBlock.Text = $"Template file '{configFilePath}' not found.";
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(configFilePath);
                var templateData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonContent);

                if (templateData == null || !templateData.ContainsKey("ProjectRoot"))
                {
                    _statusTextBlock.Text = "Invalid template format.";
                    return;
                }

                List<string> folderTemplate = templateData["ProjectRoot"];
                string existingFolders = string.Empty;
                string createdFolders = string.Empty;

                foreach (var folder in folderTemplate)
                {
                    string sanitizedFolderName = folder.Trim();
                    string folderPath = Path.Combine(projectPath, sanitizedFolderName);

                    if (Directory.Exists(folderPath))
                    {
                        existingFolders += $"{sanitizedFolderName}, ";
                    }
                    else
                    {
                        Directory.CreateDirectory(folderPath);
                        createdFolders += $"{sanitizedFolderName}, ";
                    }
                }

                string message = "Project folders created successfully.";
                if (!string.IsNullOrEmpty(createdFolders))
                {
                    message += $"\nCreated folders: {createdFolders.TrimEnd(',', ' ')}";
                }
                if (!string.IsNullOrEmpty(existingFolders))
                {
                    message += $"\nAlready existing folders: {existingFolders.TrimEnd(',', ' ')}";
                }

                _statusTextBlock.Text = message;
            }
            catch (Exception ex)
            {
                _statusTextBlock.Text = $"Error creating project folders: {ex.Message}";
            }
        }
    }
}

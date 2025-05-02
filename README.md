# YamlTreeEditor

**YamlTreeEditorControl** is an extensible WinForms component for viewing, editing, and managing YAML files in a structured tree format. It is designed to offer a user-friendly alternative to raw text-based editing, with support for advanced YAML features like sequences, mappings, inline and multi-line comments, drag-and-drop reordering, and visual styling.


<img src="https://github.com/user-attachments/assets/c1dfbec2-e8d1-4ca2-bbfd-44810dcd2713" alt="Alt Text" width="500" height="400">

<img src="https://github.com/user-attachments/assets/006a9825-d692-45e8-ad4f-d48b403452dd" alt="Alt Text" width="400" height="300">


---

## Features

- **TreeView UI** for easy navigation of nested YAML structures
- **Add/Edit/Delete keys and values**
- **Support for sequences (`- value`) and mappings (`key: value`)**
- **Preserve and edit inline and multi-line comments**
- **Color-coded tree display** for keys, values, and comments
-️ **Drag-and-drop** support to rearrange nodes
- **Read-only comment nodes** for multi-line documentation
- **Pretty YAML saving** with aligned comment formatting (unfinished)
- **Change detection** for Save prompt and Save button state tracking
- **Works with .NET 6+**

## References
  https://github.com/aaubry/YamlDotNet
## Usage

You can embed the `YamlTreeEditorControl` into your own Windows Forms application. The control supports:

- Loading a YAML file:
  ```csharp
  yamlTreeEditor.LoadYaml("path/to/file.yaml");

- Saving YAML file:
  ```csharp
  yamlControl.SaveTreeViewToYaml("path/to/file.yaml");

- Detecting changes
  ```csharp
  public TestForm()
  {
  	InitializeComponent();  
  	yamlControl.SaveStateChanged += YamlControl_SaveStateChanged;
  }

  private void YamlControl_SaveStateChanged(object? sender, EventArgs e)
  {
  	buttonSaveYaml.Enabled = yamlControl.IsChanged;
  }

## Structure
- YamlTreeEditorControl.cs — Main UI logic and tree rendering
- YamlTreeNode.cs — Custom tree node class for YAML content
- AddNodeForm.cs — Popup dialog to add/edit keys, values, and comments
- CommentInfo.cs — Tracks structured multi-line comment placement

## Getting Started
- Clone the repo
- Open the solution in Visual Studio
- Add YamlTreeEditorControl to your form
- Ensure you have YamlDotNet installed via NuGet

  ```csharp
  dotnet add package YamlDotNet
## License
MIT License

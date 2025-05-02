using System;
using System.Windows.Forms;
using YamlTreeEditorPro; // Reference your DLL namespace here

namespace YamlTreeEditorTestApp
{
	public partial class TestForm : Form
	{

		public TestForm()
		{
			InitializeComponent();

			yamlControl.SaveStateChanged += YamlControl_SaveStateChanged;
		}

		private void YamlControl_SaveStateChanged(object? sender, EventArgs e)
		{
			buttonSaveYaml.Enabled = yamlControl.IsChanged;
		}

		private void buttonOpenYaml_Click(object sender, EventArgs e)
		{
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*";
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					yamlControl.LoadYaml(dialog.FileName);
				}
			}
		}

		private void buttonSaveYaml_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*";
			saveFileDialog1.Title = "Save an YAML File";
			saveFileDialog1.ShowDialog();

			if (saveFileDialog1.FileName != "")
			{
				yamlControl.SaveTreeViewToYaml(saveFileDialog1.FileName);
			}
		}
	}
}

namespace YamlTreeEditorTestApp
{
	partial class TestForm
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button buttonOpenYaml;
		private YamlTreeEditorPro.YamlTreeEditorControl yamlControl;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			buttonOpenYaml = new Button();
			yamlControl = new YamlTreeEditorPro.YamlTreeEditorControl();
			buttonSaveYaml = new Button();
			SuspendLayout();
			// 
			// buttonOpenYaml
			// 
			buttonOpenYaml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonOpenYaml.Location = new Point(769, 12);
			buttonOpenYaml.Name = "buttonOpenYaml";
			buttonOpenYaml.Size = new Size(148, 43);
			buttonOpenYaml.TabIndex = 1;
			buttonOpenYaml.Text = "Open YAML File";
			buttonOpenYaml.TextImageRelation = TextImageRelation.ImageAboveText;
			buttonOpenYaml.Click += buttonOpenYaml_Click;
			// 
			// yamlControl
			// 
			yamlControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			yamlControl.Location = new Point(1, 1);
			yamlControl.Margin = new Padding(3, 4, 3, 4);
			yamlControl.Name = "yamlControl";
			yamlControl.Size = new Size(762, 562);
			yamlControl.TabIndex = 0;
			// 
			// buttonSaveYaml
			// 
			buttonSaveYaml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonSaveYaml.Enabled = false;
			buttonSaveYaml.Location = new Point(769, 61);
			buttonSaveYaml.Name = "buttonSaveYaml";
			buttonSaveYaml.Size = new Size(148, 43);
			buttonSaveYaml.TabIndex = 2;
			buttonSaveYaml.Text = "Save YAML File";
			buttonSaveYaml.TextImageRelation = TextImageRelation.ImageAboveText;
			buttonSaveYaml.Click += buttonSaveYaml_Click;
			// 
			// TestForm
			// 
			ClientSize = new Size(925, 563);
			Controls.Add(buttonSaveYaml);
			Controls.Add(yamlControl);
			Controls.Add(buttonOpenYaml);
			Name = "TestForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "YamlTreeEditorTestApp";
			ResumeLayout(false);
		}

		private Button buttonSaveYaml;
	}
}

namespace YamlTreeEditorPro
{
	partial class YamlTreeEditorControl
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TreeView treeViewYaml;

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
			treeViewYaml = new TreeView();
			SuspendLayout();
			// 
			// treeViewYaml
			// 
			treeViewYaml.Dock = DockStyle.Fill;
			treeViewYaml.Font = new Font("Arial Rounded MT Bold", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
			treeViewYaml.HideSelection = false;
			treeViewYaml.Location = new Point(0, 0);
			treeViewYaml.Name = "treeViewYaml";
			treeViewYaml.Size = new Size(500, 500);
			treeViewYaml.TabIndex = 0;
			// 
			// YamlTreeEditorControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(treeViewYaml);
			Name = "YamlTreeEditorControl";
			Size = new Size(500, 500);
			ResumeLayout(false);
		}
	}
}

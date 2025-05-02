namespace YamlTreeEditorPro
{
	partial class AddNodeForm
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TextBox textBoxKey;
		private System.Windows.Forms.TextBox textBoxValue;
		private System.Windows.Forms.TextBox textBoxComment;
		private System.Windows.Forms.Label labelKey;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.Label labelComment;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;

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
			textBoxKey = new TextBox();
			textBoxValue = new TextBox();
			textBoxComment = new TextBox();
			labelKey = new Label();
			labelValue = new Label();
			labelComment = new Label();
			buttonOK = new Button();
			buttonCancel = new Button();
			SuspendLayout();
			// 
			// textBoxKey
			// 
			textBoxKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBoxKey.Location = new Point(100, 20);
			textBoxKey.Name = "textBoxKey";
			textBoxKey.Size = new Size(351, 23);
			textBoxKey.TabIndex = 0;
			// 
			// textBoxValue
			// 
			textBoxValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBoxValue.Location = new Point(100, 49);
			textBoxValue.Name = "textBoxValue";
			textBoxValue.Size = new Size(351, 23);
			textBoxValue.TabIndex = 1;
			// 
			// textBoxComment
			// 
			textBoxComment.AcceptsReturn = true;
			textBoxComment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			textBoxComment.Location = new Point(100, 78);
			textBoxComment.Multiline = true;
			textBoxComment.Name = "textBoxComment";
			textBoxComment.ScrollBars = ScrollBars.Vertical;
			textBoxComment.Size = new Size(351, 127);
			textBoxComment.TabIndex = 2;
			// 
			// labelKey
			// 
			labelKey.AutoSize = true;
			labelKey.Location = new Point(20, 23);
			labelKey.Name = "labelKey";
			labelKey.Size = new Size(29, 15);
			labelKey.TabIndex = 3;
			labelKey.Text = "Key:";
			// 
			// labelValue
			// 
			labelValue.AutoSize = true;
			labelValue.Location = new Point(20, 52);
			labelValue.Name = "labelValue";
			labelValue.Size = new Size(38, 15);
			labelValue.TabIndex = 4;
			labelValue.Text = "Value:";
			// 
			// labelComment
			// 
			labelComment.AutoSize = true;
			labelComment.Location = new Point(20, 81);
			labelComment.Name = "labelComment";
			labelComment.Size = new Size(64, 15);
			labelComment.TabIndex = 5;
			labelComment.Text = "Comment:";
			// 
			// buttonOK
			// 
			buttonOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonOK.Location = new Point(468, 18);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(80, 25);
			buttonOK.TabIndex = 6;
			buttonOK.Text = "OK";
			buttonOK.Click += buttonOK_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonCancel.Location = new Point(468, 47);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(80, 25);
			buttonCancel.TabIndex = 7;
			buttonCancel.Text = "Cancel";
			buttonCancel.Click += buttonCancel_Click;
			// 
			// AddNodeForm
			// 
			AcceptButton = buttonOK;
			CancelButton = buttonCancel;
			ClientSize = new Size(561, 222);
			ControlBox = false;
			Controls.Add(textBoxKey);
			Controls.Add(textBoxValue);
			Controls.Add(textBoxComment);
			Controls.Add(labelKey);
			Controls.Add(labelValue);
			Controls.Add(labelComment);
			Controls.Add(buttonOK);
			Controls.Add(buttonCancel);
			Name = "AddNodeForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Add / Edit Node";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}

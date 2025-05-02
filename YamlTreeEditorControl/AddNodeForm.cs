using System;
using System.Windows.Forms;

namespace YamlTreeEditorPro
{
	public partial class AddNodeForm : Form
	{
		public string NodeKey { get; private set; }
		public string NodeValue { get; private set; }
		public string NodeComment { get; private set; }

		private bool isSequenceItem;

		public AddNodeForm(bool isSequence = false)
		{
			InitializeComponent();
			isSequenceItem = isSequence;
			if (isSequenceItem)
			{
				textBoxKey.Enabled = false;
				textBoxKey.Text = "(sequence item)";
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (!isSequenceItem && string.IsNullOrWhiteSpace(textBoxKey.Text))
			{
				MessageBox.Show("Key cannot be empty for a mapping item.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			NodeKey = isSequenceItem ? null : textBoxKey.Text.Trim();
			NodeValue = textBoxValue.Text.Trim();

			var commentLines = textBoxComment.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			NodeComment = string.Join("\n", commentLines.Select(line =>
			{
				if (line.StartsWith("#"))
					return line.Substring(1); // Remove leading '#' only
				else
					return line;
			}));

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}

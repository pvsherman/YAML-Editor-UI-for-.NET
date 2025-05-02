using System.Windows.Forms;


public class YamlTreeNode : TreeNode
{
	public string Key { get; set; }
	public string ValueData { get; set; }
	public string Comment { get; set; }
	public bool IsEmptyLine { get; set; } = false;

	public YamlTreeNode(string key, string value, string comment, int startingPosition = 0)
	{
		Key = key;
		ValueData = value;
		Comment = comment;
		IsEmptyLine = string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value);
		UpdateText(startingPosition);
	}

	public YamlTreeNode(string key, string value, string comment, int startingPosition, bool isEmptyLine)
	{
		Key = key;
		ValueData = value;
		Comment = comment;
		IsEmptyLine = isEmptyLine;
		UpdateText(startingPosition);
	}

	public void UpdateText(int commentAlignCol = 0)
	{
		if (IsEmptyLine)
		{
			Text = string.IsNullOrWhiteSpace(Comment) ? "" : $"# {Comment}";
		}
		else if (string.IsNullOrEmpty(Key))
		{
			var baseText = $"- {ValueData}";
			Text = !string.IsNullOrWhiteSpace(Comment) ? baseText + "  # " + Comment : baseText;
		}
		else
		{
			var baseText = $"{Key}: {ValueData}";
			if (!string.IsNullOrWhiteSpace(Comment))
			{
				Text = baseText + "  # " + Comment;
			}
			else
			{
				Text = baseText;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YamlDotNet.RepresentationModel;

namespace YamlTreeEditorPro
{
	public partial class YamlTreeEditorControl : UserControl
	{
		private string[] rawYamlLines;		private ContextMenuStrip contextMenu;

		private Dictionary<int, List<CommentInfo>> commentAnnotations;
		private int latestStartingPosition = 0;
		
		private Dictionary<string, List<CommentInfo>> keyCommentMap = new();

		private string _savedTreeImage = "";

		public event EventHandler SaveStateChanged;
		private bool suppressCollapse = false;

		private void NotifyTreeChanged()
		{
			SaveStateChanged?.Invoke(this, EventArgs.Empty);
		}
				
		public bool IsChanged
		{
			get
			{
				string current = SerializeTreeViewToString();

				if (_savedTreeImage != current)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public YamlTreeEditorControl()
		{
			InitializeComponent();
			InitializeContextMenu();

			treeViewYaml.AllowDrop = true;
			treeViewYaml.DrawMode = TreeViewDrawMode.OwnerDrawText;

			treeViewYaml.ItemDrag += TreeViewYaml_ItemDrag;
			treeViewYaml.DragEnter += TreeViewYaml_DragEnter;			
			
			treeViewYaml.DrawNode += TreeViewYaml_DrawNode;
			
			treeViewYaml.DragOver += TreeViewYaml_DragOver;
			treeViewYaml.DragDrop += TreeViewYaml_DragDrop;

			treeViewYaml.BeforeCollapse += TreeViewYaml_BeforeCollapse;
			
			treeViewYaml.NodeMouseDoubleClick += TreeViewYaml_NodeMouseDoubleClick;
			treeViewYaml.MouseDown += TreeViewYaml_MouseDown;
		}

		private void TreeViewYaml_MouseDown(object sender, MouseEventArgs e)
		{
			suppressCollapse = true;
		}

		private void TreeViewYaml_BeforeCollapse(object? sender, TreeViewCancelEventArgs e)
		{
			if (suppressCollapse)
			{
				e.Cancel = true;

				suppressCollapse = false;
			}				
		}

		public void LoadYaml(string filePath)
		{
			if (File.Exists(filePath))
			{
				treeViewYaml.Nodes.Clear();
				var yaml = File.ReadAllText(filePath);
				rawYamlLines = yaml.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
				BuildKeyCommentMap();

				var yamlStream = new YamlStream();
				yamlStream.Load(new StringReader(yaml));

				if (yamlStream.Documents.Count > 0)
				{
					PopulateTreeView(yamlStream.Documents[0].RootNode, treeViewYaml.Nodes, "");
					treeViewYaml.ExpandAll();
				}

				_savedTreeImage = SerializeTreeViewToString();
			}
		}


		private void InitializeContextMenu()
		{
			contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("Add Key", null, AddKey_Click);
			contextMenu.Items.Add("Add List Item", null, AddListItem_Click);
			contextMenu.Items.Add("Delete Node", null, DeleteNode_Click);
			contextMenu.Items.Add("Add Top-Level Key", null, AddTopLevelKey_Click);

			treeViewYaml.ContextMenuStrip = contextMenu;
		}

		private void AddTopLevelKey_Click(object sender, EventArgs e)
		{
			var dialog = new AddNodeForm(isSequence: false);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var newNode = new YamlTreeNode(dialog.NodeKey, dialog.NodeValue, dialog.NodeComment, 0);
				treeViewYaml.Nodes.Add(newNode);
				newNode.UpdateText();
				treeViewYaml.Invalidate();

				NotifyTreeChanged();
			}
		}

		private void AddKey_Click(object sender, EventArgs e)
		{
			var dialog = new AddNodeForm(false);
			if (dialog.ShowDialog() != DialogResult.OK) return;

			var key = dialog.NodeKey;
			var value = dialog.NodeValue;
			var commentLines = (dialog.NodeComment ?? "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			var mainNode = new YamlTreeNode(key, value, commentLines.FirstOrDefault() ?? "");
			for (int i = 1; i < commentLines.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(commentLines[i]))
				{
					mainNode.Nodes.Add(new YamlTreeNode(null, null, commentLines[i], 0, true));
				}
			}

			if (treeViewYaml.SelectedNode is YamlTreeNode selected && !selected.IsEmptyLine)
			{
				selected.Nodes.Add(mainNode);
				selected.Expand();
			}
			else
			{
				treeViewYaml.Nodes.Add(mainNode);
			}
			treeViewYaml.SelectedNode = mainNode;

			NotifyTreeChanged();
		}
				
		private void AddListItem_Click(object sender, EventArgs e)
		{
			var dialog = new AddNodeForm(true);
			if (dialog.ShowDialog() != DialogResult.OK) return;

			var value = dialog.NodeValue;
			var commentLines = (dialog.NodeComment ?? "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			var mainNode = new YamlTreeNode(null, value, commentLines.FirstOrDefault() ?? "");
			for (int i = 1; i < commentLines.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(commentLines[i]))
				{
					mainNode.Nodes.Add(new YamlTreeNode(null, null, commentLines[i], 0, true));
				}
			}

			if (treeViewYaml.SelectedNode is YamlTreeNode selected && !selected.IsEmptyLine)
			{
				selected.Nodes.Add(mainNode);
				selected.Expand();
			}
			else
			{
				treeViewYaml.Nodes.Add(mainNode);
			}
			treeViewYaml.SelectedNode = mainNode;

			NotifyTreeChanged();
		}


		private void DeleteNode_Click(object sender, EventArgs e)
		{
			var selectedNode = treeViewYaml.SelectedNode;
			if (selectedNode != null)
			{
				selectedNode.Remove();
				NotifyTreeChanged();
			}
		}
		
		private void TreeViewYaml_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			TreeView tree = sender as TreeView;
			bool isSelected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
			//--------------------------------------------------------------------------------

			e.DrawDefault = false;
			if (e.Node is not YamlTreeNode node)
			{
				e.Graphics.DrawString(e.Node.Text, treeViewYaml.Font, Brushes.Black, e.Bounds);
				return;
			}

			var g = e.Graphics;
			var bounds = e.Bounds;
			var font = treeViewYaml.Font;

			if (string.IsNullOrWhiteSpace(node.Text)) return;

			var keyBrush = Brushes.DarkBlue;
			var valueBrush = Brushes.Maroon;
			var commentBrush = Brushes.ForestGreen;

			float x = bounds.X;
			float y = bounds.Y;

			if (isSelected)
			{
				e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds); // custom color
				TextRenderer.DrawText(e.Graphics, e.Node.Text, tree.Font, e.Bounds, Color.Black);
			}
			else { 

				if (!string.IsNullOrEmpty(node.Key))
				{
					string keyLabel = node.Key + ": ";
					g.DrawString(keyLabel, font, keyBrush, x, y);
					x += TextRenderer.MeasureText(keyLabel, font).Width;
					g.DrawString(node.ValueData, font, valueBrush, x, y);
					x += TextRenderer.MeasureText(node.ValueData, font).Width;
				}
				else if (!string.IsNullOrEmpty(node.ValueData))
				{
					g.DrawString("- ", font, keyBrush, x, y);
					x += TextRenderer.MeasureText("- ", font).Width;
					g.DrawString(node.ValueData, font, valueBrush, x, y);
					x += TextRenderer.MeasureText(node.ValueData, font).Width;
				}

				if (!string.IsNullOrWhiteSpace(node.Comment))
				{
					g.DrawString("  # " + node.Comment, font, commentBrush, x, y);
				}
			}
		}

		private void TreeViewYaml_DragOver(object sender, DragEventArgs e)
		{
			Point targetPoint = treeViewYaml.PointToClient(new Point(e.X, e.Y));
			TreeNode targetNode = treeViewYaml.GetNodeAt(targetPoint);

			treeViewYaml.SelectedNode = targetNode;
			e.Effect = DragDropEffects.Move;
		}

		private void TreeViewYaml_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if(e.Item != null)
				{
					//TODO - Remove
					System.Diagnostics.Debug.WriteLine("Dragging started...");
					DoDragDrop(e.Item, DragDropEffects.Move);
					NotifyTreeChanged();
				}				
			}
		}

		private void TreeViewYaml_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void TreeViewYaml_DragDrop(object sender, DragEventArgs e)
		{
			Point targetPoint = treeViewYaml.PointToClient(new Point(e.X, e.Y));
			TreeNode targetNode = treeViewYaml.GetNodeAt(targetPoint);

			var draggedNode = e.Data.GetData(typeof(YamlTreeNode)) as YamlTreeNode;
			if (draggedNode == null || draggedNode == targetNode)
				return;

			// Prevent dropping into itself
			TreeNode temp = targetNode;
			while (temp != null)
			{
				if (temp == draggedNode)
					return;
				temp = temp.Parent;
			}

			// Remove from old parent
			TreeNodeCollection oldCollection = draggedNode.Parent?.Nodes ?? treeViewYaml.Nodes;
			oldCollection.Remove(draggedNode);

			// Add to new location
			if (targetNode is YamlTreeNode targetYaml && !targetYaml.IsEmptyLine)
			{
				// Drop onto a real node → become its child
				targetNode.Nodes.Add(draggedNode);
				targetNode.Expand();
			}
			else if (targetNode != null && targetNode.Parent != null)
			{
				// Drop between siblings
				TreeNodeCollection siblings = targetNode.Parent.Nodes;
				int index = targetNode.Index;
				siblings.Insert(index, draggedNode);
			}
			else
			{
				// Drop on empty space → add to root
				treeViewYaml.Nodes.Add(draggedNode);
			}

			treeViewYaml.SelectedNode = draggedNode;
			treeViewYaml.Invalidate();

			NotifyTreeChanged();
		}


		private YamlNode BuildYamlFromTree(TreeNodeCollection nodes)
		{
			if (nodes.Count == 0)
				return new YamlScalarNode("");

			bool isSequence = nodes.Cast<TreeNode>().All(n => n is YamlTreeNode y && string.IsNullOrEmpty(y.Key));

			if (isSequence)
			{
				var sequenceNode = new YamlSequenceNode();
				foreach (TreeNode node in nodes)
				{
					if (node is YamlTreeNode yamlNode)
					{
						if (yamlNode.Nodes.Count > 0)
							sequenceNode.Add(BuildYamlFromTree(yamlNode.Nodes));
						else
							sequenceNode.Add(new YamlScalarNode(yamlNode.ValueData ?? ""));
					}
				}
				return sequenceNode;
			}
			else
			{
				var mappingNode = new YamlMappingNode();
				foreach (TreeNode node in nodes)
				{
					if (node is YamlTreeNode yamlNode)
					{
						if (yamlNode.Nodes.Count > 0)
							mappingNode.Add(new YamlScalarNode(yamlNode.Key), BuildYamlFromTree(yamlNode.Nodes));
						else
							mappingNode.Add(new YamlScalarNode(yamlNode.Key), new YamlScalarNode(yamlNode.ValueData ?? ""));
					}
				}
				return mappingNode;
			}
		}
		private void BuildKeyCommentMap()
		{
			keyCommentMap.Clear();
			var stack = new Stack<(string keyPath, int indent)>();
			var currentKey = "";

			using var g = Graphics.FromImage(new Bitmap(1, 1));

			for (int i = 0; i < rawYamlLines.Length; i++)
			{
				string line = rawYamlLines[i];
				if (string.IsNullOrWhiteSpace(line)) continue;

				int indent = line.TakeWhile(char.IsWhiteSpace).Count();
				string trimmed = line.Trim();

				if (trimmed.StartsWith("#"))
				{
					if (!string.IsNullOrEmpty(currentKey))
					{
						var comment = trimmed.Substring(1).Trim();
						float space = g.MeasureString("", treeViewYaml.Font).Width;
						keyCommentMap[currentKey].Add(new CommentInfo { Text = comment, StartingPosition = (int)space });
					}
					continue;
				}

				if (!trimmed.Contains(":")) continue;
				while (stack.Count > 0 && indent <= stack.Peek().indent)
				{
					stack.Pop();
				}

				string key = trimmed.Split(':')[0].Trim();
				string parentPath = string.Join(".", stack.Reverse().Select(s => s.keyPath));
				currentKey = string.IsNullOrEmpty(parentPath) ? key : parentPath + "." + key;

				stack.Push((key, indent));

				int hashIndex = line.IndexOf("#");
				if (hashIndex >= 0)
				{
					string comment = line.Substring(hashIndex + 1).Trim();
					float space = g.MeasureString(line.Substring(0, hashIndex) + "  ", treeViewYaml.Font).Width;
					keyCommentMap.TryAdd(currentKey, new List<CommentInfo>());
					keyCommentMap[currentKey].Add(new CommentInfo { Text = comment, StartingPosition = (int)space });
				}
				else
				{
					keyCommentMap.TryAdd(currentKey, new List<CommentInfo>());
				}
			}
		}
		private void PopulateTreeView(YamlNode node, TreeNodeCollection nodes, string parentPath)
		{
			switch (node)
			{
				case YamlMappingNode mappingNode:
					foreach (var entry in mappingNode.Children)
					{
						var key = ((YamlScalarNode)entry.Key).Value ?? "";
						var fullPath = string.IsNullOrEmpty(parentPath) ? key : parentPath + "." + key;

						if (entry.Value is YamlScalarNode scalarValue)
						{
							var value = scalarValue.Value ?? "";
							var comments = GetCommentsForKey(fullPath, out int startCol);

							var mainNode = new YamlTreeNode(key, value, comments.Count > 0 ? comments[0].Text : "", startCol);
							nodes.Add(mainNode);

							foreach (var extra in comments.Skip(1))
							{
								var commentNode = new YamlTreeNode(null, null, extra.Text, extra.StartingPosition)
								{
									IsEmptyLine = true
								};
								mainNode.Nodes.Add(commentNode);
							}
						}
						else
						{
							var comments = GetCommentsForKey(fullPath, out int startCol);

							var firstComment = "";
							if(comments.Count > 0)
							{
								firstComment = comments[0].Text;
							}

							var parentNode = new YamlTreeNode(key, "", firstComment);
							nodes.Add(parentNode);							

							foreach (var extra in comments.Skip(1))
							{
								var commentNode = new YamlTreeNode(null, null, extra.Text, extra.StartingPosition)
								{
									IsEmptyLine = true
								};
								parentNode.Nodes.Add(commentNode);
							}

							PopulateTreeView(entry.Value, parentNode.Nodes, fullPath);
						}
					}
					break;

				case YamlSequenceNode sequenceNode:
					int idx = 0;
					foreach (var child in sequenceNode.Children)
					{
						var itemPath = parentPath + $"[{idx}]";

						if (child is YamlScalarNode scalarItem)
						{
							var value = scalarItem.Value ?? "";
							var comments = GetCommentsForKey(itemPath, out int startCol);

							var mainNode = new YamlTreeNode(null, value, comments.Count > 0 ? comments[0].Text : "", startCol)
							{
								Text = $"[{idx++}]: {value}"
							};
							nodes.Add(mainNode);

							foreach (var extra in comments.Skip(1))
							{
								var commentNode = new YamlTreeNode(null, null, extra.Text, extra.StartingPosition)
								{
									IsEmptyLine = true
								};
								mainNode.Nodes.Add(commentNode);
							}
						}
						else
						{
							var parentNode = new YamlTreeNode(null, "", "")
							{
								Text = $"[{idx++}]"
							};
							nodes.Add(parentNode);
							PopulateTreeView(child, parentNode.Nodes, itemPath);
						}
					}
					break;
			}
		}

		private List<CommentInfo> GetCommentsForKey(string keyPath, out int startCol)
		{
			startCol = 0;
			if (keyCommentMap.TryGetValue(keyPath, out var list) && list.Count > 0)
			{
				startCol = list[0].StartingPosition;
				return list;
			}
			return new List<CommentInfo>();
		}


		private string ExtractInlineComment(string[] rawYaml, string keyOrDash, string value)
		{
			var lines = rawYamlLines;
			var commentBuilder = new StringBuilder();
			bool foundMainLine = false;

			string searchPattern;
			if (keyOrDash == "- ")
				searchPattern = "- " + value;
			else
				searchPattern = keyOrDash + ": " + value;

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();

				if (!foundMainLine)
				{
					if (line.StartsWith(searchPattern))
					{
						foundMainLine = true;

						int commentStart = line.IndexOf("#");
						if (commentStart >= 0)
							commentBuilder.Append(line.Substring(commentStart + 1).Trim());
					}
				}
				else
				{
					if (line.StartsWith("#"))
					{
						if (commentBuilder.Length > 0)
							commentBuilder.AppendLine();
						commentBuilder.Append(line.Substring(1).Trim());
					}
					else if (string.IsNullOrWhiteSpace(line))
					{
						// allow blank
					}
					else
					{
						break;
					}
				}
			}

			return commentBuilder.ToString();
		}
		private void InjectComments(ref string yamlText, YamlTreeNode node)
		{
			if (!string.IsNullOrWhiteSpace(node.Comment))
			{
				string searchLine;
				if (string.IsNullOrEmpty(node.Key))
				{
					searchLine = "- " + node.ValueData;
					if (!yamlText.Contains(searchLine))
						searchLine = "- \"" + node.ValueData + "\"";
				}
				else
				{
					searchLine = node.Key + ": " + node.ValueData;
					if (!yamlText.Contains(searchLine))
						searchLine = node.Key + ": \"" + node.ValueData + "\"";
				}

				if (yamlText.Contains(searchLine))
				{
					var commentLines = node.Comment.Split(new[] { '\n' }, StringSplitOptions.None);

					if (commentLines.Length == 1)
					{
						string commentLine = $"{searchLine}  # {commentLines[0]}";
						yamlText = yamlText.Replace(searchLine, commentLine);
					}
					else
					{
						// Multiline: replace with key/value, then separate lines of comments
						string injected = searchLine;
						foreach (var c in commentLines)
						{
							injected += Environment.NewLine + "  # " + c.Trim();
						}
						yamlText = yamlText.Replace(searchLine, injected);
					}
				}
			}

			foreach (TreeNode child in node.Nodes)
			{
				if (child is YamlTreeNode yamlChild)
				{
					InjectComments(ref yamlText, yamlChild);
				}
			}
		}

		private void TreeViewYaml_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{



			if (e.Node is YamlTreeNode clickedNode)
			{
				YamlTreeNode targetNode = clickedNode;

				if (clickedNode.IsEmptyLine)
				{
					// Find the parent node if clicking on comment node
					if (clickedNode.Parent is YamlTreeNode parentNode)
						targetNode = parentNode;
					else
						return;
				}

				bool isSequence = string.IsNullOrEmpty(targetNode.Key);
				var dialog = new AddNodeForm(isSequence);

				if (!isSequence)
					dialog.Controls["textBoxKey"].Text = targetNode.Key;

				dialog.Controls["textBoxValue"].Text = targetNode.ValueData;

				// 🔥 Collect inline + child comments
				var allComments = new List<string>();
				if (!string.IsNullOrEmpty(targetNode.Comment))
					allComments.Add(targetNode.Comment);

				foreach (TreeNode child in targetNode.Nodes)
				{
					if (child is YamlTreeNode commentNode && commentNode.IsEmptyLine)
					{
						allComments.Add(commentNode.Comment);
					}
				}

				dialog.Controls["textBoxComment"].Text = string.Join(Environment.NewLine, allComments);

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					if (!isSequence)
						targetNode.Key = dialog.NodeKey;

					targetNode.ValueData = dialog.NodeValue;

					// 🔥 Remove old comment children
					for (int i = targetNode.Nodes.Count - 1; i >= 0; i--)
					{
						if (targetNode.Nodes[i] is YamlTreeNode n && n.IsEmptyLine)
							targetNode.Nodes.RemoveAt(i);
					}

					// 🔥 Update inline comment + children
					var commentLines = dialog.NodeComment?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();

					if (commentLines.Length > 0)
					{
						targetNode.Comment = commentLines[0];

						for (int i = 1; i < commentLines.Length; i++)
						{
							var emptyCommentNode = new YamlTreeNode(null, null, commentLines[i])
							{
								IsEmptyLine = true
							};
							targetNode.Nodes.Add(emptyCommentNode);
						}
					}
					else
					{
						targetNode.Comment = "";
					}

					targetNode.UpdateText();
					treeViewYaml.Invalidate();

					NotifyTreeChanged();
				}
			}
		}	
		

		private void InsertEmptyNodesBeforePopulate(TreeNodeCollection nodes)
		{
			foreach (var line in rawYamlLines)
			{
				string trimmed = line.Trim();
				if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
				{
					var emptyNode = new YamlTreeNode(null, null, trimmed.StartsWith("#") ? trimmed.Substring(1).Trim() : "")
					{
						IsEmptyLine = true
					};
					nodes.Add(emptyNode);
				}
			}
		}

		//public void SaveYaml(string filePath)
		//{
		//	var rootMapping = new YamlMappingNode();
		//	foreach (TreeNode node in treeViewYaml.Nodes)
		//	{
		//		if (node is YamlTreeNode yamlNode)
		//		{
		//			if (yamlNode.IsEmptyLine)
		//				continue;

		//			if (yamlNode.Nodes.Count > 0)
		//				rootMapping.Add(new YamlScalarNode(yamlNode.Key), BuildYamlFromTree(yamlNode.Nodes));
		//			else
		//				rootMapping.Add(new YamlScalarNode(yamlNode.Key), new YamlScalarNode(yamlNode.ValueData ?? ""));
		//		}
		//	}

		//	var yamlStream = new YamlStream(new YamlDocument(rootMapping));
		//	using var sw = new StringWriter();
		//	yamlStream.Save(sw, assignAnchors: false);
		//	var yamlText = sw.ToString();

		//	foreach (TreeNode node in treeViewYaml.Nodes)
		//	{
		//		if (node is YamlTreeNode yamlNode)
		//		{
		//			InjectComments(ref yamlText, yamlNode);
		//		}
		//	}

		//	File.WriteAllText(filePath, yamlText);
		//}

		private string GetInlinePortion(string comment)
		{
			if (string.IsNullOrWhiteSpace(comment)) return "";
			var lines = comment.Split(new[] { '\n' }, StringSplitOptions.None);
			return lines[0];
		}

		public void SaveTreeViewToYaml(string path)
		{

			_savedTreeImage = SerializeTreeViewToString();

			File.WriteAllText(path, _savedTreeImage);
		}

		private string SerializeTreeViewToString()
		{
			var sb = new StringBuilder();
			foreach (TreeNode node in treeViewYaml.Nodes)
			{
				BuildYamlFromNode(node, sb, 0);
			}
			return sb.ToString();
		}

		private void BuildYamlFromNode(TreeNode node, StringBuilder sb, int indentLevel)
		{
			string indent = new string(' ', indentLevel * 2);
			var index = 0;

			if (node is YamlTreeNode yamlNode)
			{
				if (yamlNode.IsEmptyLine)
				{
					if (yamlNode.Parent.Text.Contains("#"))
					{
						index = yamlNode.Parent.Text.IndexOf("#");

						var str = new string(' ', index) + "# " + yamlNode.Comment;

						sb.AppendLine(str);
					}
					else
					{
						var str = new string(' ', yamlNode.Parent.Text.Length + 1) + "# " + yamlNode.Comment;

						sb.AppendLine(str);
					}

					
					return;
				}

				string baseLine = yamlNode.Key == null ?
					$"{indent}- {yamlNode.ValueData}" :
					$"{indent}{yamlNode.Key}: {yamlNode.ValueData}";

				if (!string.IsNullOrWhiteSpace(yamlNode.Comment))
				{
					sb.AppendLine($"{baseLine}  # {yamlNode.Comment}");
				}
				else
				{
					sb.AppendLine(baseLine);
				}

				foreach (TreeNode child in yamlNode.Nodes)
				{
					BuildYamlFromNode(child, sb, indentLevel + 1);
				}
			}
		}
	}
}

namespace YamlTreeEditorPro
{
	public class LineAnnotation
	{
		public int LineNumber { get; set; }
		public bool IsEmpty { get; set; }
		public bool IsComment { get; set; }
		public string CommentText { get; set; }
	}
}

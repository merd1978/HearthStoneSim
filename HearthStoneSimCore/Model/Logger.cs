using System;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
	public enum LogLevel
	{
		DUMP, ERROR, WARNING, INFO, VERBOSE, DEBUG
	}

	public class LogEntry
	{
		public DateTime TimeStamp { get; set; }
		public LogLevel Level { get; set; }
		public BlockType BlockType { get; set; }
		public string Location { get; set; }
		public string Text { get; set; }
	}
}

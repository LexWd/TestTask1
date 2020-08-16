using System;

namespace TestTask1
{
	class NotSupportedTagTypeException : Exception
	{
		public NotSupportedTagTypeException() { }
		public NotSupportedTagTypeException(string message) : base(message) { }
		public NotSupportedTagTypeException(string message, Exception inner) : base(message, inner) { }
	}
}

namespace Humper.Sample.Basic
{
	using System;

	[Flags]
	public enum Tags
	{
		Group1 = 1 << 0,
		Group2 = 1 << 1,
		Group3 = 1 << 2,
		Group4 = 1 << 3,
		Group5 = 1 << 4,
	}
}


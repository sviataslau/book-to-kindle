using System;
using System.Diagnostics;
using Serilog;

namespace BookToKindle.Infrastructure
{
	internal sealed class MeasuredOperation : IDisposable
	{
		private readonly string operation;
		private readonly Stopwatch stopWatch;

		public MeasuredOperation(string operation)
		{
			this.operation = operation;
			this.stopWatch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			this.stopWatch.Stop();
			Log.Information("{@Operation} finished in {@Timespan}", this.operation, this.stopWatch.Elapsed);
		}
	}
}
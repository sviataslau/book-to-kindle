using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace BookToKindle.Infrastructure
{
	/// <summary>
	/// A class to delete containing files upon disposal
	/// </summary>
	public sealed class TempFiles : IDisposable
	{
		private readonly IList<string> files = new List<string>();

		public void Add(string filePath)
		{
			this.files.Add(filePath);
		}

		public void Dispose()
		{
			foreach (string file in this.files.Where(File.Exists))
			{
				try
				{
					File.Delete(file);
				}
				catch (Exception exception)
				{
					Log.Error(exception, $"Can't delete a file {file}");
				}
			}
		}
	}
}
using System;
using System.Linq;

namespace BoundedLayers
{
	public class Reader
	{
		public Reader(string solutionPath)
		{
			var sol = new Models.Solution(solutionPath);
			foreach (var project in sol.Projects)
			{
				Console.WriteLine("{0} - {1}", project.Name, project.Id);
				foreach (var r in project.References)
				{
					Console.WriteLine(" - {0}", r);
				}
				Console.WriteLine();
			}
		}
	}
}


using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BoundedLayers.Models
{
	public class Solution
	{
		private static readonly Regex _projectRegex = new Regex(@"Project\(.*=([^,]*),([^,]*\.csproj"" *),(.*)");
		private readonly List<Project> _projects;
		private readonly IDictionary<string, Project> _projectMap;

		public Solution(string path) : this(LoadProjects(path)) {}

		public Solution(IEnumerable<Project> projects)
		{
			_projects = projects.ToList();
			_projectMap = _projects.ToDictionary(p => p.Id);
		}

		public IEnumerable<Project> Projects { get { return _projects; } }

		public Project Find(string id)
		{
			return _projectMap[id];
		}

		private static List<Project> LoadProjects(string path)
		{
			var pathPrefix = Path.GetDirectoryName(path);

			return File.ReadAllLines(path)
				.Select(l => _projectRegex.Match(l))
				.Where(m => m.Success)
				.Select(m => new Project(
						m.Groups[3].Value.Trim().Trim('"'),
						m.Groups[1].Value.Trim().Trim('"'),
						Path.GetFullPath(Path.Combine(pathPrefix, m.Groups[2].Value.Trim().Trim('"')))))
				.ToList();
		}
	}
}


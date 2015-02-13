// The MIT License (MIT)
// 
// Copyright (c) 2015 Patricio Zavolinsky
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
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


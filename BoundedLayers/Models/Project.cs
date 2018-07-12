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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BoundedLayers.Models
{
	/// <summary>
	/// Project model.
	/// </summary>
	public class Project
	{
        private readonly List<string> _references;

		/// <summary>
		/// Loads a project from the specified path.
		/// </summary>
		/// <param name="id">Project id, as defined in the solution file.</param>
		/// <param name="name">Project name.</param>
		/// <param name="path">Project absolute path.</param>
		public Project(string id, string name, string path) : this(id, name, LoadReferences(path)) {}

		/// <summary>
		/// Loads a project from the specified references.
		/// </summary>
		/// <param name="id">Project id, as defined in the solution file.</param>
		/// <param name="name">Project name.</param>
		/// <param name="references">List of referenced project ids.</param>
		public Project(string id, string name, IEnumerable<string> references)
		{
			Id = id;
			Name = name;
			_references = references.ToList();
		}

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the referenced project ids.
        /// </summary>
        /// <value>The referenced project ids.</value>
        public IEnumerable<string> References => _references;

	    private static List<string> LoadReferences(string path)
		{
			var root = XDocument.Load(path).Root;
			var ns = root?.Name.Namespace;
			return root?
				.Descendants(ns + "ProjectReference")
				.Select(pr => pr.Attribute("Include")?.Value)
			    .Select(pn => pn?.Replace("..\\", string.Empty))
				.ToList();
		}
	}
}


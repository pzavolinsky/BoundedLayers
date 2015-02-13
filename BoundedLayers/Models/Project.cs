using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BoundedLayers.Models
{
	public class Project
	{
		private readonly string _id;
		private readonly string _name;
		private readonly List<string> _references;

		public Project(string id, string name, string path) : this(id, name, LoadReferences(path)) {}

		public Project(string id, string name, IEnumerable<string> references)
		{
			_id = id;
			_name = name;
			_references = references.ToList();
		}


		public string Id { get { return _id; } }
		public string Name { get { return _name; } }
		public IEnumerable<string> References { get { return _references; } }

		private static List<string> LoadReferences(string path)
		{
			var root = XDocument.Load(path).Root;
			var ns = root.Name.Namespace;
			return root
				.Descendants(ns + "ProjectReference")
				.Select(pr => pr.Element(ns + "Project").Value)
				.ToList();
		}
	}
}


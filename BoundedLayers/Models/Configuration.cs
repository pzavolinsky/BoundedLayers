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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace BoundedLayers.Models
{
	/// <summary>
	/// Layer configuration interface.
	/// </summary>
	public interface IConfiguration
	{
		/// <summary>
		/// Define a new layer.
		/// </summary>
		/// <param name="name">Layer name.</param>
		IRule Layer(string name);

		/// <summary>
		/// Define a new component.
		/// </summary>
		/// <param name="name">Component name.</param>
		IRule Component(string name);

		/// <summary>
		/// Validate the solution file in the specified path.
		/// </summary>
		/// <param name="solutionPath">Solution path.</param>
		/// <returns>A list of validation exceptions.</returns>
		IEnumerable<ProjectException> Validate(string solutionPath);

		/// <summary>
		/// Validate the specified solution.
		/// </summary>
		/// <param name="solution">Solution.</param>
		/// <returns>A list of validation exceptions.</returns>
		IEnumerable<ProjectException> Validate(Solution solution);
	}

	/// <summary>
	/// Configuration implementation.
	/// </summary>
	public class Configuration : IConfiguration
	{
		private readonly List<Rule> _layerRules = new List<Rule>();
		private readonly List<Rule> _componentRules = new List<Rule>();
		private readonly Expression.Type _expType;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.Configuration"/> class.
		/// </summary>
		/// <param name="expType">Expression type.</param>
		public Configuration(Expression.Type expType)
		{
			_expType = expType;
		}

		/// <see cref="BoundedLayers.Models.IConfiguration.Layer"/>
		public IRule Layer(string name)
		{
			var rule = new Rule(this, name);
			_layerRules.Add(rule);
			return rule;
		}

		/// <see cref="BoundedLayers.Models.IConfiguration.Component"/>
		public IRule Component(string name)
		{
			var rule = new Rule(this, name);
			_componentRules.Add(rule);
			return rule;
		}

		/// <summary>
		/// Creates a new expression using the pattern specified.
		/// </summary>
		/// <returns>The expression.</returns>
		/// <param name="pattern">Pattern.</param>
		public IExpression CreateExpression(string pattern)
		{
			return Expression.Create(_expType, pattern);
		}

		/// <see cref="BoundedLayers.Models.IConfiguration.Validate(string)"/>
		public IEnumerable<ProjectException> Validate(string solutionPath)
		{
			if (!Path.IsPathRooted(solutionPath)) 
			{
				var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
				var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
				var dirPath = Path.GetDirectoryName(codeBasePath);
				solutionPath = Path.Combine(dirPath, solutionPath);
			}

			return Validate(new Solution(solutionPath));
		}

		/// <see cref="BoundedLayers.Models.IConfiguration.Validate(Solution)"/>
		public IEnumerable<ProjectException> Validate(Solution solution)
		{
			var res = new List<ProjectException>();

			foreach (var project in solution.Projects)
			{
				// verify layer rules
				var layers = _layerRules.Where(r => r.Matches(project.Name)).ToArray();
				var components = _componentRules.Where(r => r.Matches(project.Name)).ToArray();

				if (!layers.Any()) 
				{
					res.Add(new UnknownLayerException(project));
				}
				if (!components.Any()) 
				{
					res.Add(new UnknownComponentException(project));
				}

				foreach (var referenced in project.References.Select(id => solution.Find(id)))
				{
					if (layers.Any() && !layers.Any(r => r.Allows(referenced.Name)))
					{
						res.Add(new LayerViolationException(project, referenced));
					}
					if (components.Any() && !components.Any(r => r.Allows(referenced.Name)))
					{
						res.Add(new ComponentViolationException(project, referenced));
					}
				}
			}

			return res;
		}
	}
}


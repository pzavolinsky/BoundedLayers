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

namespace BoundedLayers.Models
{
	/// <summary>
	/// Project rule interface.
	/// </summary>
	public interface IRule
	{
		/// <summary>
		/// Specify the project references.
		/// </summary>
		/// <param name="names">Names.</param>
		IConfiguration References(params string[] names);

		/// <summary>
		/// Specify that the project can reference any other project.
		/// </summary>
		IConfiguration ReferencesAnything();

		/// <summary>
		/// Specify that the project has no references.
		/// </summary>
		IConfiguration HasNoReferences();
	}

	/// <summary>
	/// Rule implementation.
	/// </summary>
	public class Rule : IRule
	{
		private readonly Configuration _configuration;
		private readonly IExpression _nameEx;
		private readonly List<IExpression> _referenced;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.Rule"/> class.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		/// <param name="name">Name.</param>
		public Rule(Configuration configuration, string name)
		{
			_configuration = configuration;
			_nameEx = configuration.CreateExpression(name);
			_referenced = new List<IExpression>();
		}

		/// <see cref="BoundedLayers.Models.IRule.References"/>
		public IConfiguration References(params string[] names)
		{
			_referenced.AddRange(names.Select(n => _configuration.CreateExpression(n)));
			return _configuration;
		}

		/// <see cref="BoundedLayers.Models.IRule.ReferencesAnything"/>
		public IConfiguration ReferencesAnything()
		{
			_referenced.Add(new TrueExpression());
			return _configuration;
		}

		/// <see cref="BoundedLayers.Models.IRule.HasNoReferences"/>
		public IConfiguration HasNoReferences()
		{
			return _configuration;
		}

		/// <summary>
		/// Returns true if the rule matches the specified project name.
		/// </summary>
		/// <param name="name">Project name.</param>
		/// <returns>True if the rule matches the project name, false otherwise.</returns>
		public bool Matches(string name)
		{
			return _nameEx.Matches(name);
		}

		/// <summary>
		/// Returns true if the rule allows a reference to the specified project name.
		/// </summary>
		/// <param name="name">Project name.</param>
		/// <returns>True if the rule allows a reference to the specified the project name, false otherwise.</returns>
		public bool Allows(string name)
		{
			return Matches(name) || _referenced.Any(e => e.Matches(name));
		}
	}

}


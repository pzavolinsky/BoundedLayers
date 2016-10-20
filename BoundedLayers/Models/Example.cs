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
	/// Rule example interface.
	/// </summary>
	public interface IExample
	{
		/// <summary>
		/// Specify valid references for this example project.
		/// </summary>
		/// <param name="names">Names.</param>
		IConfiguration CanReference(params string[] names);

		/// <summary>
		/// Specify invalid references for this example project.
		/// </summary>
		/// <param name="names">Names.</param>
		IConfiguration CannotReference(params string[] names);
	}

	/// <summary>
	/// Rule implementation.
	/// </summary>
	public class Example : IExample
	{
		private readonly Configuration _configuration;
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.Example"/> class.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		/// <param name="name">Name.</param>
		public Example(Configuration configuration, string name)
		{
			_configuration = configuration;
			_name = name;
		}

		/// <see cref="BoundedLayers.Models.IExample.CanReference"/>
		public IConfiguration CanReference(params string[] names)
		{
			_configuration.Validate(_name, names).AssertThrowsFirst();
			return _configuration;
		}

		/// <see cref="BoundedLayers.Models.IExample.CannotReference"/>
		public IConfiguration CannotReference(params string[] names)
		{
			var res = _configuration.ValidateExtended(_name, names);
			res.Item1.Assert();
			names
				.Zip(res.Item2, (name, rules) => new Tuple<string, AllowingRules>(name, rules))
				.Where(t => t.Item2.Component != null && t.Item2.Layer != null)
				.Select(t => new NegativeExampleAssertionException(
					_name,
					t.Item1,
					t.Item2.Layer.ToString(),
					t.Item2.Component.ToString()))
				.AssertThrowsFirst();
			return _configuration;
		}
	}
}


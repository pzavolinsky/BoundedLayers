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
using System.IO;
using System.Linq;
using BoundedLayers.Models;
using System.Reflection;

namespace BoundedLayers
{
	/// <summary>
	/// Layers configuration builder.
	/// </summary>
	public static class Layers
	{
		/// <summary>
		/// Configure bounded layer rules.
		/// </summary>
		/// <param name="expType">Expression type.</param>
		public static IConfiguration Configure(Expression.Type expType = Expression.Type.NamePart)
		{
			return new Configuration(expType);
		}
	}

	/// <summary>
	/// Validation result extensions.
	/// </summary>
	public static class ValidationResultExtensions
	{
		/// <summary>
		/// Asserts that the validation was successful, and if the validation failed
		/// throws the first exception in the error list.
		/// </summary>
		/// <param name="res">Validation result.</param>
		public static void AssertThrowsFirst(this IEnumerable<ProjectException> res)
		{
			if (res.Any()) throw res.First();
		}

		/// <summary>
		/// Asserts that the validation was successful, and if the validation failed
		/// throws a System.Exception with the concatenated validation error messages.
		/// </summary>
		/// <param name="res">Validation result.</param>
		public static void Assert(this IEnumerable<ProjectException> res)
		{
			res.Assert(s => new Exception(s));
		}

		/// <summary>
		/// Asserts that the validation was successful, and if the validation failed
		/// throws a T (exception) with the concatenated validation error messages.
		/// </summary>
		/// <param name="res">Validation result.</param>
		/// <param name="ctor">A constructor delegate that takes the concatenated
		/// validation error messages and returns the new exception of type T.</param>
		/// <typeparam name="T">The exception type.</typeparam>
		public static void Assert<T>(this IEnumerable<ProjectException> res, Func<string, T> ctor) where T : Exception
		{
			if (!res.Any()) return;
			throw ctor(string.Join("\n", res.Select(e => e.Message)));
		}
	}
}

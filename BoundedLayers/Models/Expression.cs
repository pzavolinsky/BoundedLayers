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
using System.Linq;
using System.Text.RegularExpressions;

namespace BoundedLayers.Models
{
	/// <summary>
	/// Expression interface.
	/// </summary>
	public interface IExpression
	{
		/// <summary>
		/// Returns true if the expression matches the specified project name.
		/// </summary>
		/// <param name="name">Project name.</param>
		/// <returns>True if the expression matches the project name, false otherwise.</returns>
		bool Matches(string name);
	}

	/// <summary>
	/// Expression factory.
	/// </summary>
	public static class Expression
	{
		/// Expression type
		public enum Type
		{
			/// <see cref="BoundedLayers.Models.NamePartExpression"/>
			NamePart,
			/// <see cref="BoundedLayers.Models.RegexExpression"/>
			RegularExpression
		};

		/// <summary>
		/// Create an expression of the specified type, using the specified pattern.
		/// </summary>
		/// <param name="type">The expression type.</param>
		/// <param name="pattern">The expression pattern.</param>
		public static IExpression Create(Expression.Type type, string pattern)
		{
			if (type == Type.RegularExpression)
			{
				return new RegexExpression(pattern);
			}
			if (pattern.StartsWith("r:"))
			{
				return new RegexExpression(pattern.Substring(2));
			}
			return new NamePartExpression(pattern);
		}
	}

	/// <summary>
	/// This expression pattern matches any part of an project name.
	/// Project name parts are the strings delimited by dots. For example,
	/// System.Web.Http contains 3 parts: "System", "Web" and "Http".
	/// </summary>
	public class NamePartExpression : IExpression
	{
		private readonly string _part;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.NamePartExpression"/> class.
		/// </summary>
		/// <param name="pattern">Project name part (should not contain dots).</param>
		public NamePartExpression(string pattern)
		{
			_part = pattern;
		}

		/// <see cref="BoundedLayers.Models.IExpression.Matches"/>
		public bool Matches(string name)
		{
			return name.Split('.').Contains(_part);
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.NamePartExpression"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.NamePartExpression"/>.</returns>
        public override string ToString() 
        {
            return _part;
        }
	}

	/// <summary>
	/// This expression pattern is an anchored regex that is matched
	/// against the project name.
	/// </summary>
	public class RegexExpression : IExpression
	{
		private readonly Regex _regex;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.RegexExpression"/> class.
		/// </summary>
		/// <param name="pattern">Anchored regex (should not start with ^, nor end with $).</param>
		public RegexExpression(string pattern)
		{
			_regex = new Regex(string.Format("^{0}$", pattern));
		}

		/// <see cref="BoundedLayers.Models.IExpression.Matches"/>
		public bool Matches(string name)
		{
			return _regex.IsMatch(name);
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.RegexExpression"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.RegexExpression"/>.</returns>
        public override string ToString() 
        {
            return _regex.ToString();
        }
    }

	/// <summary>
	/// This expression always matches.
	/// </summary>
	public class TrueExpression : IExpression
	{
		/// <see cref="BoundedLayers.Models.IExpression.Matches"/>
		public bool Matches(string name)
		{
			return true;
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.TrueExpression"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="BoundedLayers.Models.TrueExpression"/>.</returns>
        public override string ToString()
        {
            return "ReferencesAnything";
        }
	}

}


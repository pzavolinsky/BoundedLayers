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
using System.Linq;
using System.Text.RegularExpressions;

namespace BoundedLayers
{
	public interface IExpression
	{
		bool Matches(string s);
	}

	public static class Expression
	{
		public enum Type
		{
			AssemblyPart,
			RegularExpression
		};

		public static IExpression Create(Expression.Type expType, string s)
		{
			if (expType == Type.RegularExpression)
				return new RegexExpression(s);
			return new AssemblyPartExpression(s);
		}
	}

	public class AssemblyPartExpression : IExpression
	{
		public enum Type
		{
			AssemblyPart,
			RegularExpression
		};

		private readonly string _part;

		public AssemblyPartExpression(string s)
		{
			_part = s;
		}

		public bool Matches(string s)
		{
			return s.Split('.').Contains(_part);
		}
	}

	public class RegexExpression : IExpression
	{
		private readonly Regex _regex;

		public RegexExpression(string s)
		{
			_regex = new Regex(string.Format("^{0}$", s));
		}

		public bool Matches(string s)
		{
			return _regex.IsMatch(s);
		}
	}
}


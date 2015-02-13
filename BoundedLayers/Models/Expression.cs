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

		public override string ToString()
		{
			return _part;
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

		public override string ToString()
		{
			return _regex.ToString();
		}
	}
}


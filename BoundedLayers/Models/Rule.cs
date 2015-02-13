using System;

namespace BoundedLayers.Models
{
	public class Rule
	{
		private readonly Expression _refererEx;
		private readonly Expression _referedEx;

		public Rule(string referer, string refered)
		{
			_refererEx = new Expression(referer);
			_referedEx = new Expression(refered);
		}

		public bool Matches(Project referer, Project refered)
		{
			return _refererEx.Matches(referer.Name) && _referedEx.Matches(refered.Name);
		}
	}
}


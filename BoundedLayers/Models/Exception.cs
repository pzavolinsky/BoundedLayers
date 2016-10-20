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

namespace BoundedLayers.Models
{
	/// <summary>
	/// Base class for project exceptions.
	/// </summary>
	public class ProjectException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.ProjectException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		/// <param name="message">The exception message.</param>
		public ProjectException(string project, string message) : base(message)
		{
			Project = project;
		}

		/// <summary>
		/// Gets the misconfigured project.
		/// </summary>
		/// <value>The project.</value>
		public string Project { get; private set; }
	}

	/// <summary>
	/// Unknown layer exception, thrown when a project does not match any of
	/// the defined layers.
	/// </summary>
	public class UnknownLayerException : ProjectException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.UnknownLayerException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		public UnknownLayerException(string project)
			: base(project, string.Format("Unknown layer: {0}", project)) {}
	}

	/// <summary>
	/// Unknown component exception, thrown when a project does not match any
	/// of the defined components.
	/// </summary>
	public class UnknownComponentException : ProjectException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.UnknownComponentException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		public UnknownComponentException(string project)
			: base(project, string.Format("Unknown component: {0}", project)) {}
	}

	/// <summary>
	/// Base class for project reference exceptions.
	/// </summary>
	public class ReferenceException : ProjectException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.ReferenceException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		/// <param name="referenced">The offending project reference.</param>
		/// <param name="message">The exception message.</param>
		public ReferenceException(string project, string referenced, string message) : base(project, message)
		{
			Referenced = referenced;
		}

		/// <summary>
		/// Gets the referenced project.
		/// </summary>
		/// <value>The referenced project.</value>
		public string Referenced { get; private set; }
	}

	/// <summary>
	/// Layer violation exception, thrown when a project reference that
	/// crosses layer boundaries is not defined in the list of allowed
	/// layer references.
	/// </summary>
	public class LayerViolationException : ReferenceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.LayerViolationException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		/// <param name="referenced">The offending project reference.</param>
		public LayerViolationException(string project, string referenced)
			: base(project, referenced, string.Format("Layer violation: {0} cannot refer to {1}", project, referenced)) {}
	}

	/// <summary>
	/// Component violation exception, thrown when a project reference that
	/// crosses component boundaries is not defined in the list of allowed
	/// component references.
	/// </summary>
	public class ComponentViolationException : ReferenceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.ComponentViolationException"/> class.
		/// </summary>
		/// <param name="project">The misconfigured project.</param>
		/// <param name="referenced">The offending project reference.</param>
		public ComponentViolationException(string project, string referenced)
			: base(project, referenced, string.Format("Component violation: {0} cannot refer to {1}", project, referenced)) {}
	}

	/// <summary>
	/// Negative example assertion exception, thrown when an invalid
	/// project reference is accepted by the current rules.
	/// </summary>
	public class NegativeExampleAssertionException : ReferenceException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundedLayers.Models.NegativeExampleAssertionException"/> class.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <param name="referenced">The project reference.</param>
		/// <param name="layerRule">The layer rule that allows this invalid reference.</param>
		/// <param name="componentRule">The component rule that allows this invalid reference.</param>
		public NegativeExampleAssertionException(string project, string referenced, string layerRule, string componentRule)
			: base(project, referenced, string.Format(
				"Example assertion failed: {0} should NOT be able to refer to {1} but the following rules allow the reference: layer={2}, component={3}",
				project, referenced, layerRule, componentRule))
		{
			LayerRule = layerRule;
			ComponentRule = componentRule;
		}

		/// <summary>
		/// Gets the layer rule.
		/// </summary>
		/// <value>The layer rule.</value>
		public string LayerRule { get; }

		/// <summary>
		/// Gets the component rule.
		/// </summary>
		/// <value>The component rule.</value>
		public string ComponentRule { get;}
	}
}

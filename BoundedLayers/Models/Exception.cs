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
	public class ProjectException : Exception
	{
		public ProjectException(Project project, string message) : base(message)
		{
			Project = project;
		}
		public Project Project { get; private set; }
	}

	public class UnknownLayerException : ProjectException
	{
		public UnknownLayerException(Project project)
			: base(project, string.Format("Unknown layer: {0}", project.Name)) {}
	}

	public class UnknownComponentException : ProjectException
	{
		public UnknownComponentException(Project project)
			: base(project, string.Format("Unknown component: {0}", project.Name)) {}
	}

	public class ReferenceException : ProjectException
	{
		public ReferenceException(Project project, Project referenced, string message) : base(project, message)
		{
			Referenced = referenced;
		}
		public Project Referenced { get; private set; }
	}

	public class LayerViolationException : ReferenceException
	{
		public LayerViolationException(Project project, Project referenced)
			: base(project, referenced, string.Format("Layer violation: {0} cannot refer to {1}", project.Name, referenced.Name)) {}
	}

	public class ComponentViolationException : ReferenceException
	{
		public ComponentViolationException(Project project, Project referenced)
			: base(project, referenced, string.Format("Component violation: {0} cannot refer to {1}", project.Name, referenced.Name)) {}
	}
}

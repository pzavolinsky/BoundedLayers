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

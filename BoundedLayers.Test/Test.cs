using NUnit.Framework;
using System;
using BoundedLayers.Models;

namespace BoundedLayers.Test
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void HappyPath()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core | <---- | Shared.Host |
			// +-------------+       +-------------+
			//        ^   ^                 ^
			//        |   +-------------+   |
			//        |                 |   |
			// +-------------+       +-------------+
			// |  App.Core   | <---- |  App.Host   |
			// +-------------+       +-------------+
			//
			Assert.True(false);

			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);
			var appCore    = CreateProject("App.Core"   , sharedCore.Id);
			var appHost    = CreateProject("App.Host"   , sharedCore.Id, sharedHost.Id, appCore.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			Layers.Configure()
				.Layer("Shared").HasNoReferences()
				.Layer("App").References("Shared")
				.Component("Core").HasNoReferences()
				.Component("Host").References("Core")
				.Validate(solution)
				.Assert();
		}

		protected Project CreateProject(string name, params string[] referenced)
		{
			return new Project(name, name, referenced);
		}
	}
}


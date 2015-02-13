using NUnit.Framework;
using System;
using BoundedLayers.Models;
using System.Reflection;
using System.IO;

namespace BoundedLayers.Test
{
	[TestFixture]
	public class Test
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
		public void AssertDefaultLayout(Solution solution)
		{
			Layers.Configure()
				.Layer("Shared").HasNoReferences()
				.Layer("App").References("Shared")
				.Component("Core").HasNoReferences()
				.Component("Host").References("Core")
				.Validate(solution)
				.AssertThrowsFirst();
		}

		[Test]
		public void NoReferences()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core |       | Shared.Host |
			// +-------------+       +-------------+
			//
			//
			//
			// +-------------+       +-------------+
			// |  App.Core   |       |  App.Host   |
			// +-------------+       +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host");
			var appCore    = CreateProject("App.Core"   );
			var appHost    = CreateProject("App.Host"   );

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			AssertDefaultLayout(solution);
		}

		[Test]
		public void IntraLayerReferences()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core | <---- | Shared.Host |
			// +-------------+       +-------------+
			//
			//
			//
			// +-------------+       +-------------+
			// |  App.Core   | <---- |  App.Host   |
			// +-------------+       +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);
			var appCore    = CreateProject("App.Core"   );
			var appHost    = CreateProject("App.Host"   , appCore.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			AssertDefaultLayout(solution);
		}

		[Test]
		public void InterLayerReferences()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core |       | Shared.Host |
			// +-------------+       +-------------+
			//        ^                     ^
			//        |                     |
			//        |                     |
			// +-------------+       +-------------+
			// |  App.Core   |       |  App.Host   |
			// +-------------+       +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host");
			var appCore    = CreateProject("App.Core"   , sharedCore.Id);
			var appHost    = CreateProject("App.Host"   , sharedHost.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			AssertDefaultLayout(solution);
		}

		[Test]
		public void AllTogether()
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
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);
			var appCore    = CreateProject("App.Core"   , sharedCore.Id);
			var appHost    = CreateProject("App.Host"   , sharedCore.Id, sharedHost.Id, appCore.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			AssertDefaultLayout(solution);
		}

		[Test]
		public void UnknownLayer()
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
			//        ^
			//        x
			//        |
			// +-------------+
			// |  Unk.Core   |
			// +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);
			var appCore    = CreateProject("App.Core"   , sharedCore.Id);
			var appHost    = CreateProject("App.Host"   , sharedCore.Id, sharedHost.Id, appCore.Id);
			var unkCore    = CreateProject("Unk.Core"   , appCore.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost,
				unkCore
			});

			var e = Assert.Throws<UnknownLayerException>(() => AssertDefaultLayout(solution));
			Assert.AreEqual(unkCore, e.Project);
		}

		[Test]
		public void UnknownComponent()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core | <---- | Shared.Host |
			// +-------------+       +-------------+
			//        ^   ^                 ^
			//        |   +-------------+   |
			//        |                 |   |
			// +-------------+       +-------------+       +-------------+
			// |  App.Core   | <---- |  App.Host   | <-x-- |  App.Test   |
			// +-------------+       +-------------+       +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);
			var appCore = CreateProject("App.Core", sharedCore.Id);
			var appHost = CreateProject("App.Host", sharedCore.Id, sharedHost.Id, appCore.Id);
			var appTest = CreateProject("App.Test", appHost.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost, appTest
			});

			var e = Assert.Throws<UnknownComponentException>(() => AssertDefaultLayout(solution));
			Assert.AreEqual(appTest, e.Project);
		}

		[Test]
		public void LayerViolation()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core | <---- | Shared.Host |
			// +-------------+       +-------------+
			//        ^                     |
			//        |                     x
			//        |                     v
			// +-------------+       +-------------+
			// |  App.Core   | <---- |  App.Host   |
			// +-------------+       +-------------+
			//
			var sharedCore = CreateProject("Shared.Core");
			var appCore = CreateProject("App.Core", sharedCore.Id);
			var appHost = CreateProject("App.Host", appCore.Id);
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id, appHost.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			var e = Assert.Throws<LayerViolationException>(() => AssertDefaultLayout(solution));
			Assert.AreEqual(sharedHost, e.Project);
			Assert.AreEqual(appHost, e.Referenced);
		}

		[Test]
		public void ComponentViolation()
		{
			//
			// +-------------+       +-------------+
			// | Shared.Core | --x-> | Shared.Host |
			// +-------------+       +-------------+
			//        ^                     ^
			//        |                     |
			//        |                     |
			// +-------------+       +-------------+
			// |  App.Core   | <---- |  App.Host   |
			// +-------------+       +-------------+
			//
			var sharedHost = CreateProject("Shared.Host");
			var sharedCore = CreateProject("Shared.Core", sharedHost.Id);
			var appCore = CreateProject("App.Core", sharedCore.Id);
			var appHost = CreateProject("App.Host", appCore.Id, sharedHost.Id);

			var solution = new Solution(new Project[] {
				sharedCore, sharedHost,
				appCore, appHost
			});

			var e = Assert.Throws<ComponentViolationException>(() => AssertDefaultLayout(solution));
			Assert.AreEqual(sharedCore, e.Project);
			Assert.AreEqual(sharedHost, e.Referenced);
		}

		[Test]
		public void RegexExpressions()
		{
			var sharedCore = CreateProject("Shared");
			var sharedHost = CreateProject("Shared.Host", sharedCore.Id);

			Layers.Configure(Expression.Type.RegularExpression)
				.Layer(@"Shared.*").HasNoReferences()
				.Component(@"Shared").HasNoReferences()
				.Component(@".*\.Host").References(@"Shared")
				.Validate(new Solution(new Project[] { sharedCore, sharedHost }))
				.AssertThrowsFirst();
		}

		[Test]
		public void AssertionsPassIfNoErrors()
		{
			var noErrors = new ProjectException[0];
			noErrors.Assert();
			noErrors.Assert(s => new Exception(s));
			noErrors.AssertThrowsFirst();
		}

		[Test]
		public void AssertionsFailIfErrors()
		{
			var first  = new UnknownComponentException(CreateProject("First"));
			var second = new UnknownLayerException(CreateProject("Second"));
			var errors = new ProjectException[] { first, second };

			Assert.Throws<Exception>(() => errors.Assert());
			Assert.Throws<AssertionException>(() => errors.Assert(s => new AssertionException(s)));
			var e = Assert.Throws<UnknownComponentException>(() => errors.AssertThrowsFirst());
			Assert.AreEqual(first, e);
		}

		[Test]
		public void ValidateSolution()
		{
			Layers.Configure(Expression.Type.RegularExpression)
				.Layer(@"BoundedLayers.*").HasNoReferences()
				.Component(@"BoundedLayers").HasNoReferences()
				.Component(@".*\.Test").References(@"BoundedLayers")
				.Validate(@"../../../BoundedLayers.sln")
				.Assert();
		}

		[Test]
		public void ValidateInvalidSolution()
		{
			Assert.Throws<FileNotFoundException>(() => Layers.Configure().Validate("invalid.sln").Assert());
		}


		protected Project CreateProject(string name, params string[] referenced)
		{
			return new Project(name, name, referenced);
		}
	}
}


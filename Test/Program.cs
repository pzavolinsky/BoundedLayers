using System;
using BoundedLayers;
using BoundedLayers.Models;

namespace Test
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//var reader = new Reader("/home/pzavolinsky/src/lab/Autocore/Autocore.sln");

			var sharedCore = new Project("sharedCore", "Shared.Core", new string[] {});
			var sharedInf  = new Project("sharedInf" , "Shared.Inf" , new string[] { sharedCore.Id });
			var sharedHost = new Project("sharedHost", "Shared.Host", new string[] { sharedCore.Id, sharedInf.Id});
			var appCore    = new Project("appCore"   , "App.Core"   , new string[] { sharedCore.Id, sharedInf.Id });
			var appInf     = new Project("appInf"    , "App.Inf"    , new string[] { sharedCore.Id, sharedInf.Id, appCore.Id});
			var appHost    = new Project("appHost"   , "App.Host"   , new string[] { sharedCore.Id, sharedInf.Id, sharedHost.Id, appCore.Id, appInf.Id});

			var solution = new Solution(new Project[] {
				sharedCore, sharedInf, sharedHost,
				appCore, appInf, appHost
			});

			/*
			var res = Layers.Configure(Expression.Type.RegularExpression)
				.Layer("Shared\\..*").HasNoReferences()
				.Layer("App\\..*").References("Shared\\..*")
				.Component(".*\\.Core").HasNoReferences()
				.Component(".*\\.Inf").References(".*\\.Core")
				.Component(".*\\.Host").References(".*\\.Inf", ".*\\.Core")
				.Validate(solution);
			*/
			var res = Layers.Configure()
				.Layer("Shared").HasNoReferences()
				.Layer("App").References("Shared")
				.Component("Core").HasNoReferences()
				.Component("Inf").References("Core")
				.Component("Host").References("Inf", "Core")
				.Validate(solution);

			foreach (var s in res)
			{
				Console.WriteLine(s);
			}

			res.Assert();
		}
	}
}

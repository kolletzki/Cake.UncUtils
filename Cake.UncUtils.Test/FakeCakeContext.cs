using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Testing;

namespace Cake.UncUtils.Test
{
	public class FakeCakeContext
	{
		private readonly FakeLog _log;

		public FakeCakeContext ()
		{
			WorkingDirectory = new DirectoryPath (
				System.IO.Path.GetFullPath(
					System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "../../")));

			var environment = FakeEnvironment.CreateUnixEnvironment (false);

			var fileSystem = new FakeFileSystem (environment);
			var globber = new Globber (fileSystem, environment);
			_log = new FakeLog ();
			var args = new FakeCakeArguments ();
			var processRunner = new ProcessRunner (environment, _log);
			var registry = new WindowsRegistry ();

			CakeContext = new CakeContext (
				fileSystem,
				environment,
				globber,
				_log,
				args,
				processRunner,
				registry,
				new ToolLocator(
					environment,
					new ToolRepository(environment),
					new ToolResolutionStrategy(
						fileSystem,
						environment,
						globber,
						new FakeConfiguration()
					)
				)
			);

			CakeContext.Environment.WorkingDirectory = WorkingDirectory;
		}

		public DirectoryPath WorkingDirectory { get; }

		public ICakeContext CakeContext { get; }

		public string GetLogs ()
		{
			return string.Join(Environment.NewLine, _log.Entries);
		}

		public void DumpLogs ()
		{
			foreach (var m in _log.Entries)
				Console.WriteLine (m);
		}
	}
}
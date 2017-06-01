using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Cake.UncUtils.Test
{
	[TestFixture]
	public class Tests
	{
		private FakeCakeContext _context;

		private dynamic _config;

		[SetUp]
		public void Setup()
		{
			_context = new FakeCakeContext();

			_config = JObject.Parse(File.ReadAllText(Path.Combine(_context.WorkingDirectory.ToString(), "./testData.json")));
		}

		[TearDown]
		public void Teardown()
		{
			_context.DumpLogs();
		}

		[Test, Order(1)]
		public void TestMountUncDir()
		{
			var source = (string) _config.source;
			var target = (string) _config.target;
			
			_context.CakeContext.MountUncDir(source, target);

			DirectoryAssert.Exists(target);
		}

		[Test, Order(2)]
		public void TestUnmountUncDir()
		{
			var dir = (string) _config.target;

			_context.CakeContext.UnmountUncDir(dir);

			DirectoryAssert.DoesNotExist(dir);
		}

		[Test, Order(3)]
		public void TestMountUncDirInvalid()
		{
			var invalidSource = (string) _config.invalidSource;
			var target = (string) _config.target;

			if (Directory.Exists(target))
			{
				Directory.Delete(target);
			}

			Assert.Throws<ArgumentException>(() => _context.CakeContext.MountUncDir(invalidSource, target));
		}
	}
}
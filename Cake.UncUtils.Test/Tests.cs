using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cake.UncUtils.Test
{
    [TestCaseOrderer("Cake.UncUtils.Test.PriorityOrderer", "Cake.UncUtils.Test")]
    public class Tests : IDisposable
    {
        private readonly FakeCakeContext _context;
        private readonly dynamic _config;

        public Tests()
        {
            _context = new FakeCakeContext();
            _config = JObject.Parse(File.ReadAllText(Path.Combine(_context.WorkingDirectory.ToString(), "../testData.json")));
        }

        public void Dispose()
        {
            _context.DumpLogs();
        }

        [Fact]
        [TestPriority(1)]
        public void TestMountUncDir()
        {
            var source = (string)_config.source;
            var target = (string)_config.target;

            _context.CakeContext.MountUncDir(source, target);

            Assert.True(Directory.Exists(target));
        }

        [Fact]
        [TestPriority(2)]
        public void TestUnmountUncDir()
        {
            var dir = (string)_config.target;

            _context.CakeContext.UnmountUncDir(dir);

            Assert.False(Directory.Exists(dir));
        }

        [Fact]
        [TestPriority(3)]
        public void TestMountUncDirInvalid()

        {
            var invalidSource = (string)_config.invalidSource;
            var target = (string)_config.target;

            if (Directory.Exists(target))
            {
                Directory.Delete(target);
            }

            Assert.Throws<ArgumentException>(() => _context.CakeContext.MountUncDir(invalidSource, target));
        }
    }
}

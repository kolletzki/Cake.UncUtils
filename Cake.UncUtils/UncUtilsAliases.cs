using System;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.UncUtils
{
	/// <summary>
	/// Provides some aliases for dealing with UNC paths
	/// </summary>
	public static class UncUtilsAliases
	{
		/// <summary>
		/// Mounts a UNC path as directory
		/// </summary>
		/// <param name="context">Cake context</param>
		/// <param name="uncSource">UNC path</param>
		/// <param name="localTarget">Local directory to mount to</param>
		/// <exception cref="ArgumentNullException">uncSource or localTarget are null or empty</exception>
		/// <exception cref="ArgumentException">paths do (not) exist</exception>
		/// <exception cref="InvalidOperationException">Timeout while mounting the UNC directory</exception>
		[CakeMethodAlias]
		public static void MountUncDir(this ICakeContext context, string uncSource, string localTarget)
		{
			//Check arguments
			if (uncSource.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(uncSource));
			}

			if (localTarget.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(localTarget));
			}

			var tCheck = new Task<bool>(() =>
			{
				var di = new DirectoryInfo(uncSource);
				return di.Exists;
			});
			tCheck.Start();

			if (!tCheck.Wait(500) || !tCheck.Result)
			{
				throw new ArgumentException("UNC path does not exist", nameof(uncSource));
			}

			//Check if target path does not exists
			if (Directory.Exists(localTarget))
			{
				throw new ArgumentException("Local target directory does already exist", nameof(localTarget));
			}

			//Create all parent directories of targen path
			var localParent = localTarget.Substring(0, localTarget.LastIndexOf("\\", StringComparison.Ordinal));

			Directory.CreateDirectory(localParent);

			//Run the mount script
			RunScript($"Start-Process cmd -Verb RunAs -argument \"/c mklink /d {localTarget} {uncSource}\"");

			var tResult = new Task<bool>(() =>
			{
				while (!Directory.Exists(localTarget))
				{
					Thread.Sleep(5);
				}

				return Directory.Exists(localTarget);
			});
			tResult.Start();

			if (!tResult.Wait(5000) || !tResult.Result)
			{
				throw new InvalidOperationException("Timeout while mounting the UNC directory");
			}
		}

		/// <summary>
		/// Unmounts a unc dir
		/// </summary>
		/// <param name="context">Cake context</param>
		/// <param name="localDir">Local dir where UNC dir is mounted</param>
		/// <exception cref="ArgumentNullException">localDir is null or empty</exception>
		/// <exception cref="ArgumentException">localDir path does not exist</exception>
		[CakeMethodAlias]
		public static void UnmountUncDir(this ICakeContext context, string localDir)
		{
			//Check arguments
			if (localDir.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(localDir));
			}

			if (!Directory.Exists(localDir))
			{
				throw new ArgumentException("Local dir does not exist", nameof(localDir));
			}

			//Delete mount dir
			Directory.Delete(localDir);
		}

		/// <summary>
		/// Local helper method to run a script with powershell
		/// </summary>
		/// <param name="script">Script contents</param>
		private static void RunScript(string script)
		{
			using (var runspace = RunspaceFactory.CreateRunspace())
			{
				runspace.Open();

				using (var pipeline = runspace.CreatePipeline())
				{
					pipeline.Commands.AddScript(script);
					pipeline.Invoke();
				}
			}
		}
	}
}
using Seed.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoFactories.Tests
{
    internal static class ProjectPaths
    {
        public static AbsolutePath RootDir { get; }
        public static AbsolutePath AutoFactoriesProjectDir { get; }
        public static AbsolutePath MicrosoftDir { get; }
        public static AbsolutePath NinjectDir { get; }

        static ProjectPaths()
        {
            RootDir = GetRotoDirectory();
            AutoFactoriesProjectDir = RootDir / "AutoFactories";
            MicrosoftDir = RootDir / "AutoFactories.Microsoft.DependencyInjection";
            NinjectDir = RootDir / "AutoFactories.Ninject";
        }

        public static AbsolutePath GetRotoDirectory(
            [CallerFilePath] string filePath = "")
        {
            DirectoryInfo? directoryInfo = new DirectoryInfo(filePath);

            while (directoryInfo is not null &&
                !Path.Exists(Path.Combine(directoryInfo.FullName, "AutoFactories.sln")))
            {
                directoryInfo = directoryInfo.Parent;
            }
            return directoryInfo is null 
                ? throw new Exception("Unable to find the source directory")
                : new AbsolutePath(directoryInfo.FullName);
        }
    }
}

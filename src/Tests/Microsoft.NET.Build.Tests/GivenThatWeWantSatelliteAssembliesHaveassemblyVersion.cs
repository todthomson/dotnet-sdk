// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.NET.TestFramework;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using FluentAssertions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.NET.Build.Tests
{
    public class GivenThatWeWantSatelliteAssembliesHaveAssemblyVersion : SdkTest
    {
        private string _mainAssemblyPath;
        private string _satelliteAssemblyPath;
        public GivenThatWeWantSatelliteAssembliesHaveAssemblyVersion(ITestOutputHelper log) : base(log)
        {
        }

        private void RestoreAndBuildTestAssets([CallerMemberName] string callingMethod = "")
        {
            TestAsset testAsset = _testAssetsManager
              .CopyTestAsset("AllResourcesInSatelliteDisableVersionGenerate", callingMethod)
              .WithSource();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //  Also target desktop on Windows to get more test coverage:
                //    * Desktop requires satellites to have same public key as parent whereas coreclr does not.
                //    * Reference path handling of satellite assembly generation used to be incorrect for desktop.
                testAsset = testAsset.WithTargetFrameworks($"{ToolsetInfo.CurrentTargetFramework};net46");
            }

            var buildCommand = new BuildCommand(testAsset);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            DirectoryInfo outputDirectory = buildCommand.GetOutputDirectory(ToolsetInfo.CurrentTargetFramework);
            _mainAssemblyPath = Path.Combine(outputDirectory.FullName, "AllResourcesInSatellite.dll");
            _satelliteAssemblyPath = Path.Combine(outputDirectory.FullName, "en", "AllResourcesInSatellite.resources.dll");
        }

        [Fact]
        public void It_should_produce_same_satelliteAssembly_FileVersionInfo_as_main()
        {
            RestoreAndBuildTestAssets();

            var mainAssemblyFileVersioninfo = FileVersionInfo.GetVersionInfo(_mainAssemblyPath);
            var satelliteAssemblyFileVersioninfo = FileVersionInfo.GetVersionInfo(_satelliteAssemblyPath);

            satelliteAssemblyFileVersioninfo.CompanyName.Should().Be(mainAssemblyFileVersioninfo.CompanyName);
            satelliteAssemblyFileVersioninfo.LegalCopyright.Should().Be(mainAssemblyFileVersioninfo.LegalCopyright);
            satelliteAssemblyFileVersioninfo.Comments.Should().Be(mainAssemblyFileVersioninfo.Comments);
            satelliteAssemblyFileVersioninfo.FileVersion.Should().Be(mainAssemblyFileVersioninfo.FileVersion);
            satelliteAssemblyFileVersioninfo.ProductVersion.Should().Be(mainAssemblyFileVersioninfo.ProductVersion);
            satelliteAssemblyFileVersioninfo.ProductName.Should().Be(mainAssemblyFileVersioninfo.ProductName);
            satelliteAssemblyFileVersioninfo.FileDescription.Should().Be(mainAssemblyFileVersioninfo.FileDescription);
        }

        [Fact]
        public void It_should_produce_same_satelliteAssembly_AssemblyVersions_as_main()
        {
            RestoreAndBuildTestAssets();

            var mainAssembly = AssemblyName.GetAssemblyName(_mainAssemblyPath);
            var satelliteAssembly = AssemblyName.GetAssemblyName(_satelliteAssemblyPath);

            satelliteAssembly.Version.Should().Be(mainAssembly.Version);
        }
    }
}

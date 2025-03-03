// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.NET.Sdk.WebAssembly
{
    public class AssetsComputingHelper
    {
        public static bool ShouldFilterCandidate(
            ITaskItem candidate,
            bool timezoneSupport,
            bool invariantGlobalization,
            bool copySymbols,
            string customIcuCandidateFilename,
            out string reason)
        {
            var extension = candidate.GetMetadata("Extension");
            var fileName = candidate.GetMetadata("FileName");
            var assetType = candidate.GetMetadata("AssetType");
            var fromMonoPackage = string.Equals(
                candidate.GetMetadata("NuGetPackageId"),
                "Microsoft.NETCore.App.Runtime.Mono.browser-wasm",
                StringComparison.Ordinal);

            reason = extension switch
            {
                ".a" when fromMonoPackage => "extension is .a is not supported.",
                ".c" when fromMonoPackage => "extension is .c is not supported.",
                ".h" when fromMonoPackage => "extension is .h is not supported.",
                // It is safe to filter out all XML files since we are not interested in any XML file from the list
                // of ResolvedFilesToPublish to become a static web asset. Things like this include XML doc files and
                // so on.
                ".xml" => "it is a documentation file",
                ".rsp" when fromMonoPackage => "extension is .rsp is not supported.",
                ".props" when fromMonoPackage => "extension is .props is not supported.",
                ".blat" when !timezoneSupport => "timezone support is not enabled.",
                ".dat" when invariantGlobalization && fileName.StartsWith("icudt") => "invariant globalization is enabled",
                ".dat" when !string.IsNullOrEmpty(customIcuCandidateFilename) && fileName != customIcuCandidateFilename => "custom icu file will be used instead of icu from the runtime pack",
                ".json" when fromMonoPackage && (fileName == "emcc-props" || fileName == "package") => $"{fileName}{extension} is not used by Blazor",
                ".ts" when fromMonoPackage && fileName == "dotnet.d" => "dotnet type definition is not used by Blazor",
                ".ts" when fromMonoPackage && fileName == "dotnet-legacy.d" => "dotnet type definition is not used by Blazor",
                ".js" when assetType == "native" && fileName != "dotnet" => $"{fileName}{extension} is not used by Blazor",
                ".pdb" when !copySymbols => "copying symbols is disabled",
                ".symbols" when fromMonoPackage => "extension .symbols is not required.",
                _ => null
            };

            return reason != null;
        }

        public static string GetCandidateRelativePath(ITaskItem candidate)
        {
            var destinationSubPath = candidate.GetMetadata("DestinationSubPath");
            if (!string.IsNullOrEmpty(destinationSubPath))
                return $"_framework/{destinationSubPath}";

            var relativePath = candidate.GetMetadata("FileName") + candidate.GetMetadata("Extension");
            return $"_framework/{relativePath}";
        }

        public static ITaskItem GetCustomIcuAsset(ITaskItem candidate)
        {
            var customIcuCandidate = new TaskItem(candidate);
            var relativePath = GetCandidateRelativePath(customIcuCandidate);
            customIcuCandidate.SetMetadata("RelativePath", relativePath);
            customIcuCandidate.SetMetadata("AssetTraitName", "BlazorWebAssemblyResource");
            customIcuCandidate.SetMetadata("AssetTraitValue", "native");
            customIcuCandidate.SetMetadata("AssetType", "native");
            return customIcuCandidate;
        }

        public static bool TryGetAssetFilename(ITaskItem candidate, out string filename)
        {
            bool candidateIsValid = candidate != null && !string.IsNullOrEmpty(candidate.ItemSpec);
            filename = candidateIsValid ?
                $"{candidate.GetMetadata("FileName")}" :
                "";
            return candidateIsValid;
        }
    }
}

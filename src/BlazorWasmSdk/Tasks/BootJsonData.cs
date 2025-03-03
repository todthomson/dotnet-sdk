// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Collections.Generic;
using System.Runtime.Serialization;
using ResourceHashesByNameDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Microsoft.NET.Sdk.BlazorWebAssembly
{
#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    /// Defines the structure of a Blazor boot JSON file
    /// </summary>
    public class BootJsonData50
    {
        /// <summary>
        /// Gets the name of the assembly with the application entry point
        /// </summary>
        public string entryAssembly { get; set; }

        /// <summary>
        /// Gets the set of resources needed to boot the application. This includes the transitive
        /// closure of .NET assemblies (including the entrypoint assembly), the dotnet.wasm file,
        /// and any PDBs to be loaded.
        ///
        /// Within <see cref="ResourceHashesByNameDictionary"/>, dictionary keys are resource names,
        /// and values are SHA-256 hashes formatted in prefixed base-64 style (e.g., 'sha256-abcdefg...')
        /// as used for subresource integrity checking.
        /// </summary>
        public ResourcesData50 resources { get; set; } = new ResourcesData50();

        /// <summary>
        /// Gets a value that determines whether to enable caching of the <see cref="resources"/>
        /// inside a CacheStorage instance within the browser.
        /// </summary>
        public bool cacheBootResources { get; set; }

        /// <summary>
        /// Gets a value that determines if this is a debug build.
        /// </summary>
        public bool debugBuild { get; set; }

        /// <summary>
        /// Gets a value that determines if the linker is enabled.
        /// </summary>
        public bool linkerEnabled { get; set; }

        /// <summary>
        /// Config files for the application
        /// </summary>
        public List<string> config { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICUDataMode"/> that determines how icu files are loaded.
        /// </summary>
        public ICUDataMode50 icuDataMode { get; set; }
    }

    public class ResourcesData50
    {
        /// <summary>
        /// .NET Wasm runtime resources (dotnet.wasm, dotnet.js) etc.
        /// </summary>
        public ResourceHashesByNameDictionary runtime { get; set; } = new ResourceHashesByNameDictionary();

        /// <summary>
        /// "assembly" (.dll) resources
        /// </summary>
        public ResourceHashesByNameDictionary assembly { get; set; } = new ResourceHashesByNameDictionary();

        /// <summary>
        /// "debug" (.pdb) resources
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ResourceHashesByNameDictionary pdb { get; set; }

        /// <summary>
        /// localization (.satellite resx) resources
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, ResourceHashesByNameDictionary> satelliteResources { get; set; }

        /// <summary>
        /// Assembly (.dll) resources that are loaded lazily during runtime
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ResourceHashesByNameDictionary lazyAssembly { get; set; }

        /// <summary>
        /// JavaScript module initializers that Blazor will be in charge of loading.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ResourceHashesByNameDictionary libraryInitializers { get; set; }

        /// <summary>
        /// Extensions created by users customizing the initialization process. The format of the file(s)
        /// is up to the user.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, ResourceHashesByNameDictionary> extensions { get; set; }

        /// <summary>
        /// Additional assets that the runtime consumes as part of the boot process.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, AdditionalAsset50> runtimeAssets { get; set; }

    }

    public enum ICUDataMode50 : int
    {
        // Note that the numeric values are serialized and used in JS code, so don't change them without also updating the JS code

        /// <summary>
        /// Load optimized icu data file based on the user's locale
        /// </summary>
        Sharded = 0,

        /// <summary>
        /// Use the combined icudt.dat file
        /// </summary>
        All = 1,

        /// <summary>
        /// Do not load any icu data files.
        /// </summary>
        Invariant = 2,

        /// <summary>
        /// Load custom icu file provided by the developer.
        /// </summary>
        Custom = 3,
    }

    [DataContract]
    public class AdditionalAsset50
    {
        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "behavior")]
        public string Behavior { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}

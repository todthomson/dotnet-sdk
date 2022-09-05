﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Cli.Commands
{
    internal class BaseUninstallCommand : BaseCommand<UninstallCommandArgs>
    {
        internal BaseUninstallCommand(
            Func<ParseResult, ITemplateEngineHost> hostBuilder,
            string commandName)
            : base(hostBuilder, commandName, SymbolStrings.Command_Uninstall_Description)
        {
            this.AddArgument(NameArgument);
        }

        internal static Argument<string[]> NameArgument { get; } = new("package")
        {
            Description = SymbolStrings.Command_Uninstall_Argument_Package,
            Arity = new ArgumentArity(0, 99)
        };

        protected override async Task<NewCommandStatus> ExecuteAsync(
            UninstallCommandArgs args,
            IEngineEnvironmentSettings environmentSettings,
            InvocationContext context)
        {
            using TemplatePackageManager templatePackageManager = new TemplatePackageManager(environmentSettings);
            TemplatePackageCoordinator templatePackageCoordinator = new TemplatePackageCoordinator(
                environmentSettings,
                templatePackageManager);

            return await templatePackageCoordinator.EnterUninstallFlowAsync(args, context.GetCancellationToken()).ConfigureAwait(false);
        }

        protected override UninstallCommandArgs ParseContext(ParseResult parseResult)
        {
            return new UninstallCommandArgs(this, parseResult);
        }
    }
}

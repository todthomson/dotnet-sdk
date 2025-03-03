﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;

namespace Microsoft.NET.Sdk.Razor.Tool
{
    internal struct ConnectionResult
    {
        public readonly Reason CloseReason;
        public readonly TimeSpan? KeepAlive;

        public ConnectionResult(Reason closeReason, TimeSpan? keepAlive = null)
        {
            CloseReason = closeReason;
            KeepAlive = keepAlive;
        }

        public enum Reason
        {
            /// <summary>
            /// There was an error creating the request object and a compilation was never created.
            /// </summary>
            CompilationNotStarted,

            /// <summary>
            /// The compilation completed and results were provided to the client.
            /// </summary>
            CompilationCompleted,

            /// <summary>
            /// The compilation process was initiated and the client disconnected before the results could be provided to them.
            /// </summary>
            ClientDisconnect,

            /// <summary>
            /// There was an unhandled exception processing the result.
            /// </summary>
            ClientException,

            /// <summary>
            /// There was a request from the client to shutdown the server.
            /// </summary>
            ClientShutdownRequest,
        }
    }
}

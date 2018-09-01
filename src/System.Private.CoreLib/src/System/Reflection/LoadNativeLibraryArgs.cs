﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Reflection
{
    public class LoadNativeLibraryArgs : EventArgs
    {
        public LoadNativeLibraryArgs(string libraryName, DllImportSearchPath dllImportSearchPath, Assembly callingAssembly)
        {
            LibraryName = libraryName;
            DllImportSearchPath = dllImportSearchPath;
            CallingAssembly = callingAssembly;
        }

        public string LibraryName { get; set; }
        public DllImportSearchPath DllImportSearchPath { get; set; }
        public Assembly CallingAssembly { get; set; }
    }
}

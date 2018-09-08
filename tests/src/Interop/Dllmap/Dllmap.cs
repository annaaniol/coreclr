// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;

using Console = Internal.Console;

public class Dllmap
{
    private static int s_failures = 0;

    [DllImport("WrongLibraryName", EntryPoint = "Sum")]
    public extern static int Sum(int a, int b);

    [DllImport("WrongLibraryName.dll", EntryPoint = "Multiply")]
    public extern static int Multiply(int a, int b);

    [DllImport("WrongLibraryName", EntryPoint = "Substract")]
    public extern static int Substract(int a, int b);

    private static void ExpectSuccessnOnRegistering()
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        try
        {
            NativeLibrary.RegisterNativeLibraryLoadCallback(callingAssembly, TestCallbackHandler);
        }
        catch (Exception e)
        {
            Console.WriteLine("Registering a callback for an assembly throws unexpected exception: " + e.Message);
            s_failures++;
        }
    }

    private static void ExpectExceptionOnRegistering()
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        string expectedMessage = "Callback for " + callingAssembly.GetName().Name + " has already been registered.";

        try
        {
            NativeLibrary.RegisterNativeLibraryLoadCallback(callingAssembly, TestCallbackHandler);
            s_failures++;
        }
        catch (Exception e)
        {
            if (!(e is CallbackAlreadyRegisteredException) || e.Message != expectedMessage)
            {
                s_failures++;
            }
        }
    }

    private static void ExpectSuccessSum()
    {
        try
        {
            if (5 != Sum(2, 3))
            {
                Console.WriteLine("Dll returns incorrectly result for Sum!");
                s_failures++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Dll throws unexpected exception: " + e.Message);
            s_failures++;
        }
    }

    private static void ExpectSuccessMultiply()
    {
        try
        {
            if (6 != Multiply(2, 3))
            {
                Console.WriteLine("Dll returns incorrectly result for Multiply!");
                s_failures++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Dll throws unexpected exception: " + e.ToString());
            s_failures++;
        }
    }

    private static void ExpectExceptionOnSubstract()
    {
        try
        {
            int result = Substract(2, 3);
            s_failures++;
        }
        catch (Exception e)
        {
            if (!(e is EntryPointNotFoundException))
            {
                Console.WriteLine("Calling an unexisting entrypint in mapped library throws unexpected exception: " + e.ToString());
                s_failures++;
            }
        }
    }

    public static Func<LoadNativeLibraryArgs, NativeLibrary> TestCallbackHandler = TestCallbackHandlerLogic;

    public static NativeLibrary TestCallbackHandlerLogic(LoadNativeLibraryArgs args)
    {
        string libraryName = args.LibraryName;
        DllImportSearchPath dllImportSearchPath = args.DllImportSearchPath;
        Assembly assembly = args.CallingAssembly;

        if (libraryName == "WrongLibraryName")
        {
            libraryName = "Library";
            NativeLibrary nativeLibrary = NativeLibrary.Load(libraryName, dllImportSearchPath, assembly);
            return nativeLibrary;
        }
        else if (libraryName == "WrongLibraryName.dll")
        {
            libraryName = "Library.dll";
            NativeLibrary nativeLibrary = NativeLibrary.Load(libraryName, dllImportSearchPath, assembly);
            return nativeLibrary;
        }
        return new NativeLibrary("LibraryNotFound", IntPtr.Zero);
    }


    public static int Main()
    {
        ExpectSuccessnOnRegistering();
        ExpectExceptionOnRegistering();
        ExpectSuccessSum();
        ExpectSuccessMultiply();
        ExpectExceptionOnSubstract();

        if (s_failures > 0)
        {
            Console.WriteLine("Failed!");
            return 101;
        }
        else
        {
            Console.WriteLine("Succeed!");
            return 100;
        }
    }
}

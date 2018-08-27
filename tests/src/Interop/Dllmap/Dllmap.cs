// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime;
using TestLibrary;

public class Dllmap
{
    private static int s_failures = 0;

    [DllImport("WrongLibraryName", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Sum")]
    public extern static int Sum(int a, int b);

    private static void Simple()
    {
        try
        {
            if (5 != Sum(2, 3))
            {
                Console.WriteLine("Dll returns incorrectly result!");
                s_failures++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Dll throws unexpected exception: " + e.Message);
            s_failures++;
        }
    }

    public static int Main()
    {
        Simple();

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

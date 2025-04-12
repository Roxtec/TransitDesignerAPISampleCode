// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.WinForms;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows.Forms;
using CefSharp;

namespace BrowserWrapper;

public class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var exitCode = CefSharp.BrowserSubprocess.SelfHost.Main(args);
        if (exitCode >= 0)
        {
            return exitCode;
        }

        var settings = new CefSettings()
        {
            //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
            CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
        };

        // We use our Applications exe as the BrowserSubProcess, multiple copies
        // will be spawned
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        settings.BrowserSubprocessPath = exePath;

        // Specify the standard user data path, to reuse login cookies etc.
        settings.RootCachePath = @$"{localAppData}\Google\Chrome\User Data";
            
        // performDependencyCheck must be false as CEF dependencies are not copied to the
        // target/output directory.
        Cef.Initialize(settings, performDependencyCheck: false);

        var applicationUrl = args.Length > 0 ? args[0] : BrowserForm.DefaultApplicationUrl;
        var browser = new BrowserForm(applicationUrl);
        Application.Run(browser);

        return 0;
    }
}
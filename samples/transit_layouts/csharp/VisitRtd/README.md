# VisitRtd sample

## Build

To build this sample, run:

    dotnet build /p:Platform=x64

This will build 64-bit applications (preferred). To build 32-bit
applications instead, run:

    dotnet build /p:Platform=x86

## Run

To run the sample, run:

    Launcher\bin\x64\Debug\net9.0-windows\Launcher.exe <project ID> <project API key>

Or for the 32-bit version:

    Launcher\bin\x86\Debug\net9.0-windows\Launcher.exe <project ID> <project API key>

Launcher is a console application that will:

1. Create a new transit with cables so that the fill rate is too high.
2. Open Transit Designer using the Chrome-based BrowserWrapper.
3. Wait for Transit Designer to exist.
4. Fetch the transit again and print fill rate.

The reason for dividing the sample into two parts is that
BrowserWrapper is a Windows desktop application that runs without
a console.

## Notes

The solution has been created using Visual Studio 2019.

BrowserWrapper is based on [CefSharp.MinimalExample](https://github.com/cefsharp/CefSharp.MinimalExample/).
Its license can be found in LICENSE-CefSharp.

The UI can be customized by changing the file _BrowserWrapper\BrowserForm.Customize.cs_.
# WebViewScreensaver

A Windows screensaver that displays web content. By default it shows an embedded Matrix digital rain animation. You can configure it to display any URL instead.

## Requirements

- Windows 10/11 (x64)
- [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)

## Installation

1. Build in Release mode (requires .NET SDK):
   ```powershell
   dotnet build -c Release
   ```
2. Run the install script as Administrator:
   ```powershell
   .\install.ps1
   ```
   This copies `WebViewScreensaver.scr`, the WebView2 DLLs, and the embedded `matrix/` web app to `C:\Windows\System32\`.

3. Open **Screen Saver Settings** (right-click desktop → Personalize → Lock screen → Screen saver) and select **WebViewScreensaver**.

## Configuration

Click **Settings** in the Screen Saver Settings dialog to set a custom URL. Leave it blank to use the default embedded screensaver.

The URL is stored in the registry at `HKCU\Software\NascentMaker\Screensaver`.

## Default Screensaver

The embedded default is a Matrix digital rain animation from [rezmason/matrix](https://github.com/rezmason/matrix) (the "Resurrections" variant). It runs entirely offline via a virtual host mapped to the bundled `matrix/` folder.

## Custom URLs

Any URL can be used as the screensaver content. The screensaver dismisses on any key press, mouse click, or mouse movement greater than 5 pixels.

## Multi-Monitor

One WebView2 window is created per monitor, each filling its screen at the correct DPI.

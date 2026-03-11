# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build

**Always use PowerShell for builds** — `dotnet` is not in bash's PATH on this machine:

```powershell
# Debug build
powershell -Command "cd '<repo-root>'; dotnet build 2>&1"

# Release build
powershell -Command "cd '<repo-root>'; dotnet build -c Release 2>&1"
```

Post-build automatically copies `WebViewScreensaver.exe` → `WebViewScreensaver.scr` in `bin\<config>\net48\`.

**Install to system** (copies .scr, WebView2 DLLs, and matrix folder to `C:\Windows\System32\`):
```powershell
.\install.ps1
```

## Key Platform Constraints

- **Target**: `net48` (WPF/.NET Framework 4.8) built with .NET 10 SDK
- **`<LangVersion>latest</LangVersion>`** is required — net48 defaults to C# 7.3 which lacks nullable
- **`IntPtr.TryParse` does not exist in net48** — use `new IntPtr(long.Parse(...))`
- **`using System.Runtime.InteropServices`** must be explicit in files using P/Invoke (`HandleRef` etc.)

## Architecture

### Screensaver Modes (Program.cs routing)
- **No args / unknown** → `ConfigWindow`
- **`/c`** → `ConfigWindow` (Settings dialog)
- **`/s`** → `RunScreensaver()` — one `ScreensaverWindow` per `Screen.AllScreens`
- **`/p <hwnd>`** → `PreviewWindow` — reparented as WS_CHILD into the Settings preview pane

### Multi-Monitor & Initialization Sync
`RunScreensaver()` creates all `ScreensaverWindow` instances, collects their `ReadyTask` (a `TaskCompletionSource<bool>` signaled when WebView2 finishes init), waits for all via `Task.WhenAll()`, then waits an extra 500ms grace period before installing `GlobalHooks`. This prevents spurious mouse events during window creation from immediately dismissing the screensaver.

### WebView2 Shared Environment
`WebViewEnvironment.cs` caches a single `CoreWebView2Environment` (user data: `%APPDATA%\NascentMaker\Screensaver\WebView2`) and exposes `SetupVirtualHost()` which maps `matrix.screensaver.local` → the `matrix/` folder in the output directory. Both `ScreensaverWindow` and `PreviewWindow` call this after WebView2 init to enable offline operation.

### Configuration (RegistryHelper.cs)
Registry key: `HKCU\Software\NascentMaker\Screensaver` → `Url` value.
- Empty or legacy remote URL → returns `DefaultUrl` (embedded `https://matrix.screensaver.local/index.html?version=resurrections`)
- `GetCustomUrl()` returns `null` if using default, otherwise the stored URL

### Input Dismissal (GlobalHooks.cs)
Installs `WH_MOUSE_LL` and `WH_KEYBOARD_LL` hooks after the grace period. Mouse movement only dismisses if > 5px from initial position (first move is recorded, not acted upon). Hook delegate fields are kept as class members to prevent GC collection.

### Preview Window (PreviewWindow.xaml.cs)
After `OnSourceInitialized`, uses `SetParent()` + `SetWindowLong()` to convert the WPF window from `WS_POPUP` to `WS_CHILD | WS_VISIBLE` and embed it in the Settings preview pane. DPI is assumed 96 since the preview is tiny.

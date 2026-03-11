$src = Join-Path $PSScriptRoot 'bin\Release\net48'
$dst = 'C:\Windows\System32'

Copy-Item "$src\WebViewScreensaver.scr" "$dst\WebViewScreensaver.scr" -Force
Copy-Item "$src\WebView2Loader.dll" "$dst\WebView2Loader.dll" -Force
Copy-Item "$src\Microsoft.Web.WebView2.Core.dll" "$dst\Microsoft.Web.WebView2.Core.dll" -Force
Copy-Item "$src\Microsoft.Web.WebView2.WinForms.dll" "$dst\Microsoft.Web.WebView2.WinForms.dll" -Force
Copy-Item "$src\Microsoft.Web.WebView2.Wpf.dll" "$dst\Microsoft.Web.WebView2.Wpf.dll" -Force
Copy-Item "$src\matrix" "$dst\matrix" -Recurse -Force
Write-Host "Installed successfully."

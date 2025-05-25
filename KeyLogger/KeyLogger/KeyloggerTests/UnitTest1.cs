/**************************************************************************
 *                                                                        *
 *  File:        UnitTest1.cs                                             *
 *  Copyright:   (c) 2025, Adelina-Petronela Hritcu,                      *
 *                         Raluca-Elena Musteata,                         *
 *                         Alexandru Simiuc,                              *
 *                         Rares-Mihai Vasilca                            *
 *                                                                        *
 *  E-mail:      raluca-elena.musteata@student.tuiasi.ro                  *
 *               petronela-adelina.hritcu@student.tuiasi.ro               *
 *               alexandru.simiuc@student.tuiasi.ro                       *  
 *               rares-mihai.vasilca@student.tuiasi.ro                    *
 *  Description: Implementare keylogger                                   *
 *               Această aplicație implementează un keylogger în C#       *
 *               folosind modelul de proiectare Observer. Proiectul are   *
 *               o arhitectură modulară, ceea ce face codul ușor de       *
 *               extins și întreținut, și include teste unitare pentru    *
 *               a asigura fiabilitatea componentelor. Aplicația este     *
 *               structurată pentru integrare ușoară în alte proiecte     *
 *               sau adaptare la diverse scenarii de monitorizare.        *
 *                                                                        *
 **************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Keylogger;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyloggerTests
{
    [TestClass]
    public class teste_keylogger_full
    {
        /// <summary>
        /// creeaza un keylogger inactiv pentru testare
        /// </summary>
        /// <returns>instanta Keylogger cu IsActive = false</returns>
        private Keylogger.Keylogger creaKL()
        {
            var kl = new Keylogger.Keylogger();
            kl.IsActive = false; // setare inactiv explicit
            return kl;
        }

        private class TestableKeylogger : Keylogger.Keylogger
        {
            private bool _forceCapsLock;
            public void SetForceCapsLockState(bool state) => _forceCapsLock = state;
            // Override to mock Caps Lock state
            protected override bool IsCapsLockOn => _forceCapsLock;
        }

        /// <summary>
        /// aloca un pointer IntPtr cu valoarea codului dat (folosit pentru testare)
        /// </summary>
        /// <param name="cod">codul ce va fi scris în pointer</param>
        /// <returns>pointer la memorie alocata cu codul scris</returns>
        private IntPtr alocLP(int cod)
        {
            IntPtr ptr = Marshal.AllocHGlobal(4); // aloc 4 bytes
            Marshal.WriteInt32(ptr, cod); // scriu codul în memorie
            return ptr;
        }

        // -------------------------
        // 1. teste pozitive
        // -------------------------

        /// <summary>
        /// testeaza transformarea tastei 'a' fara shift in "a"
        /// </summary>
        [TestMethod]
        public void t_min_a()
        {
            var kl = creaKL();
            // reflecta metoda privată GetKeyName
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            // apel metodei cu key A - shift false
            var res = (string)m.Invoke(kl, new object[] { Keys.A, false });
            Assert.AreEqual("a", res); // verifica sa fie litera mica
        }

        /// <summary>
        /// testeaza transformarea tastei 'A' cu shift în "A"
        /// </summary>
        [TestMethod]
        public void t_maj_a()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(kl, new object[] { Keys.A, true });
            Assert.AreEqual("A", res);
        }

        /// <summary>
        /// testeaza cifra '1' fara shift în "1"
        /// </summary>
        [TestMethod]
        public void t_digit1_no_shift()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(kl, new object[] { Keys.D1, false });
            Assert.AreEqual("1", res);
        }

        /// <summary>
        /// testeaza (!) pentru Shift+1
        /// </summary>
        [TestMethod]
        public void t_exclam_d1()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(kl, new object[] { Keys.D1, true });
            Assert.AreEqual("!", res);
        }

        /// <summary>
        /// testeaza daca tasta OemQuestion este considerata deja shiftata
        /// </summary>
        [TestMethod]
        public void t_shifted_oem()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("IsKeyAlreadyShifted", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (bool)m.Invoke(kl, new object[] { Keys.OemQuestion });
            Assert.IsTrue(res);
        }

        /// <summary>
        /// testeaza daca subscribe returnează un ID nenul
        /// </summary>
        [TestMethod]
        public void t_subscribe()
        {
            var kl = creaKL();
            var obs = new Obs();
            var u = kl.Subscribe(obs);
            Assert.IsNotNull(u);
        }

        /// <summary>
        /// testeaza daca keyloggerul este implicit inactiv la creare
        /// </summary>
        [TestMethod]
        public void t_default_inactive()
        {
            var kl = new Keylogger.Keylogger();
            Assert.IsFalse(kl.IsActive);
        }

        /// <summary>
        /// testeaza pornirea keylogger-ului (IsActive = true)
        /// </summary>
        [TestMethod]
        public void t_start()
        {
            var kl = new Keylogger.Keylogger();
            kl.IsActive = true;
            Assert.IsTrue(kl.IsActive);
        }

        /// <summary>
        /// testeaza oprirea keylogger-ului (IsActive = false)
        /// </summary>
        [TestMethod]
        public void t_stop()
        {
            var kl = new Keylogger.Keylogger();
            kl.IsActive = false;
            Assert.IsFalse(kl.IsActive);
        }

        /// <summary>
        /// testeaza daca HookCallback notifica o singura data cand keyloggerul este activ
        /// </summary>
        [TestMethod]
        public void t_hook_active()
        {
            var kl = creaKL();
            var obs = new Obs();
            var unsub = kl.Subscribe(obs);
            kl.IsActive = true; // activez keylogger
            var h = typeof(Keylogger.Keylogger)
                .GetMethod("HookCallback", BindingFlags.NonPublic | BindingFlags.Instance);
            var lp = alocLP((int)Keys.B);
            h.Invoke(kl, new object[] { 0, (IntPtr)0x0100, lp }); // simulez eveniment tasta
            Marshal.FreeHGlobal(lp); // elibereza mem
            Assert.AreEqual(1, obs.Count); // verific nr de notificari
        }

        // -------------------------
        // 2. teste negative
        // -------------------------

        /// <summary>
        /// test negativ: asteapta "A" în loc de "a"
        /// </summary>
        [TestMethod]
        public void n_get_a_fail()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(kl, new object[] { Keys.A, false });
            Assert.AreEqual("A", res);
        }

        /// <summary>
        /// test negativ: asteapta  (%) în loc de (!)
        /// </summary>
        [TestMethod]
        public void n_get_d1_fail()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(kl, new object[] { Keys.D1, true });
            Assert.AreEqual("%", res);
        }

        /// <summary>
        /// test negativ: se asteapta true pentru tasta 'a' la IsKeyAlreadyShifted
        /// </summary>
        [TestMethod]
        public void n_shifted_a_fail()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("IsKeyAlreadyShifted", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (bool)m.Invoke(kl, new object[] { Keys.A });
            Assert.IsTrue(res);
        }

        /// <summary>
        /// test negativ: subscribe asteapta sa returneze null
        /// </summary>
        [TestMethod]
        public void n_sub_fail()
        {
            var kl = creaKL();
            var obs = new Obs();
            var u = kl.Subscribe(obs);
            Assert.IsNull(u);
        }

        /// <summary>
        /// test negativ: count trebuie sa fie 0 cand keylogger-ul e activ
        /// </summary>
        [TestMethod]
        public void n_hook_active_fail()
        {
            var kl = creaKL();
            var obs = new Obs();
            var unsub = kl.Subscribe(obs);
            kl.IsActive = true;
            var h = typeof(Keylogger.Keylogger)
                .GetMethod("HookCallback", BindingFlags.NonPublic | BindingFlags.Instance);
            var lp = alocLP((int)Keys.G);
            h.Invoke(kl, new object[] { 0, (IntPtr)0x0100, lp });
            Marshal.FreeHGlobal(lp);
            Assert.AreEqual(0, obs.Count);
        }

        // -------------------------
        // 3. teste de validare (exceptii si fallback)
        // -------------------------

        /// <summary>
        /// test care asteapta o exceptie cand GetKeyName este apelat fara parametrul shift
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TargetParameterCountException))]
        public void v_get_param_fail()
        {
            var kl = creaKL();
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            // lipsa param -> va arunca exceptie
            m.Invoke(kl, new object[] { Keys.A });
        }

        /// <summary>
        /// test fallback pentru tasta F13: returneaza ToString() in loc de mapare speciala
        /// </summary>
        [TestMethod]
        public void v_unsupported_F13_fallback()
        {
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(creaKL(), new object[] { Keys.F13, false });
            Assert.AreEqual("F13", res);
        }

        /// <summary>
        /// test fallback pentru tasta OemSemicolon (colon ro): trebuie sa fie mapata la ";"
        /// </summary>
        [TestMethod]
        public void v_colon_ro_fallback_to_semicolon()
        {
            var m = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (string)m.Invoke(creaKL(), new object[] { Keys.OemSemicolon, false });
            Assert.AreEqual(";", res);
        }

        /// <summary>
        /// testeaza logica pentru Shift+CapsLock: litera trebuie sa fie mica
        /// </summary>
        [TestMethod]
        public void v_shiftA_withCapsLock()
        {
            var keylogger = new TestableKeylogger();
            keylogger.SetForceCapsLockState(true); 
            var method = typeof(Keylogger.Keylogger)
                .GetMethod("GetKeyName", BindingFlags.NonPublic | BindingFlags.Instance);
            string result = (string)method.Invoke(keylogger, new object[] { Keys.A, true });
            Assert.AreEqual("a", result); 
        }

        /// <summary>
        /// testeaza ca dublul apel Dispose nu arunca exceptie
        /// </summary>
        [TestMethod]
        public void v_double_dispose_does_not_throw()
        {
            var kl = creaKL();
            kl.Dispose();
            kl.Dispose(); // apel dublu fara efecte adverse
            Assert.IsTrue(true);
        }

        /// <summary>
        /// observer helper pentru testare notificari
        /// </summary>
        private class Obs : IObserver<KeyEvent>
        {
            public int Count { get; private set; }
            public KeyEvent Last { get; private set; }
            public void OnNext(KeyEvent v) { Count++; Last = v; }
            public void OnError(Exception e) { }
            public void OnCompleted() { }
        }
    }
}

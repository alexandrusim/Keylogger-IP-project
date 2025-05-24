/**************************************************************************
 *                                                                        *
 *  File:        Form1.cs                                                 *
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
using System.Windows.Forms;
using Keylogger;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KeyloggerApp
{
    public partial class Form1 : Form, IObserver<KeyEvent>
    {
        //private KeyloggerApp _keylogger;
        private Keylogger.Keylogger _keylogger;
        private IDisposable _unsubscriber;
        private bool _isMonitoring = false;
        /// <summary>
        /// Fereastra principala a aplicatiei Keylogger
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            textLog.ReadOnly = true;  // Add this line
            InitializeKeylogger();
        }
        /// <summary>
        /// Initializeaza fereastra si componenta keylogger
        /// </summary>
        private void InitializeKeylogger()
        {
            try
            {
                _keylogger = new Keylogger.Keylogger();
                _unsubscriber = _keylogger.Subscribe(this);
                _keylogger.IsActive = false; // Start inactive
                labelStatus.Text = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize keylogger: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        /// <summary>
        /// Gestioneaza noile evenimente de tastare primite de la keylogger
        /// </summary>
        /// <param name="value">Datele evenimentului de tastare</param>
        public void OnNext(KeyEvent value)
        {
            try
            {
                if (textLog.InvokeRequired)
                {
                    textLog.Invoke(new Action(() => OnNext(value)));
                }
                else
                {
                    textLog.AppendText($"[{value.Timestamp:HH:mm:ss}] {value.Key}\n");
                    textLog.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la procesarea evenimentului de tastare: {ex.Message}", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Gestioneaza erorile aparute in keylogger
        /// </summary>
        /// <param name="error">Exceptia aparuta</param>
        public void OnError(Exception error)
        {
            MessageBox.Show($"Keylogger error: {error.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// Gestioneaza finalizarea monitorizarii tastelor
        /// </summary>
        public void OnCompleted()
        {
            textLog.AppendText("Monitoring completed.\n");
        }
        /// <summary>
        /// Porneste monitorizarea tastelor
        /// </summary>
        private void startButton_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                _keylogger.IsActive = true;
                _isMonitoring = true;
                labelStatus.Text = "Monitoring...";
                textLog.AppendText("----- Monitoring Started -----\n");
            }
        }
        /// <summary>
        /// Opreste monitorizarea tastelor
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                _keylogger.IsActive = false;
                _isMonitoring = false;
                labelStatus.Text = "Paused";
                textLog.AppendText("----- Monitoring Paused -----\n");
            }
        }
        /// <summary>
        /// Elibereaza resursele la inchiderea ferestrei
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _unsubscriber?.Dispose();
            _keylogger?.Dispose();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Inchide aplicatia
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        /// <summary>
        /// Afiseaza informatii despre aplicatie
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Implementare keylogger");
        }
        /// <summary>
        /// Deschide documentatia de ajutor
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("KeyLogger.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la deschiderea fisierului de ajutor: {ex.Message}", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
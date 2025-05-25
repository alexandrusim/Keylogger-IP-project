/**************************************************************************
 *                                                                        *
 *  File:        Form1.cs                                                 *
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
        /// fereastra princ a aplicației Keylogger - gestioneaza interactiunile UI si observa evenimentele tastelor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            textLog.ReadOnly = true;  // Add this line
            InitializeKeylogger();
        }
        /// <summary>
        /// init fereastra si componenta keylogger
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
        /// gestioneaza noile evenimente de tastare primite de la keylogger
        /// </summary>
        /// <param name="value">datele evenimentului de tastare</param>
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
        /// gestioneaza erorile aparute in keylogger
        /// </summary>
        /// <param name="error">exceptia aparuta</param>
        public void OnError(Exception error)
        {
            MessageBox.Show($"Keylogger error: {error.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// gestioneaza finalizarea monitorizarii tastelor
        /// </summary>
        public void OnCompleted()
        {
            textLog.AppendText("Monitoring completed.\n");
        }
        /// <summary>
        /// porneste monitorizarea tastelor
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
        /// opreste monitorizarea tastelor
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
        /// elibereaza resursele la inchiderea ferestrei
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _unsubscriber?.Dispose();
            _keylogger?.Dispose();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// inchide aplicatia
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        /// <summary>
        /// afiseaza informatii despre aplicatie
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Acest proiect implementează un keylogger modular în C#, folosind modelul de design Observer pentru a monitoriza și înregistra apăsările de tastă. Aplicația utilizează hook-uri Windows pentru a intercepta evenimentele de la tastatură și notifică observatorii înregistrați atunci când apar acțiuni. Soluția este proiectată pentru a fi ușor extensibilă și integrabilă în alte aplicații.\n\nProiect realizat de:\nHrițcu Petronela\nMusteață Raluca-ELena,\nSimiuc Alexandru\nVasilca Rareș-Mihai");
        }
        /// <summary>
        /// deschide documentatia de ajutor
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("ProiectIP.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la deschiderea fisierului de ajutor: {ex.Message}", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
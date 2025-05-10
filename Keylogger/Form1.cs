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
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/
using System;
using System.Windows.Forms;

namespace Keylogger
{
    public partial class Form1 : Form, IObserver<KeyEvent>
    {
        private Keylogger _keylogger;
        private IDisposable _unsubscriber;
        private bool _isMonitoring = false;

        public Form1()
        {
            InitializeComponent();
            textLog.ReadOnly = true;  // Add this line
            InitializeKeylogger();
        }

        private void InitializeKeylogger()
        {
            try
            {
                _keylogger = new Keylogger();
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

        public void OnNext(KeyEvent value)
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

        public void OnError(Exception error)
        {
            MessageBox.Show($"Keylogger error: {error.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void OnCompleted()
        {
            textLog.AppendText("Monitoring completed.\n");
        }

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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _unsubscriber?.Dispose();
            _keylogger?.Dispose();
            base.OnFormClosing(e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Implementare keylogger");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("KeyLogger.chm");
        }
    }
}
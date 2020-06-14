using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics.Tracing;

namespace SwitchSendkey
{
    public partial class Form1 : Form
    {
        SerialPort sio = new SerialPort();
        string default_port;
        public Form1()
        {
            InitializeComponent();
            // Create a new SerialPort object with default settings.

            // Allow the user to set the appropriate properties.
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendKeys.Send(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendKeys.Send(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendKeys.Send(textBox3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendKeys.Send(textBox4.Text);
        }

        private void fire_sendkey(string command)
        {
            if (command[0]<0x20)
            {
                command = command.Substring(1);
            }
            if (command=="Switch 1 ON")
            {
                SendKeys.Send(textBox1.Text);
            }
            else if (command == "Switch 2 ON")
            {
                SendKeys.Send(textBox2.Text);
            }
            else if (command == "Switch 3 ON")
            {
                SendKeys.Send(textBox3.Text);
            }
            else if (command == "Switch 4 ON")
            {
                SendKeys.Send(textBox4.Text);
            }
        }
        private void LogString(string str)
        {
            textBoxLog.Text += str + "\r\n";
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.Focus();
            textBoxLog.ScrollToCaret();
        } 
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!sio.IsOpen) return;
            if (sio.BytesToRead > 0)
            {
                try
                {
                    string buff = sio.ReadLine();
                    LogString(buff);
                    
                    fire_sendkey(buff);
                }
                catch (TimeoutException timeex)
                {

                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sio.Close();
            Properties.Settings.Default.Save();
        }

        private void open_sio()
        {
            if (sio.IsOpen)
            {
                sio.Close();
            }
            sio.PortName = default_port;
            sio.BaudRate = 9600;
            sio.Parity = Parity.None;
            sio.DataBits = 8;
            sio.StopBits = StopBits.One;
            sio.Handshake = Handshake.None;
            sio.NewLine = "\r\n";

            // Set the read/write timeouts
            sio.ReadTimeout = 100;
            sio.WriteTimeout = 100;

            sio.Open();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // arduino default is 8bit、パリティなし、1ストップビット(SERIAL_8N1)
            default_port = Properties.Settings.Default.port;
            bool find_default_port = false;
            foreach (string port in SerialPort.GetPortNames())
            {
                int n = comboBoxPort.Items.Add(port);
                if (port == default_port)
                {
                    comboBoxPort.SelectedIndex = n;
                    find_default_port = true;
                }
            }
            if (comboBoxPort.Items.Count==0)
            {
                MessageBox.Show("シリアルポートが見つかりません", "エラー");
                Close();
            }
            if (!find_default_port)
            {
                default_port = comboBoxPort.Items[0].ToString();
                comboBoxPort.SelectedIndex = 0;
            }
            open_sio();
        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            default_port = comboBoxPort.SelectedItem.ToString();
            open_sio();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

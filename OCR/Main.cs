using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TessecratLib;
//using MODILib;

namespace OCR
{
    public partial class Main : Form
    {
        private String tessdatapath;
        private String outputfile;
        public Main()
        {
            InitializeComponent();
            QueueThread.Start();
            comboBox1.Items.Add("ENG");
            comboBox1.Items.Add("JPN");
            comboBox2.Items.Add("GOOGLE(Tessecrat)");
            comboBox2.Items.Add("MS(MODI)");
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            this.MaximumSize = this.Size;

            tessdatapath = Path.GetDirectoryName(Application.ExecutablePath);
            outputfile = "output.txt";
        }
        protected override void OnLoad(EventArgs e)
        {
            HideMessage();
            textBox2.Text = Path.GetDirectoryName(Application.ExecutablePath);
            base.OnLoad(e);
        }
        public void HideMessage()
        {
            QueueThread.InvokeControl(statusStrip1, () =>
            {
                toolStripProgressBar1.Visible = false;
                toolStripStatusLabel1.Text = "Ready...";
            });
        }
        public void ShowMessage(String msg)
        {
            QueueThread.InvokeControl(statusStrip1, () =>
            {
                toolStripProgressBar1.Visible = true;
                toolStripStatusLabel1.Text = msg;
            });
        }
        private void EnableControl(bool enable)
        {
            QueueThread.InvokeForm(this, () =>
            {
                button1.Enabled = enable;
                button2.Enabled = enable;
                button3.Enabled = enable;
                comboBox1.Enabled = enable;
                comboBox2.Enabled = enable;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = textBox2.Text;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("Please select the image file...");
            }
            QueueThread.Push(() =>
            {
                try
                {
                    EnableControl(false);
                    ShowMessage("Extracting...");
                    int index = QueueThread.InvokeControl(comboBox1, () =>
                    {
                        return comboBox1.SelectedIndex;
                    });
                    String img = QueueThread.InvokeControl(textBox1, () =>
                    {
                        return textBox1.Text.Trim();
                    });
                    String path = QueueThread.InvokeControl(textBox2, () =>
                    {
                        return textBox2.Text.Trim();
                    });
                    int ocrtype = QueueThread.InvokeControl(comboBox2, () =>
                    {
                        return comboBox2.SelectedIndex;
                    });
                    String text = "";
                    /*if (ocrtype == 1)
                    {
                        text = MODIOCRConverter.Run(img);
                    }
                    else
                    {
                        text = TessecratOCRConverter.Run(tessdatapath, index == 0 ? TessecratOCRConverter.LanguageType.ENG : TessecratOCRConverter.LanguageType.JPN, img);
                    }*/
                    text = TessecratOCRConverter.Run(tessdatapath, index == 0 ? TessecratOCRConverter.LanguageType.ENG : TessecratOCRConverter.LanguageType.JPN, img);
                    ShowMessage("Saving...");
                    SaveFile(path, outputfile, text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    HideMessage();
                    EnableControl(true);
                }

            });
        }
        private void SaveFile(String outputpath, String filename, String text)
        {
            String buffer = filename;
            for (int i = 0; true; i++)
            {
                if (File.Exists(outputpath + "\\" + buffer))
                {
                    int pos = filename.LastIndexOf(".");
                    if (pos != -1)
                    {
                        String pre = filename.Substring(0, pos);
                        String extension = filename.Substring(pos + 1, filename.Length - (pos + 1));
                        buffer = String.Format("{0}({1}).{2}", pre, i, extension);
                    }
                    else
                    {
                        buffer = String.Format("{0}({1})", filename, i);
                    }
                    continue;
                }
                break;
            }
            using (FileStream stream = new FileStream(outputpath + "\\" + buffer, FileMode.Create, FileAccess.Write))
            {
                byte[] temp = Encoding.UTF8.GetBytes(text);
                stream.Write(temp, 0, temp.Length);
            }
        }
    }
}

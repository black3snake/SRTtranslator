﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.Web.UI.WebControls;

namespace SRTtranslator
{
    public partial class Form1 : Form
    {
        List<string> listSelectFiles = new List<string>();
        static Logger logger = LogManager.GetCurrentClassLogger();

        public Form1()
        {
            InitializeComponent();
            
            listView1.MouseUp += (s, a) => {
                if (a.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(MousePosition, ToolStripDropDownDirection.Right);
                }
            };

            REFRESHToolStripMenuItem.Click += (s, a) => {
                if(!string.IsNullOrEmpty(labDir.Text))
                {
                    DirRefresh(labDir.Text, toolStripTextBox1.Text);
                }
            };

            TranslateToolStripMenuItem.Click += (s, a) =>
            {
                if(listView1.SelectedItems.Count > 1)
                {
                    if (ConsoleTB.TextLength > 0)
                        ConsoleTB.Clear();

                    if (bc.Count > 0)
                    {
                        //MyClass localItem;
                        while (bc.TryTake(out MyClass localItem)) {}
                    }
                        
                    foreach (ListViewItem lVitem in listView1.SelectedItems)
                    {
                        listSelectFiles.Add(labDir.Text + "\\" + lVitem.Text);
                    }
                    TranslatorHub(listSelectFiles);
                    listSelectFiles.Clear();
                    

                } else if (listView1.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Ничего не выбрано", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else
                {
                    if (ConsoleTB.TextLength > 0)
                        ConsoleTB.Clear();

                    listSelectFiles.Add(labDir.Text + "\\"+ listView1.SelectedItems[0].Text);
                    TranslatorHub(listSelectFiles);
                    listSelectFiles.Clear();
                }


            };



            listView1.SmallImageList = imageList1;

            #region NLog Initializator

            var config = new NLog.Config.LoggingConfiguration();
            LogManager.Configuration = new LoggingConfiguration();
            const string LayoutFile = @"[${date:format=yyyy-MM-dd HH\:mm\:ss}] [${logger}/${uppercase: ${level}}] [THREAD: ${threadid}] >> ${message}
${exception: format=ToString}";

            var logfile = new FileTarget();

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

            DateTime dtlog = DateTime.Now;

            logfile.CreateDirs = true;
            logfile.FileName = $"logs{Path.DirectorySeparatorChar}{dtlog:yyyy-MM-dd}.log";
            logfile.AutoFlush = true;
            logfile.LineEnding = LineEndingMode.CRLF;
            logfile.Layout = LayoutFile;
            logfile.FileNameKind = FilePathKind.Absolute;
            logfile.ConcurrentWrites = false;
            logfile.KeepFileOpen = false;


            // Apply config
            NLog.LogManager.Configuration = config;

            #endregion NLog Initializator

            logger.Info($"!!!---- Start Main Program {DateTime.Now}");


        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrEmpty(folderBrowser.SelectedPath))
            {
                labDir.Text = folderBrowser.SelectedPath;

                DirRefresh(folderBrowser.SelectedPath, toolStripTextBox1.Text);
            
            }

        }

    }
}

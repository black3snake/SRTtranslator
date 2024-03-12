using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRTtranslator
{
    public partial class Form1 : Form
    {
        List<string> listSelectFiles = new List<string>();


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
                    foreach (ListViewItem lVitem in listView1.SelectedItems)
                    {
                        listSelectFiles.Add(labDir.Text + "\\" + lVitem.Text);
                    }

                } else if (listView1.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Ничего не выбрано", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else
                {
                    if(listSelectFiles.Count > 0)
                        listSelectFiles.Clear();
                    
                    if (ConsoleTB.TextLength > 0)
                        ConsoleTB.Clear();

                    listSelectFiles.Add(labDir.Text + "\\"+ listView1.SelectedItems[0].Text);
                    TranslatorHub(listSelectFiles);
                }


            };



            listView1.SmallImageList = imageList1;

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

                /*string[] files = Directory.GetFiles(folderBrowser.SelectedPath, toolStripTextBox1.Text, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    ListViewItem lvi = new ListViewItem();
                    // установка названия файла
                    lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                    lvi.ImageIndex = 1; // установка картинки для файла
                                        // добавляем элемент в ListView
                    listView1.Items.Add(lvi);

                }*/
            
            }

        }

    }
}

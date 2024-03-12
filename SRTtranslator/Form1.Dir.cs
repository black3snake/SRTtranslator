using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRTtranslator
{
    public partial class Form1
    {
        internal void DirRefresh(string path, string mask)
        {
            string[] files = Directory.GetFiles(path, mask, SearchOption.AllDirectories);
            if(files.Length > 0)
            {
                if(listView1.Items.Count >0)
                    listView1.Items.Clear();
                
                foreach (string file in files)
                {
                    ListViewItem lvi = new ListViewItem();
                    // установка названия файла
                    lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                    lvi.ImageIndex = 0; // установка картинки для файла
                                        // добавляем элемент в ListView
                    listView1.Items.Add(lvi);

                }
                labCountFiles.Text = files.Length.ToString();
            }

        }
    }
}

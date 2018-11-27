using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrepSharp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            tboxDirectory.Text = Directory.GetCurrentDirectory();
            comboBox1.SelectedIndex = 2;
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tboxDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/Fhoughton");
        }

        private void addFile(string name,string size,string matches, string path)
        {
            ListViewItem item1 = new ListViewItem(name);
            item1.SubItems.Add(size);
            item1.SubItems.Add(matches);
            item1.SubItems.Add(path);

            listView1.Items.AddRange(new ListViewItem[] { item1 });
        }

        private long getSize(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Length;
        }

        private bool checkText(string path, string text)
        {
            string contents = File.ReadAllText(path);
            return contents.Contains(text);
        }

        private bool checkTextInsensitive(string path, string text)
        {
            string contents = File.ReadAllText(path);
            return Regex.IsMatch(contents, text, RegexOptions.IgnoreCase);
        }

        private bool checkRegex(string path, string regex)
        {
            string contents = File.ReadAllText(path);
            return Regex.IsMatch(contents, regex);
        }

        public static List<string>DirectorySearchAll(string dir)
        {
            List<string> files = new List<string>();

            foreach (string f in Directory.GetFiles(dir))
            {
                files.Add(Path.GetFullPath(f));
            }
            foreach (string d in Directory.GetDirectories(dir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    files.Add(Path.GetFullPath(f));
                }
                DirectorySearchAll(d);
            }

            return files;
        }

        public static List<string> DirectorySearch(string dir)
        {
            List<string> files = new List<string>();

            foreach (string f in Directory.GetFiles(dir))
            {
                files.Add(Path.GetFullPath(f));
            }

            return files;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            List<string> found = new List<string>();
            List<string> remove = new List<string>();
            List<int> size = new List<int>();
            if (checkBox1.Checked == false) { foreach (string f in DirectorySearch(tboxDirectory.Text)) { found.Add(f); } }
            if (checkBox1.Checked == true) { foreach (string f in DirectorySearchAll(tboxDirectory.Text)) { found.Add(f); } }

            if (radioButton1.Checked)
            {
                if (cboxCaseSensitive.Checked)
                {
                    foreach (string f in found)
                    {
                        if (!checkText(f, textBox1.Text)) { remove.Add(f); }
                    }
                }
                else
                {
                    foreach (string f in found)
                    {
                        if (!checkTextInsensitive(f, textBox1.Text)) { remove.Add(f); }
                    }
                }
            }

            if (radioButton2.Checked)
            {
                foreach (string f in found)
                {
                    if (!checkRegex(f, textBox2.Text)) { remove.Add(f); }
                }
            }

            if (radioButton4.Checked)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    foreach (string f in found)
                    {
                        FileInfo fi = new FileInfo(f);
                        if (fi.Length < int.Parse(textBox3.Text)) { remove.Add(f); };
                    }
                }

                if (comboBox1.SelectedIndex == 1)
                {
                    foreach (string f in found)
                    {
                            FileInfo fi = new FileInfo(f);
                            if (fi.Length > int.Parse(textBox3.Text)) { remove.Add(f); };
                    }
                }

                if (comboBox1.SelectedIndex == 2)
                {
                    foreach (string f in found)
                    {
                        FileInfo fi = new FileInfo(f);
                        if (fi.Length != int.Parse(textBox3.Text)) { remove.Add(f); };
                    }
                }
            }

            foreach (string f in remove)
            {
                found.RemoveAt(found.IndexOf(f));
            }

            foreach (string f in found)
            {
                addFile(Path.GetFileName(f), getSize(f).ToString(),"11",f);
            }

        }
    }
}

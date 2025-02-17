using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Coursework
{
    public partial class Form1 : Form
    {
        private Editor _editor;


        public Form1()  // ����������� �����
        {
            InitializeComponent();
            _editor = new Editor(tabControl1);  // ��������� ��� ���������� ���������
            AddNewTab();    // ������� ����� ������� �� ���������
            this.FormClosing += Form1_FormClosing;
        }

        private void AddNewTab(string title = "No name")    // ����� ��� �������� ����� �������, �������� �������� - �������� ���-��
        {
            // ������� ������ ���������, � ������� ����� ������� ������ �� �������
            var doc = new Document();

            // ������� ������ ������� (TabPage), � ������� �������� ��������� �������
            var tabPage = new TabPage();

            // ������� �������� � ��� �������� ����� ��� �������� Tag, ����� ����� ���� �������� ��� �������
            tabPage.Tag = doc;

            // ������� ������� RichTextBox ��� �������������� ������
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,  // ���������� �������� Doc ����� ������������ �������
                Font = new Font("Consolas", 12)                        // ������ ����� � ������
            };

            // ������� RichTextBox � ����������, ����� �� ��� �������� � �������� ��� ����������
            doc.TextBox = richTextBox;

            // �������� RichTextBox � TabPage
            tabPage.Controls.Add(richTextBox);

            // �������� TabPage � ControlTab
            tabControl1.TabPages.Add(tabPage);

            // ����������� ControlTab � ����� ������� ����� �� ��������
            tabControl1.SelectedTab = tabPage;

        }




        /*����� Document ������������ ����� ������ ���������� ��������� � ����������, ������������ ���������������� ��� ��������, �������������� � ���������� ������*/
        public class Document
        {
            public string Name { get; set; }     // ������ ��� ����� (������� ����) ������������ ��� ������������, ���� ��������� ��� ������ ��������� ����
            public string ShortName => Path.GetFileName(Name);  // ������������� ������������� ��������. ���������� �������� ��� ����� 
            public bool HasName => !string.IsNullOrEmpty(Name);     // ��������, ������ �� ��� �����. ���������� true, ���� Name �� ������ � �� null
            public bool Modified { get; set; }  // ���� ���������. ������������ ��� �������������� ������������ � ������������� ���������� ����� ��������� ���������

            public RichTextBox TextBox { get; set; }    // ����������� ������� ���������� (RichTextBox), ��������������� ��� ����������� � �������������� ������ ���������

            public Document()
            /*�������������� ��������� RichTextBox � ������������� ��� �������� Dock ��� ��������������� ���������� ���������� ������������ � ������������ �������� ����������.
            ��� ������������ ���������� ����������� ���������� ���� � ���������������� ����������*/
            {
                TextBox = new RichTextBox { Dock = DockStyle.Fill };
            }

            public void Open(string fileName)
            {
                Name = fileName;    // ��������� ���� � ����� � ��������� ��� ���������� � TextBox
                TextBox.Text = File.ReadAllText(fileName);
                Modified = false;   // ������������� ���� Modified � false, ��� ��� �������� ������ ��� ������ � ��������� ���
            }

            public void Save()
            {
                if (HasName)    // ��������� ���������� ���������� ���� � ����, ���� ��� ����� ������
                {
                    File.WriteAllText(Name, TextBox.Text);
                    Modified = false;   // ����� ���������� ������������� Modified � false
                }
            }

            public void SaveAs(string fileName)
            {
                Name = fileName;    // ��������� ��������� �������� ��� ����� ������
                Save();
            }
        }



        public class RecentList
        {
            private const int MaxRecentFiles = 5;
            private List<string> _files = new List<string>();

            public void Add(string fileName)
            {
                if (_files.Contains(fileName))  // ���� ���� ��� ������������, ������� ��� �� ������ (����� ����������� ��� � ������)
                {
                    _files.Remove(fileName);    // ��������� ����� ���� � ������ ������
                }
                _files.Insert(0, fileName);     // ��������� ����� ���� � ������ ������ 

                // ���� ���������� ������ ��������� MaxRecentFiles, ������� ��������� ���� �� ������
                if (_files.Count > MaxRecentFiles)
                {
                    _files.RemoveAt(MaxRecentFiles);
                }
            }

            public void SaveData()  // ��������� ������� ������ ������ � ��������� ���� recent.txt
            {
                // ����������� ������ ������ � ���� ������, �������� �������� �������� ����� ������
                string fileContent = string.Join(Environment.NewLine, _files);

                File.WriteAllText("recent.txt", fileContent);    // ��������� ������ � ����
            }
            public void LoadData()
            {
                // ��������� ������ ������ �� ����� recent.txt, ���� ���� ���������� 
                if (File.Exists("recent.txt"))
                    _files = File.ReadAllLines("recent.txt").ToList(); // ����������� ������ ����� � ������ � ������� ToList()
            }

            public List<string> GetFiles() => _files;   // ���������� ������� ������ ������

        }




        public class Editor
        {
            // ������� ����������, ������� �������� ������� (TabPage) ��� ������� ��������� ���������
            private TabControl _tabControl;

            // ��������� ������ RecentList, ������������ ��� ���������� ������� ������� �������� ������
            private RecentList _recentList = new RecentList();

            public RecentList RecentList => _recentList;


            /*����������� ��������� TabControl, ������� ����������� � ����������� ���������. 
             ��� ������������� ����������� ������ ��������� �������� ������ ����� LoadData*/
            public Editor(TabControl tabControl)
            {
                _tabControl = tabControl;
                _recentList.LoadData();
            }

            public void NewDoc()
            {
                var doc = new Document();
                var tabPage = new TabPage("No name");    // ������ ����� �������� ��� �����
                tabPage.Tag = doc;
                tabPage.Controls.Add(doc.TextBox);  // ��������� ��� �� ����� ������� (TabPage) � ��������� ����� TextBox
                _tabControl.TabPages.Add(tabPage);
                _tabControl.SelectedTab = tabPage;
            }

            public void OpenDoc(string fileName)
            {
                if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                    MessageBox.Show("File not found: " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (DocOpened(fileName))
                {
                    return; // ���� ���� ��� ������, ������ ������������� �� �������
                }

                var doc = new Document();
                doc.Open(fileName);

                var tabPage = new TabPage(doc.ShortName);
                tabPage.Tag = doc;
                tabPage.Controls.Add(doc.TextBox);
                _tabControl.TabPages.Add(tabPage);
                _tabControl.SelectedTab = tabPage;

                _recentList.Add(fileName); // ��������� � ������ ��������� ������
                _recentList.SaveData(); // ��������� ��� � ����
            }


            public void SaveDoc()
            {
                var doc = GetActiveDocument();
                if (doc != null)    // ��������� �������� ��������, ���� �� ����������
                {

                    //���� ��� ����� �� ������, �������� ����� SaveDocAs() ��� ������ ����
                    if (doc.HasName)
                    {
                        doc.Save();
                    }
                    else
                    {
                        SaveDocAs();
                    }
                }
            }


            public void SaveDocAs()
            {
                var doc = GetActiveDocument();
                if (doc != null)
                {
                    var saveFileDialog = new SaveFileDialog();  // ��������� ���������� ���� ��� ������ ���� ����������
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // ��������� ��������� ������� � ��������� ���� � ����� � ������ ��������� ������
                        doc.SaveAs(saveFileDialog.FileName);
                        _tabControl.SelectedTab.Text = doc.ShortName;
                        _recentList.Add(doc.Name);
                    }
                }
            }

            public void CloseActiveDoc()
            {
                var doc = GetActiveDocument();
                if (doc != null)
                {
                    // ���� �������� ��� ������ ��� �� ����� ����� (��� �� �������), ���������� ���������
                    if (doc.Modified || !doc.HasName)
                    {
                        var result = MessageBox.Show("Save changes?", "Warning", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            if (!doc.HasName)
                            {
                                SaveDocAs();  // ���� ���� �� ����� �����, �������� "Save As"
                            }
                            else
                            {
                                SaveDoc();
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return; // ������ �������� ���������
                        }
                    }
                    _tabControl.TabPages.Remove(_tabControl.SelectedTab); // ������� �������
                }
            }


            private Document GetActiveDocument()    // ���������� �������� �������� ����� �������� ������� � ��������� ����� �������� Tag.
            {
                if (_tabControl.SelectedTab != null && _tabControl.SelectedTab.Tag is Document doc)
                    return doc;
                return null;
            }

            public bool DocOpened(string fileName)
            {
                // ���������� true, ���� ������� � ����� ������ ��� ����������
                return _tabControl.TabPages.Cast<TabPage>().Any(tab => tab.Text == Path.GetFileName(fileName));
            }
            public void AddTabPage(Document doc, TabPage tabPage)
            {
                tabPage.Tag = doc;   // ��������� �������� � �������� ����� �������� Tag
                tabPage.Controls.Add(doc.TextBox);  // ��������� ��������� ���� � �������
                _tabControl.TabPages.Add(tabPage);  // ��������� ������� � TabControl
                _tabControl.SelectedTab = tabPage;  // ������������� �� ����� �������
            }

        }




        private void OpenRecentFile(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                string fileName = menuItem.Text;
                if (File.Exists(fileName))
                {
                    _editor.OpenDoc(fileName);
                }
                else
                {
                    MessageBox.Show("File not found: " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool hasUnsavedChanges = false;

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage.Tag is Document doc && (doc.Modified || !doc.HasName))
                {
                    hasUnsavedChanges = true;
                    var result = MessageBox.Show("You have unsaved changes. Do you want to save them before exiting?",
                                                 "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        if (!doc.HasName)
                        {
                            var saveFileDialog = new SaveFileDialog();
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                doc.SaveAs(saveFileDialog.FileName);
                                tabPage.Text = doc.ShortName;
                                _editor.RecentList.Add(doc.Name);
                            }
                            else
                            {
                                e.Cancel = true; // ������ ��������, ���� ������������ ��������� �� ����������
                                return;
                            }
                        }
                        else
                        {
                            doc.Save();
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true; // ������ ��������
                        return;
                    }
                }
            }
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _editor.NewDoc();
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                openFileDialog.Title = "Open File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    Console.WriteLine("Selected file: " + fileName);  // ������� ��� ���������� ����� � �������

                    _editor.OpenDoc(fileName);  // �������� ����� ��� �������� �����
                }
            }
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _editor.SaveDoc();
        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _editor.SaveDocAs();
        }

        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _editor.CloseActiveDoc();
        }

        private void recentToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            var recentFiles = _editor.RecentList.GetFiles();

            foreach (var file in recentFiles)
            {
                if (File.Exists(file))
                {
                    var menuItem = new ToolStripMenuItem(file);
                    menuItem.Click += OpenRecentFile; // ������ ������������ ������������ �����
                    recentToolStripMenuItem.DropDownItems.Add(menuItem);
                }
            }
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            bool hasUnsavedChanges = false;

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage.Tag is Document doc && (doc.Modified || !doc.HasName))
                {
                    hasUnsavedChanges = true;
                    var result = MessageBox.Show("You have unsaved changes. Do you want to save them before exiting?",
                                                 "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        if (!doc.HasName) // ���� � ��������� ��� �����, �������� "Save As"
                        {
                            var saveFileDialog = new SaveFileDialog();
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                doc.SaveAs(saveFileDialog.FileName);
                                tabPage.Text = doc.ShortName;
                                _editor.RecentList.Add(doc.Name);
                            }
                            else
                            {
                                return; // ���� ������������ ��������� �� "Save As", �� �������
                            }
                        }
                        else
                        {
                            doc.Save();
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return; // ���� ������������ ����� "Cancel", �� �������
                    }
                }
            }

            Application.Exit();
        }
    }
}
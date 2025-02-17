using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Coursework
{
    public partial class Form1 : Form
    {
        private Editor _editor;


        public Form1()  // Конструктор формы
        {
            InitializeComponent();
            _editor = new Editor(tabControl1);  // Экземпляр для управления вкладками
            AddNewTab();    // Создаем новую вкладку по умолчанию
            this.FormClosing += Form1_FormClosing;
        }

        private void AddNewTab(string title = "No name")    // Метод для создания новой вкладки, содержит параметр - название док-та
        {
            // Создаем объект документа, в котором будем хранить данные со вкладки
            var doc = new Document();

            // Создать объект вкладки (TabPage), в которую помещаем заголовок вкладки
            var tabPage = new TabPage();

            // Связали документ с его вкладкой через его свойство Tag, чтобы можно было получать его обратно
            tabPage.Tag = doc;

            // Создали полотно RichTextBox для редактирования текста
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,  // Установили привязку Doc всему пространству вкладки
                Font = new Font("Consolas", 12)                        // Задали шрифт и размер
            };

            // Связали RichTextBox с документом, чтобы он мог получать и изменять его содержимое
            doc.TextBox = richTextBox;

            // Добавили RichTextBox в TabPage
            tabPage.Controls.Add(richTextBox);

            // Добавили TabPage в ControlTab
            tabControl1.TabPages.Add(tabPage);

            // Переключили ControlTab в новую вкладку после ее создания
            tabControl1.SelectedTab = tabPage;

        }




        /*Класс Document представляет собой модель текстового документа в приложении, предоставляя функциональность для открытия, редактирования и сохранения файлов*/
        public class Document
        {
            public string Name { get; set; }     // Полное имя файла (включая путь) Используется для отслеживания, куда сохранять или откуда открывать файл
            public string ShortName => Path.GetFileName(Name);  // Автоматически резервируемое свойство. Возвращает короткое имя файла 
            public bool HasName => !string.IsNullOrEmpty(Name);     // Проверка, задано ли имя файла. Возвращает true, если Name не пустое и не null
            public bool Modified { get; set; }  // Флаг изменений. Используется для предупреждения пользователя о необходимости сохранения перед закрытием документа

            public RichTextBox TextBox { get; set; }    // Графический элемент управления (RichTextBox), предназначенный для отображения и редактирования текста документа

            public Document()
            /*Инициализирует экземпляр RichTextBox и устанавливает его свойство Dock для автоматического заполнения доступного пространства в родительском элементе интерфейса.
            Это обеспечивает корректное отображение текстового поля в пользовательском интерфейсе*/
            {
                TextBox = new RichTextBox { Dock = DockStyle.Fill };
            }

            public void Open(string fileName)
            {
                Name = fileName;    // Принимает путь к файлу и загружает его содержимое в TextBox
                TextBox.Text = File.ReadAllText(fileName);
                Modified = false;   // Устанавливает флаг Modified в false, так как документ только что открыт и изменений нет
            }

            public void Save()
            {
                if (HasName)    // Сохраняет содержимое текстового поля в файл, если имя файла задано
                {
                    File.WriteAllText(Name, TextBox.Text);
                    Modified = false;   // После сохранения устанавливает Modified в false
                }
            }

            public void SaveAs(string fileName)
            {
                Name = fileName;    // Позволяет сохранить документ под новым именем
                Save();
            }
        }



        public class RecentList
        {
            private const int MaxRecentFiles = 5;
            private List<string> _files = new List<string>();

            public void Add(string fileName)
            {
                if (_files.Contains(fileName))  // Если файл уже присутствует, удаляет его из списка (чтобы переместить его в начало)
                {
                    _files.Remove(fileName);    // Вставляет новый файл в начало списка
                }
                _files.Insert(0, fileName);     // Вставляет новый файл в начало списка 

                // Если количество файлов превышает MaxRecentFiles, удаляет последний файл из списка
                if (_files.Count > MaxRecentFiles)
                {
                    _files.RemoveAt(MaxRecentFiles);
                }
            }

            public void SaveData()  // Сохраняет текущий список файлов в текстовый файл recent.txt
            {
                // Преобразуем список файлов в одну строку, разделяя элементы символом новой строки
                string fileContent = string.Join(Environment.NewLine, _files);

                File.WriteAllText("recent.txt", fileContent);    // Сохраняем строку в файл
            }
            public void LoadData()
            {
                // Загружает список файлов из файла recent.txt, если файл существует 
                if (File.Exists("recent.txt"))
                    _files = File.ReadAllLines("recent.txt").ToList(); // Преобразует массив строк в список с помощью ToList()
            }

            public List<string> GetFiles() => _files;   // Возвращает текущий список файлов

        }




        public class Editor
        {
            // элемент управления, который содержит вкладки (TabPage) для каждого открытого документа
            private TabControl _tabControl;

            // экземпляр класса RecentList, используемый для управления списком недавно открытых файлов
            private RecentList _recentList = new RecentList();

            public RecentList RecentList => _recentList;


            /*конструктор принимает TabControl, который связывается с интерфейсом редактора. 
             При инициализации загружается список последних открытых файлов через LoadData*/
            public Editor(TabControl tabControl)
            {
                _tabControl = tabControl;
                _recentList.LoadData();
            }

            public void NewDoc()
            {
                var doc = new Document();
                var tabPage = new TabPage("No name");    // Создаёт новый документ без имени
                tabPage.Tag = doc;
                tabPage.Controls.Add(doc.TextBox);  // Добавляет его на новую вкладку (TabPage) с текстовым полем TextBox
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
                    return; // Если файл уже открыт, просто переключаемся на вкладку
                }

                var doc = new Document();
                doc.Open(fileName);

                var tabPage = new TabPage(doc.ShortName);
                tabPage.Tag = doc;
                tabPage.Controls.Add(doc.TextBox);
                _tabControl.TabPages.Add(tabPage);
                _tabControl.SelectedTab = tabPage;

                _recentList.Add(fileName); // Добавляем в список последних файлов
                _recentList.SaveData(); // Сохраняем его в файл
            }


            public void SaveDoc()
            {
                var doc = GetActiveDocument();
                if (doc != null)    // Сохраняет активный документ, если он существует
                {

                    //Если имя файла не задано, вызывает метод SaveDocAs() для выбора пути
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
                    var saveFileDialog = new SaveFileDialog();  // Открывает диалоговое окно для выбора пути сохранения
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Обновляет заголовок вкладки и сохраняет путь к файлу в списке последних файлов
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
                    // Если документ был изменён или не имеет имени (ещё не сохранён), предлагаем сохранить
                    if (doc.Modified || !doc.HasName)
                    {
                        var result = MessageBox.Show("Save changes?", "Warning", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            if (!doc.HasName)
                            {
                                SaveDocAs();  // Если файл не имеет имени, вызываем "Save As"
                            }
                            else
                            {
                                SaveDoc();
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return; // Отмена закрытия документа
                        }
                    }
                    _tabControl.TabPages.Remove(_tabControl.SelectedTab); // Удаляем вкладку
                }
            }


            private Document GetActiveDocument()    // Возвращает активный документ через привязку вкладки и документа через свойство Tag.
            {
                if (_tabControl.SelectedTab != null && _tabControl.SelectedTab.Tag is Document doc)
                    return doc;
                return null;
            }

            public bool DocOpened(string fileName)
            {
                // Возвращает true, если вкладка с таким именем уже существует
                return _tabControl.TabPages.Cast<TabPage>().Any(tab => tab.Text == Path.GetFileName(fileName));
            }
            public void AddTabPage(Document doc, TabPage tabPage)
            {
                tabPage.Tag = doc;   // Связываем документ с вкладкой через свойство Tag
                tabPage.Controls.Add(doc.TextBox);  // Добавляем текстовое поле в вкладку
                _tabControl.TabPages.Add(tabPage);  // Добавляем вкладку в TabControl
                _tabControl.SelectedTab = tabPage;  // Переключаемся на новую вкладку
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
                                e.Cancel = true; // Отмена закрытия, если пользователь отказался от сохранения
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
                        e.Cancel = true; // Отмена закрытия
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
                    Console.WriteLine("Selected file: " + fileName);  // Выводим имя выбранного файла в консоль

                    _editor.OpenDoc(fileName);  // Вызываем метод для открытия файла
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
                    menuItem.Click += OpenRecentFile; // Теперь используется исправленный метод
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
                        if (!doc.HasName) // Если у документа нет имени, вызываем "Save As"
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
                                return; // Если пользователь отказался от "Save As", не выходим
                            }
                        }
                        else
                        {
                            doc.Save();
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return; // Если пользователь нажал "Cancel", не выходим
                    }
                }
            }

            Application.Exit();
        }
    }
}
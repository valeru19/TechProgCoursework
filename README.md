# Курсовая работа по технологиям программирования "Текстовый редактор" с подробным описанием



# Класс Document 
представляет собой модель текстового документа в приложении, предоставляя функциональность для открытия, редактирования и сохранения файлов

Конструктор:

public Document()
{
    TextBox = new RichTextBox { Dock = DockStyle.Fill };
}

Инициализирует экземпляр RichTextBox и устанавливает его свойство Dock для автоматического заполнения доступного пространства в родительском элементе интерфейса

Взаимосвязь элементов:

Свойство Name тесно связано с методами Open, Save, и SaveAs, так как оно определяет путь к файлу и его свойства.
Элемент управления TextBox является контейнером для текста документа. Его содержимое загружается из файла при вызове Open() и сохраняется в файл при вызове Save().
Флаг Modified контролирует состояние документа и используется для отслеживания необходимости сохранения.


# Класс RecentList 
реализует список недавно открытых файлов с функциональностью добавления, сохранения и загрузки данных из текстового файла recent.txt. Это полезно для приложений, которые должны запоминать историю работы с файлами

Взаимосвязь элементов:

Методы Add, SaveData, и LoadData работают совместно для управления списком файлов и его синхронизации с файлом recent.txt.
Константа MaxRecentFiles контролирует размер списка.
Поле _files используется как основной контейнер для данных.



# Класс Editor
реализует логику работы текстового редактора, предоставляя функции для работы с вкладками, открытия, создания, сохранения и закрытия документов. Давай подробно разберём все ключевые элементы

Взаимосвязь элементов:

_tabControl обеспечивает возможность работы с несколькими документами через вкладки.
Методы документа Open(), Save(), и SaveAs() тесно связаны с методами Editor, которые управляют интерфейсом пользователя.
_recentList сохраняет и загружает историю открытых файлов.
Взаимодействие между документами и вкладками:
Каждая вкладка отображает текстовое поле (RichTextBox) документа.
![image](https://github.com/user-attachments/assets/6ee4fc57-5cac-46af-8210-603862a59b0c)

classDiagram
    class Form1 {
        -Editor _editor
        +Form1()
        +AddNewTab(title: string)
        -OpenRecentFile(sender, e)
        -Form1_FormClosing(sender, e)
        -newToolStripMenuItem_Click()
        -openToolStripMenuItem_Click()
        -saveToolStripMenuItem_Click()
        -saveAsToolStripMenuItem_Click()
        -closeToolStripMenuItem_Click()
        -recentToolStripMenuItem_Click()
        -exitToolStripMenuItem_Click()
        -backgroundSettingsToolStripMenuItem_Click()
        -fontSettingsToolStripMenuItem_Click()
    }

    class Editor {
        -TabControl _tabControl
        -RecentList _recentList
        +RecentList RecentList
        +Editor(tabControl)
        +NewDoc()
        +OpenDoc(fileName)
        +SaveDoc()
        +SaveDocAs()
        +CloseActiveDoc()
        -GetActiveDocument() Document
        -DocOpened(fileName) bool
        +AddTabPage(doc, tabPage)
    }

    class Document {
        +string Name
        +string ShortName
        +bool HasName
        +bool Modified
        +RichTextBox TextBox
        +Document()
        +Open(fileName)
        +Save()
        +SaveAs(fileName)
    }

    class RecentList {
        -const MaxRecentFiles
        -List~string~ _files
        +Add(fileName)
        +SaveData()
        +LoadData()
        +GetFiles() List~string~
    }

    class RichTextBox {
        +Dock DockStyle
        +Font Font
        +Text string
    }

    Form1 --> Editor : содержит
    Editor --> Document : управляет
    Editor --> RecentList : использует
    Document --> RichTextBox : содержит

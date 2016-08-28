using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MuzipCompress
{
    public partial class Form1 : Form
    {
        private Timer drag_panels_timer;
        private Label _labelWord, word_length;
        private MuzipButton exit_button, minimize_button, target_file_button, source_file_button, target_path_button,
            muzip_button, unzip_button, word_down_button, word_up_button;
        private MuzipFormTitle form_title;
        private MuzipTabs tabs;
        private Panel panelCompress, panelUnCompress, panelOptions;
        private ListView list_compress_files, list_uncompress_files;
        private ImageList list_file_icons;
        private List<string> files_list_compression;
        private TextBox text_box_target_file, text_box_source_file, text_box_target_path;
        private int word_length_selection;

        public Form1()
        {
            InitializeComponent();
            word_length_selection = 4;
            drag_panels_timer = new Timer() { Interval = 25, Enabled = false }; drag_panels_timer.Tick += drag_panels_timer_Tick;

            form_title = new MuzipFormTitle() { Width = 720, Height = 30, ButtonText = "Muzip", Top = 0, Left = 0,
                Color1 = Color.FromArgb(255, 220, 220, 240),
                Color2 = Color.FromArgb(255, 120, 120, 240),
            };
            exit_button = new MuzipButton() { Width = 28, Height = 28, ButtonText = "X",
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            exit_button.Click += exit_button_Click;
            minimize_button = new MuzipButton() { Width = 28, Height = 28, ButtonText = "-",
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            minimize_button.Click += minimize_button_Click;
            tabs = new MuzipTabs() { Width = 720, Height = 30, Left = 0, Top = 40,
                Color1 = Color.FromArgb(255, 228, 228, 255),
                Color2 = Color.FromArgb(255, 170, 170, 255),
                FocusedColor1 = Color.FromArgb(255, 180, 180, 255),
                FocusedColor2 = Color.FromArgb(255, 120, 120, 255)
            };
            tabs.TabSelected += tabs_TabSelected;
            form_title.Controls.Add(exit_button);
            form_title.Controls.Add(minimize_button);

            panelCompress = new Panel() {  Width = 700, Height = 520,
                Top = 70, Left = 10, BorderStyle = BorderStyle.FixedSingle };
            panelUnCompress = new Panel() { Width = 700, Height = 520,
                Top = 70, Left = 730, BorderStyle = BorderStyle.FixedSingle };
            panelOptions = new Panel() { Width = 700, Height = 520,
                Top = 70, Left = 1450, BorderStyle = BorderStyle.FixedSingle };

            //Compression Panel
            files_list_compression = new List<string>();
            list_file_icons = new ImageList() { ImageSize = new Size(32, 32) };
            list_file_icons.Images.Add("directory", Properties.Resources.directory_ico);
            list_compress_files = new ListView()
            {
                Width = 680, Height = 420,
                Top = 10, Left = 10, BorderStyle = BorderStyle.FixedSingle, AllowDrop = true, 
                Font = new Font("Arial", 10f),
                SmallImageList = list_file_icons, LargeImageList = list_file_icons };
            list_compress_files.DragDrop += list_compress_files_DragDrop;
            list_compress_files.DragEnter += list_compress_files_DragEnter;
            list_compress_files.PreviewKeyDown += list_compress_files_PreviewKeyDown;
            text_box_target_file = new TextBox() { Width = 500, Top = 440, Left = 10, BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Arial", 10f), ReadOnly = true, BackColor = Color.White };
            target_file_button = new MuzipButton() { Width = 120, Height = text_box_target_file.Height, ButtonText = "Hedef Dosya",
                ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            muzip_button = new MuzipButton()
            {
                Width = 120,
                Height = text_box_target_file.Height,
                ButtonText = "Sıkıştır",
                ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            target_file_button.Click += target_file_button_Click;
            muzip_button.Click += muzip_button_Click;

            panelCompress.Controls.Add(list_compress_files);
            panelCompress.Controls.Add(text_box_target_file);
            panelCompress.Controls.Add(target_file_button);
            panelCompress.Controls.Add(muzip_button);

            //UnCompression Panel
            text_box_source_file = new TextBox()
            {
                Width = 500, Top = 10, Left = 10, BorderStyle = BorderStyle.FixedSingle, ReadOnly = true, BackColor = Color.White,
                Font = new Font("Arial", 10f)
            };
            source_file_button = new MuzipButton()
            {
                Width = 120, Height = text_box_target_file.Height,
                ButtonText = "Kaynak Dosya", ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            list_uncompress_files = new ListView() { Width = 680, Height = 390,
                Top = 40, Left = 10, BorderStyle = BorderStyle.FixedSingle, AllowDrop = true, 
                Font = new Font("Arial", 10f),
                SmallImageList = list_file_icons, LargeImageList = list_file_icons };
            text_box_target_path = new TextBox()
            {
                Width = 500, Top = 440, Left = 10,
                BorderStyle = BorderStyle.FixedSingle, ReadOnly = true, BackColor = Color.White,
                Font = new Font("Arial", 10f)
            };
            target_path_button = new MuzipButton()
            {
                Width = 120, Height = text_box_target_path.Height, ButtonText = "Hedef Yol",
                ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            unzip_button = new MuzipButton()
            {
                Width = 120,
                Height = text_box_target_file.Height,
                ButtonText = "Aç",
                ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            source_file_button.Click += source_file_button_Click;
            target_path_button.Click += target_path_button_Click;
            unzip_button.Click += unzip_button_Click;
            panelUnCompress.Controls.Add(text_box_source_file);
            panelUnCompress.Controls.Add(source_file_button);
            panelUnCompress.Controls.Add(list_uncompress_files);
            panelUnCompress.Controls.Add(text_box_target_path);
            panelUnCompress.Controls.Add(target_path_button);
            panelUnCompress.Controls.Add(unzip_button);

            //Options Panel
            _labelWord = new Label()
            {
                Top = 10, Left = 10, AutoSize = true,
                Font = new Font("Arial", 10f),
                Text = "Kelime Uzunluğu:",
                BackColor = Color.Transparent,
            };
            word_down_button = new MuzipButton()
            {
                Width = 30, Height = _labelWord.Height,
                ButtonFont = new Font("Arial", 10f),
                ButtonText = "<",
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            word_length = new Label() 
            { 
                AutoSize = true,
                Font = new Font("Arial", 10f),
                Text = "8 kb",
                BackColor = Color.Transparent
            };
            word_up_button = new MuzipButton()
            {
                Width = 30,
                Height = _labelWord.Height,
                ButtonFont = new Font("Arial", 10f),
                ButtonText = ">",
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            Label _labelHuffmanLevel = new Label()
            {
                Top = 50, Left = 10, AutoSize = true,
                Font = new Font("Arial", 10f),
                Text = "Huffman Metodu Seviyesi: ",
                BackColor = Color.Transparent
            };
            TrackBar track_huffman = new TrackBar()
            {
                Top = 50, Left = 250,
                Minimum = 1, Maximum = 3
            };
            Label _labelLZWLevel = new Label()
            {
                Top = 100, Left = 10, AutoSize = true,
                Font = new Font("Arial", 10f),
                Text = "LZW Metodu Seviyesi: ",
                BackColor = Color.Transparent
            };
            TrackBar track_lzw = new TrackBar()
            {
                Top = 100, Left = 250,
                Minimum = 1, Maximum = 3
            };
            Label _labelRLLevel = new Label()
            {
                Top = 150,
                Left = 10,
                AutoSize = true,
                Font = new Font("Arial", 10f),
                Text = "Run-Length Metodu Seviyesi: ",
                BackColor = Color.Transparent
            };
            TrackBar track_rl = new TrackBar()
            {
                Top = 150, Left = 250,
                Minimum = 1, Maximum = 3
            };

            word_down_button.Click += word_down_button_Click;
            word_up_button.Click += word_up_button_Click;
            panelOptions.Controls.Add(_labelWord);
            panelOptions.Controls.Add(word_down_button);
            panelOptions.Controls.Add(word_length);
            panelOptions.Controls.Add(word_up_button);
            panelOptions.Controls.Add(_labelHuffmanLevel);
            panelOptions.Controls.Add(track_huffman);
            panelOptions.Controls.Add(_labelLZWLevel);
            panelOptions.Controls.Add(track_lzw);
            panelOptions.Controls.Add(_labelRLLevel);
            panelOptions.Controls.Add(track_rl);

            this.Controls.Add(form_title);
            this.Controls.Add(tabs);
            this.Controls.Add(panelCompress);
            this.Controls.Add(panelUnCompress);
            this.Controls.Add(panelOptions);
        }

        void list_compress_files_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int i = 0, c = list_compress_files.SelectedItems.Count;
                while (list_compress_files.SelectedItems.Count > 0)
                {
                    files_list_compression.RemoveAt(list_compress_files.SelectedItems[0].Index);
                    list_compress_files.Items.Remove(list_compress_files.SelectedItems[0]);
                }
            }
        }

        void word_up_button_Click(object sender, EventArgs e)
        {
            if (word_length_selection < 6) word_length_selection++;
            WordLabel();
        }

        void word_down_button_Click(object sender, EventArgs e)
        {
            if (word_length_selection > 0) word_length_selection--;
            WordLabel();
        }

        private void WordLabel()
        {
            switch (word_length_selection)
            {
                case 0: word_length.Text = "512 byte"; break;
                case 1: word_length.Text = "1 kb"; break;
                case 2: word_length.Text = "2 kb"; break;
                case 3: word_length.Text = "4 kb"; break;
                case 4: word_length.Text = "8 kb"; break;
                case 5: word_length.Text = "16 kb"; break;
                case 6: word_length.Text = "32 kb"; break;
            }
        }

        void unzip_button_Click(object sender, EventArgs e)
        {
            if (text_box_source_file.Text != "" && text_box_target_path.Text != "")
            {
                MuzipSystem muzip_work = new MuzipSystem();
                muzip_work.TargetPath = text_box_target_path.Text;
                FormProcess form_process = new FormProcess();
                form_process.UnCompressSource = text_box_source_file.Text;
                form_process.ProcessType = FormProcess.ProcessTypes.Unzip;
                form_process.MuzipWork = muzip_work;
                if (form_process.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    text_box_source_file.Text = "";
                    text_box_target_path.Text = "";
                }
            }
        }

        void target_path_button_Click(object sender, EventArgs e)
        {
            if (text_box_source_file.Text == "") return;
            FolderBrowserDialog folder_dialog = new FolderBrowserDialog();
            if (folder_dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(text_box_source_file.Text);
                string new_folder = fi.Name.Replace(fi.Extension, "");
                text_box_target_path.Text = folder_dialog.SelectedPath + "\\" + new_folder;
            }
        }

        void source_file_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog();
            open_dialog.Filter = "Muzip Arşivi (*.muzip)|*.muzip";
            open_dialog.Multiselect = false;
            if (open_dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                text_box_source_file.Text = open_dialog.FileName;
                sys_for_file_log_show_files(text_box_source_file.Text);
            }
        }

        void sys_for_file_log_show_files(string source) { }

        void muzip_button_Click(object sender, EventArgs e)
        {
            if (text_box_target_file.Text != "" && files_list_compression.Count > 0)
            {
                MuzipSystem muzip_work = new MuzipSystem(); muzip_work.BlockSize = word_length_selection;
                foreach (string path in files_list_compression) muzip_work.AddPath(path);
                muzip_work.TargetFile = text_box_target_file.Text;
                FormProcess form_process = new FormProcess();
                form_process.ProcessType = FormProcess.ProcessTypes.Muzip;
                form_process.MuzipWork = muzip_work;
                if (form_process.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    text_box_target_file.Text = "";
                    files_list_compression.Clear();
                    List_Compress_Items();
                }
            }
        }

        void drag_panels_timer_Tick(object sender, EventArgs e)
        {
            int target_left = (0 - tabs.SelectedTab) * 720 + 10;
            panelCompress.Left += (target_left - panelCompress.Left) / 5;
            if (panelCompress.Left > target_left - 10 && panelCompress.Left < target_left + 10) { panelCompress.Left = target_left; drag_panels_timer.Enabled = false; }
            panelUnCompress.Left = panelCompress.Left + 720;
            panelOptions.Left = panelUnCompress.Left + 720;
        }

        void target_file_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Muzip Arşivi (*.muzip)|*.muzip";
            if (save_dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                text_box_target_file.Text = save_dialog.FileName;
            }
        }

        void list_compress_files_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog();
            open_dialog.Multiselect = true;
            if (open_dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string[] file_list = open_dialog.FileNames;
                foreach (string file_path in file_list) { files_list_compression.Add(file_path); }
                List_Compress_Items();
            }
        }

        void list_compress_files_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void list_compress_files_DragDrop(object sender, DragEventArgs e)
        {
            string[] file_list = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file_path in file_list) { files_list_compression.Add(file_path); }
            List_Compress_Items();
        }

        void tabs_TabSelected(object sender, MuzipTabs.TabSelectedEventArgs e)
        {
            drag_panels_timer.Enabled = true;
        }

        void exit_button_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void List_Compress_Items()
        {
            list_compress_files.Clear();
            foreach (string file_path in files_list_compression)
            {
                System.IO.FileInfo _file = new System.IO.FileInfo(file_path);
                if (_file.Exists)
                {
                    Icon file_icon = new Icon(SystemIcons.WinLogo, new Size(32, 32));
                    file_icon = Icon.ExtractAssociatedIcon(file_path);
                    if (!list_file_icons.Images.ContainsKey(_file.Extension))
                    {
                        file_icon = Icon.ExtractAssociatedIcon(file_path);
                        list_file_icons.Images.Add(_file.Extension, file_icon);
                    }
                    list_compress_files.Items.Add(new ListViewItem(_file.Name, _file.Extension));
                }
                else if (_file.Attributes == System.IO.FileAttributes.Directory)
                {
                    list_compress_files.Items.Add(new ListViewItem(_file.Name, "directory"));
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            exit_button.Left = form_title.Width - exit_button.Width - 2; exit_button.Top = 1;
            minimize_button.Left = exit_button.Left - minimize_button.Width - 2; minimize_button.Top = 1;
            form_title.Left = 0; form_title.Top = 0;
            tabs.AddTab("Sıkıştır");
            tabs.AddTab("Aç");
            tabs.AddTab("Ayarlar");
            panelCompress.Paint += panelCompress_Paint;
            panelUnCompress.Paint += panelUnCompress_Paint;
            panelOptions.Paint += panelOptions_Paint;
            target_file_button.Top = 440; target_file_button.Left = 520;
            source_file_button.Top = 10; source_file_button.Left = 520;
            target_path_button.Top = 440; target_path_button.Left = 520;
            word_down_button.Top = 10; word_down_button.Left = _labelWord.Right;
            word_length.Top = 10; word_length.Left = word_down_button.Right + 30;
            word_up_button.Top = 10; word_up_button.Left = word_length.Right + 30;
            muzip_button.Top = panelCompress.Height - muzip_button.Height - 10;
            muzip_button.Left = (panelCompress.Width - muzip_button.Width) / 2;
            unzip_button.Top = panelUnCompress.Height - unzip_button.Height - 10;
            unzip_button.Left = (panelUnCompress.Width - unzip_button.Width) / 2;
        }

        void panelOptions_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(panelOptions.Width, 0),
                Color.FromArgb(255, 228, 228, 255), Color.FromArgb(255, 170, 170, 255)),
                new Rectangle(0, 0, panelOptions.Width, panelOptions.Height));
        }

        void panelUnCompress_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(panelUnCompress.Width, 0),
                Color.FromArgb(255, 228, 228, 255), Color.FromArgb(255, 170, 170, 255)),
                new Rectangle(0, 0, panelUnCompress.Width, panelUnCompress.Height));
        }

        void panelCompress_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(panelCompress.Width, 0),
                Color.FromArgb(255, 228, 228, 255), Color.FromArgb(255, 170, 170, 255)), 
                new Rectangle(0, 0, panelCompress.Width, panelCompress.Height));
        }
    }
}

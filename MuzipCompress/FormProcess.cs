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
    public partial class FormProcess : Form
    {
        public enum ProcessTypes { Muzip, Unzip };

        public MuzipSystem MuzipWork { get { return muzip_work; } set { muzip_work = value; MuzipWorkEvents(); } }
        public ProcessTypes ProcessType { get; set; }
        public string UnCompressSource { get; set; }
        private MuzipSystem muzip_work;
        private Pen pen_border;
        private BackgroundWorker worker;
        private ProgressBar progress_all_work, progress_just_file;
        private Label status_all_work, status_just_file;
        private MuzipButton cancel_button;
        private MuzipAnimation muzip_animation;
        private DialogResult dialog_result;

        public FormProcess()
        {
            InitializeComponent();
            dialog_result = System.Windows.Forms.DialogResult.Cancel;
            pen_border = new Pen(Color.FromArgb(150, 20, 20, 20), 1f);
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork; worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            status_just_file = new Label() { Top = 160, Left = 10, Font = new Font("Arial", 10f), AutoSize = true };
            progress_just_file = new ProgressBar() { Top = 250, Left = 10, Width = 400, Height = 30, Maximum = 100,
                Style = ProgressBarStyle.Continuous, ForeColor = Color.FromArgb(255, 180, 180, 255) };
            status_all_work = new Label() { Top = 280, Left = 10, Font = new Font("Arial", 10f), AutoSize = true };
            progress_all_work = new ProgressBar() { Top = 370, Left = 10, Width = 400, Height = 30, Maximum = 100,
                Style = ProgressBarStyle.Continuous, ForeColor = Color.FromArgb(255, 180, 180, 255) };
            cancel_button = new MuzipButton()
            {
                Width = 120, Height = 30, ButtonText = "İptal", ButtonFont = new Font("Arial", 10f),
                Color1 = Color.FromArgb(255, 240, 240, 255),
                Color2 = Color.FromArgb(255, 200, 200, 255),
                FocusedColor1 = Color.FromArgb(200, 240, 240, 255),
                FocusedColor2 = Color.FromArgb(200, 200, 200, 255)
            };
            cancel_button.Click += cancel_button_Click;
            muzip_animation = new MuzipAnimation() 
            {
                Top = 10, Left = 82, Width = 256, Height = 128
            };
            this.Controls.Add(status_just_file);
            this.Controls.Add(progress_just_file);
            this.Controls.Add(status_all_work);
            this.Controls.Add(progress_all_work);
            this.Controls.Add(cancel_button);
            this.Controls.Add(muzip_animation);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invalidate();
            cancel_button.Invalidate();
            muzip_animation.StopAnimation();
        }

        void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = dialog_result;
            this.Close();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (ProcessType == ProcessTypes.Muzip) muzip_work.Compress();
            else if (ProcessType == ProcessTypes.Unzip) muzip_work.UnCompress(UnCompressSource);
        }

        private void MuzipWorkEvents()
        {
            muzip_work.Compressing += muzip_work_Compressing;
            muzip_work.CompressingFiles += muzip_work_CompressingFiles;
            muzip_work.CompressingFinished += muzip_work_CompressingFinished;
            muzip_work.PreparingParts += muzip_work_PreparingParts;
            muzip_work.UnCompressing += muzip_work_UnCompressing;
            muzip_work.UnCompressingFiles += muzip_work_UnCompressingFiles;
            muzip_work.UnCompressingFinished += muzip_work_UnCompressingFinished;
        }

        void muzip_work_UnCompressingFinished(object sender, MuzipUnCompressingFinishedEventArgs e)
        {
            InvokeLabelText(status_all_work, String.Format("Açma İşlemi Bitti:\n{0}", e.TargetPath));
            InvokeLabelText(status_just_file, "");
            InvokeProgressValue(progress_all_work, 0);
            InvokeProgressValue(progress_just_file, 0);
            dialog_result = System.Windows.Forms.DialogResult.OK;
            cancel_button.ButtonText = "Tamam";
            if (cancel_button.InvokeRequired) cancel_button.Invoke(new MethodInvoker(() => cancel_button.Invalidate()));
            else cancel_button.Invalidate();
        }

        void muzip_work_UnCompressingFiles(object sender, MuzipUnCompressingFileEventArgs e)
        {
            int prog = (int)(e.MuzipSize * 100L / e.TotalMuzipSize);
            InvokeLabelText(status_all_work, String.Format("Açılan Byte: {0} / {1}\n{2}", e.MuzipSize, e.TotalMuzipSize, e.FileName));
            InvokeProgressValue(progress_all_work, prog);
        }

        void muzip_work_UnCompressing(object sender, UnCompressingEventArgs e)
        {
            int prog = e.Progress * 100 / e.PartsCount;
            InvokeLabelText(status_just_file, String.Format("Açılıyor: {0} / {1}", e.Progress, e.PartsCount));
            InvokeProgressValue(progress_just_file, prog);
        }

        void muzip_work_PreparingParts(object sender, PreparePartsEventArgs e)
        {
            int prog = e.Progress * 100 / e.PartsCount;
            InvokeLabelText(status_just_file, String.Format("Hesaplanıyor: {0} / {1}", e.Progress, e.PartsCount));
            InvokeProgressValue(progress_just_file, prog);
        }

        void muzip_work_CompressingFinished(object sender, MuzipCompressingFinishedEventArgs e)
        {
            InvokeLabelText(status_all_work, String.Format("Sıkıştırma Oranı: % {0}", e.CompressRate));
            InvokeLabelText(status_just_file, "");
            InvokeProgressValue(progress_all_work, 0);
            InvokeProgressValue(progress_just_file, 0);
            dialog_result = System.Windows.Forms.DialogResult.OK;
            cancel_button.ButtonText = "Tamam";
            if (cancel_button.InvokeRequired) cancel_button.Invoke(new MethodInvoker(() => cancel_button.Invalidate()));
            else cancel_button.Invalidate();
        }

        void muzip_work_CompressingFiles(object sender, MuzipCompressingFileEventArgs e)
        {
            int prog = (int)(e.FileSize * 100L / e.TotalSize);
            InvokeLabelText(status_all_work, String.Format("Dosya: {0}\nBytes: {1} / {2}\nMuzip Bytes: {3}", e.FilePath, e.FileSize, e.TotalSize, e.MuzipSize));
            InvokeProgressValue(progress_all_work, prog);
        }

        void muzip_work_Compressing(object sender, CompressingEventArgs e)
        {
            int prog = e.Progress * 100 / e.PartsCount;
            InvokeLabelText(status_just_file, String.Format("Dosya Sıkıştırılıyor: {0} / {1}", e.Progress, e.PartsCount));
            InvokeProgressValue(progress_just_file, prog);
        }

        private void FormProcess_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen_border, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }

        private void FormProcess_Load(object sender, EventArgs e)
        {
            cancel_button.Top = 410; cancel_button.Left = 150;
            worker.RunWorkerAsync();
        }

        private void InvokeProgressValue(ProgressBar progress, int val)
        {
            if (progress.InvokeRequired) progress.Invoke(new MethodInvoker(() => progress.Value = val ));
            else progress.Value = val;
        }

        private void InvokeLabelText(Label label, string text)
        {
            if (label.InvokeRequired) label.Invoke(new MethodInvoker(() => label.Text = text ));
            else label.Text = text;
        }
    }
}

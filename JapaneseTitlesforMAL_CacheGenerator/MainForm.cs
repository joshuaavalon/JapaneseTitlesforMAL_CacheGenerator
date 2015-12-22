using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace JapaneseTitlesforMAL_CacheGenerator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonAnime_Click(object sender, EventArgs e)
        {
            StartDownload(Type.Anime);
        }

        private void buttonManga_Click(object sender, EventArgs e)
        {
            StartDownload(Type.Manga);
        }

        private int ParseNumber()
        {
            var text = textBox1.Text;
            int num;
            var valid = int.TryParse(text, out num);
            if (!valid || num < 1) return -1;
            return num;
        }

        private void StartDownload(Type type)
        {
            var num = ParseNumber();
            if (num == -1) return;
            buttonAnime.Enabled = false;
            buttonManga.Enabled = false;
            backgroundWorker1.RunWorkerAsync(new Tuple<Type, int>(type, num));
        }

        private void SetStatusText(string text)
        {
            UiThread(statusStrip1, () => { toolStripStatusLabel.Text = text; });
        }

        public static void UiThread(Control control, Action code)
        {
            if (control.InvokeRequired)
                control.BeginInvoke(code);
            else
                code.Invoke();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var arg = e.Argument as Tuple<Type, int>;
            if (arg == null) return;
            var downloader = new Downloader(arg.Item1, arg.Item2);
            downloader.OnProgress += SetStatusText;
            downloader.Start();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UiThread(buttonAnime, () => { buttonAnime.Enabled = true; });
            UiThread(buttonManga, () => { buttonManga.Enabled = true; });
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WrapAround.Logic;

namespace WrapAround.LevelEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string CurrentWorkingJson = string.Empty;
        public string CurrentFilePath = string.Empty;
        public List<Rectangle> Blocks = new List<Rectangle>();
        public GameMap InternalMap = null;

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDiag = new OpenFileDialog
            {
                Title = @"Open your level",
                Filter = @"WrapAround Maps (*.wamap)|*.wamap",
                FileName = "Open a WrapAround Map",
                CheckFileExists = true,
                CheckPathExists = true,
            };

            if (openFileDiag.ShowDialog() == DialogResult.OK)
            {
                using var reader = new StreamReader(openFileDiag.FileName);
                CurrentWorkingJson = await reader.ReadToEndAsync();
                CurrentFilePath = openFileDiag.FileName;
            }

        }

        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilePath == string.Empty)
            {
                using var simpleSound = new SoundPlayer(@"C:\Windows\Media\Windows Error.wav");
                simpleSound.Play();
                return;
            }

            using var writer = new StreamWriter(File.Open(CurrentFilePath,FileMode.Open));

            await writer.WriteAsync(CurrentWorkingJson);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveWorkingMapDialog.ShowDialog() == DialogResult.OK)
            {
                using var writer = new StreamWriter(File.Create(saveWorkingMapDialog.FileName));
                CurrentFilePath = saveWorkingMapDialog.FileName;
                await writer.WriteAsync(CurrentWorkingJson);
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Aqua),new Rectangle(new Point(0,0),new Size(30,10)));
            //TODO allow dragable rectangles, convert the canvas of rectangles to a gamemap and bring over serializers
            
        }
    }
}

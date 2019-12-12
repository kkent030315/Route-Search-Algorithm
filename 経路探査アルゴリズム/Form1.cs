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
using TileType = 経路探査アルゴリズム.TileBlock.TileType;

namespace 経路探査アルゴリズム
{
    public partial class Form1 : Form
    {
        public static bool initialized = false;

        public Form1()
        {
            InitializeComponent();
            World.bitmap = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = World.bitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LoggerForm form = new LoggerForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(this.Location.X + this.Width, this.Location.Y);
            form.Show();

            World.CreateContext();
            World.CreateMap();

            //World.AnalyzeMap();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Image = World.GetBitmap();
        }

        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            World.graphics = Graphics.FromImage(World.GetBitmap());
            Random r = new Random();
            while(true)
            {
                World.DrawText(r.Next(0, 1000).ToString(), Brushes.Red, new Vector2(10, 10));
                await Task.Delay(150);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = (int)numericUpDown1.Value;
            int y = (int)numericUpDown2.Value;
            SendUpdateTile(new Vector2(x, y), comboBox1.SelectedIndex);
        }

        private void SendUpdateTile(Vector2 coord, int index)
        {
            switch (index)
            {
                case 0:
                    World.UpdateTile(coord, TileType.Walkable);
                    break;
                case 1:
                    World.UpdateTile(coord, TileType.Wall);
                    break;
                case 2:
                    World.UpdateTile(coord, TileType.StartTile);
                    break;
                case 3:
                    World.UpdateTile(coord, TileType.GoalTile);
                    break;
                case 4:
                    World.UpdateTile(coord, TileType.NullTile);
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = World.GetTileCoordByMouseCoord(new Vector2(e.X, e.Y));
            var attr = World.GetTileAttributeByMouseCoord(pos).ToString();
            label1.Text = "座標: " + pos.x + ", " + pos.y + " | " + attr;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var pos = World.GetTileCoordByMouseCoord(new Vector2(e.X, e.Y));
            SendUpdateTile(pos, comboBox2.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            World.UpdateMap();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "map.txt";
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "テキストファイル(*.txt) | *.txt";
            dialog.Title = "マップを保存";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;

                using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    var datas = World.ExportCurrentMapData();

                    for (int x = 0; x < World.MAX_COORD_X; x++)
                    {
                        for (int y = 0; y < World.MAX_COORD_Y; y++)
                        {
                            writer.WriteLine(Convert.ToInt32(datas[x, y].GetTileType()).ToString());
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "テキストファイル(*.txt) | *.txt";
            dialog.Title = "マップを読込";
            dialog.RestoreDirectory = true;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;
                
                using(StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    var line = string.Empty;
                    TileBlock[,] datas = World.ExportCurrentMapData();
                    List<string> list = new List<string>();

                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }

                    int counter = 0;
                    for (int x = 0; x < World.MAX_COORD_X; x++)
                    {
                        for (int y = 0; y < World.MAX_COORD_Y; y++)
                        {
                            counter++;
                            if (World.IsTileBlockExists(new Vector2(x, y)))
                            {
                                int result = 5;
                                if(int.TryParse(list[counter - 1], out result))
                                {
                                    datas[x, y].SetTileType(int.Parse(list[counter - 1]));
                                }
                                else
                                {
                                    LoggerForm.WriteError(string.Format("Failed to parse data[{0}, {1}] [{2}]", x, y, list[counter - 1]));
                                }
                            }
                            else
                            {
                                LoggerForm.WriteError(string.Format("Failed to load data[{0}, {1}] [{2}]. Tile is not exists.", x, y, list[counter-1]));
                            }
                        }
                    }
                    World.ImportMapData(datas);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            World.AnalyzeMap();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            World.ResetMap();
        }
    }
}

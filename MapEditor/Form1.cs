using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class MapEditor : Form
    {
        Dictionary<TilePiece, Image> tileImages = new Dictionary<TilePiece, Image>();
        TilePiece _selectedPiece;
        TilePiece[][] mapData = new TilePiece[6][];
        Image car;

        public MapEditor()
        {
            InitializeComponent();

            tileImages.Add(TilePiece.Grass, Properties.Resources.grass);
            tileImages.Add(TilePiece.TopLeft, Properties.Resources.tl);
            tileImages.Add(TilePiece.TopRight, Properties.Resources.tr);
            tileImages.Add(TilePiece.BottomLeft, Properties.Resources.bl);
            tileImages.Add(TilePiece.BottomRight, Properties.Resources.br);
            tileImages.Add(TilePiece.Horizontal, Properties.Resources.h);
            tileImages.Add(TilePiece.Vertical, Properties.Resources.v);
            tileImages.Add(TilePiece.Start, Properties.Resources.tl);

            car = Properties.Resources.car;

            foreach (TilePiece piece in tileImages.Keys)
            {
                Button button = new Button();
                button.Tag = piece;
                button.Image = tileImages[piece];
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.Size = new Size(80, 80);
                button.Click += new EventHandler(button_Click);

                flowLayoutPanel1.Controls.Add(button);
            }

            for (int y = 0; y < mapData.Length; y++)
            {
                mapData[y] = new TilePiece[10];
                for (int x = 0; x < mapData[0].Length; x++)
                {
                    mapData[y][x] = TilePiece.Grass;
                }
            }

            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_MouseClick);
            pictureBox1.MouseLeave += new EventHandler(pictureBox1_MouseLeave);

            _selectedPiece = TilePiece.Start;
        }

        void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            gridPoint = new Point(-1, -1);
            pictureBox1.Refresh();
        }

        void button_Click(object sender, EventArgs e)
        {
            _selectedPiece = (TilePiece)((Button)sender).Tag;
        }

        void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            mapData[gridPoint.Y][gridPoint.X] = _selectedPiece;
        }

        private Point gridPoint = new Point(-1, -1);

        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point newPoint = new Point(e.X / 80, e.Y / 80);

            if (newPoint.X != gridPoint.X || newPoint.Y != gridPoint.Y)
            {
                gridPoint = newPoint;
                pictureBox1.Refresh();
            }
        }

        enum TilePiece
        {
            Start = 0,
            Grass = 1,
            Horizontal = 2,
            Vertical = 3,
            TopLeft = 4,
            TopRight = 5,
            BottomLeft = 6,
            BottomRight = 7
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int y = 0; y < mapData.Length;  y++)
            {
                for (int x= 0; x< mapData[0].Length; x++)
                {
                    TilePiece mapPiece = mapData[y][x];

                    if (gridPoint.X == x && gridPoint.Y == y)
                    {
                        mapPiece = _selectedPiece;
                    }

                    Image drawPiece = tileImages[mapPiece];

                    System.Drawing.Imaging.ImageAttributes a = new System.Drawing.Imaging.ImageAttributes();
                    a.SetColorKey(Color.FromArgb(254, 254, 254), Color.White, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                    e.Graphics.DrawImage(drawPiece, new Rectangle(x*80, y*80, 80, 80), 0, 0, drawPiece.Width, drawPiece.Height, GraphicsUnit.Pixel, a);

                    if (mapPiece == TilePiece.Start)
                    {
                        e.Graphics.DrawImage(car, new Rectangle(x * 80 + 40 - car.Width / 2, y * 80 + 40 - car.Height / 2, car.Width, car.Height));
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.xml";
            openFileDialog1.FileName = "";
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                int y = 0;

                XmlDocument doc = new XmlDocument();
                doc.Load(openFileDialog1.FileName);

                string data = doc.SelectSingleNode(@"/XnaContent/Asset").InnerText.Trim();

                foreach (string row in data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int x = 0;
                    foreach (string cell in row.Trim().Split(' '))
                    {
                        if (y < mapData.Length && x < mapData[0].Length)
                        {
                            mapData[y][x] = (TilePiece)Convert.ToInt32(cell);
                        }
                        x++;
                    }
                    y++;
                }
                saveFileDialog1.FileName = openFileDialog1.FileName;
                pictureBox1.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.xml";
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                StringBuilder sb = new StringBuilder();

                foreach (TilePiece[] row in mapData)
                {
                    foreach (TilePiece cell in row)
                    {
                        sb.AppendFormat("{0} ", (int)cell);
                    }
                    sb.AppendLine();
                }

                XmlDocument doc = new XmlDocument();
                XmlElement content = doc.CreateElement("XnaContent");
                XmlElement asset = doc.CreateElement("Asset");
                
                XmlAttribute type = doc.CreateAttribute("Type", "System.String");
                asset.Attributes.Append(type);
                asset.InnerText = sb.ToString();

                content.AppendChild(asset);
                doc.AppendChild(content);

                doc.Save(saveFileDialog1.FileName);
            }
        }
    }
}

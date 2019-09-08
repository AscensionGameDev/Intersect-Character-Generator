using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkUI.Controls;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Intersect_Character_Generator
{
    public class Layer
    {
        public Dictionary<string, string> MaleParts = new Dictionary<string, string>();
        public Dictionary<string, string> FemaleParts = new Dictionary<string, string>();

        private string directory;
        private DarkComboBox cmbItems;
        private DarkButton colorBtn;
        private frmGenerator frmGenerator;
        private bool maleSelected;
        private ColorDialog colorDialog;
        private TrackBar intensityBar;
        private TrackBar alphaBar;
        private PictureBox lockBox;

        private Bitmap origGraphic;
        private Bitmap alteredGraphic;
        public string graphicPath = "";

        public Layer(string folderName, DarkComboBox itemList, DarkButton colorButton, ColorDialog colorD, TrackBar intBar, TrackBar aBar,PictureBox lockPic, frmGenerator form)
        {
            if (!Directory.Exists("assets")) Directory.CreateDirectory("assets");
            frmGenerator = form;
            colorDialog = colorD;
            intensityBar = intBar;
            lockBox = lockPic;
            alphaBar = aBar;
            directory = folderName;
            cmbItems = itemList;
            colorBtn = colorButton;
            LoadItems();
            PopulateList(true);
            cmbItems.SelectedIndexChanged += cmbItems_SelectedIndexChanged;
            colorBtn.Click += btnColor_Click;
            intBar.ValueChanged += intBar_ValueChanged;
            alphaBar.ValueChanged += alphaBar_ValueChanged;
            lockBox.Click += LockBox_Click;
        }

        public int GetWidth()
        {
            return origGraphic == null ? 0 : origGraphic.Width;
        }

        public int GetHeight()
        {
            return origGraphic == null ? 0 : origGraphic.Height;
        }

        public string Get()
        {
            return cmbItems.Text;
        }

        public void Set(string graphic)
        {
            if (cmbItems.Items.Contains(graphic))
            {
                cmbItems.SelectedIndex = cmbItems.Items.IndexOf(graphic);
            }
        }

        public int GetHue()
        {
            return colorBtn.BackColor.ToArgb();
        }

        public void SetHue(int value)
        {
            colorBtn.BackColor = Color.FromArgb(value);
        }

        public int GetHueIntensity()
        {
            return intensityBar.Value;
        }

        public void SetHueIntensity(int value)
        {
            intensityBar.Value = value;
        }

        public int GetAlpha()
        {
            return alphaBar.Value;
        }

        public void SetAlpha(int value)
        {
            alphaBar.Value = value;
        }

        public bool GetRandLock()
        {
            return lockBox.Tag != null;
        }

        public void SetRandLock(bool value)
        {
            if (!value)
            {
                lockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                lockBox.Tag = null;
            }
            else
            {
                lockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                lockBox.Tag = 1;
            }
        }

        public void Draw(Graphics g, int frameWidth, int frameHeight)
        {
            if (origGraphic == null) return;
            var img = (colorBtn.BackColor.ToArgb() == Color.White.ToArgb() ? origGraphic : alteredGraphic);
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    float[][] matrixItems ={
                                new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, (100 - alphaBar.Value) / 100f, 0},
                                new float[] {0, 0, 0, 0, 1}};
                    ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(
                        colorMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Bitmap);
                    g.DrawImage(img, new Rectangle(x * frameWidth + (frameWidth - img.Width / 4) / 2, y * frameHeight + (frameHeight - img.Height / 4) / 2, img.Width / 4, img.Height / 4), x * img.Width / 4, y * img.Height / 4, img.Width / 4, img.Height / 4, GraphicsUnit.Pixel,imageAttributes);
                }
            }
        }

        private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var redraw = false;
            if (cmbItems.SelectedIndex == 0)
            {
                if (origGraphic != null)
                {
                    origGraphic.Dispose();
                    alteredGraphic.Dispose();
                    redraw = true;
                }
                origGraphic = null;
                alteredGraphic = null;
                graphicPath = "";
            }
            else
            {
                Dictionary<string, string> partList = maleSelected ? MaleParts : FemaleParts;
                if (partList.ContainsKey(cmbItems.Text))
                {
                    if (graphicPath != partList[cmbItems.Text])
                    {
                        graphicPath = partList[cmbItems.Text];
                        origGraphic = new Bitmap(graphicPath);
                        alteredGraphic = new Bitmap(origGraphic.Width, origGraphic.Height);
                        ProcessHue();
                        redraw = true;
                    }
                }
            }
            if (redraw) frmGenerator.DrawCharacter();
        }

        private void ProcessHue()
        {
            var btnBrightness = colorBtn.BackColor.GetBrightness();
            var btnSat = colorBtn.BackColor.GetSaturation();
            var btnHue = colorBtn.BackColor.GetHue();
            var intensityVal = intensityBar.Value;
            if (origGraphic == null) return;
            BitmapData origBmd = origGraphic.LockBits(new Rectangle(0, 0, origGraphic.Width, origGraphic.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, origGraphic.PixelFormat);
            BitmapData alteredBmd = alteredGraphic.LockBits(new Rectangle(0, 0, alteredGraphic.Width, alteredGraphic.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, alteredGraphic.PixelFormat);
            int PixelSize = 4;

            unsafe
            {
                for (int y = 0; y < origBmd.Height; y++)
                {
                    byte* origRow = (byte*)origBmd.Scan0 + (y * origBmd.Stride);
                    byte* alteredRow = (byte*)alteredBmd.Scan0 + (y * alteredBmd.Stride);
                    for (int x = 0; x < origBmd.Width; x++)
                    {
                        var clr = Color.FromArgb(origRow[x * PixelSize + 3], origRow[x * PixelSize + 2], origRow[x * PixelSize + 1], origRow[x * PixelSize]);
                        var alteredColor = ColorFromAhsb(clr.A, btnHue, btnSat, clr.GetBrightness() * btnBrightness + ((100 - intensityVal) / 100f));
                        alteredRow[x * PixelSize] = alteredColor.B;   //Blue  0-255
                        alteredRow[x * PixelSize + 1] = alteredColor.G; //Green 0-255
                        alteredRow[x * PixelSize + 2] = alteredColor.R;   //Red   0-255
                        alteredRow[x * PixelSize + 3] = alteredColor.A;  //Alpha 0-255
                    }
                }
            }

            origGraphic.UnlockBits(origBmd);
            alteredGraphic.UnlockBits(alteredBmd);
        }


        //Function stolen from:
        //https://blogs.msdn.microsoft.com/cjacks/2006/04/12/converting-from-hsb-to-rgb-in-net/
        public Color ColorFromAhsb(int a, float h, float s, float b)
        {

            if (0 > a || 255 < a)
            {
                return Color.White;
            }
            if (0f > h || 360f < h)
            {
                return Color.White;
            }
            if (0f > s || 1f < s)
            {
                return Color.White;
            }
            if (0f > b || 1f < b)
            {
                return Color.White;
            }

            if (0 == s)
            {
                return Color.FromArgb(a, Convert.ToInt32(b * 255),
                  Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(a, iMax, iMid, iMin);
            }
        }

        private void LoadItems()
        {
            MaleParts.Clear();
            FemaleParts.Clear();
            var folder = directory;
            if (!Directory.Exists(Path.Combine("assets", folder))) Directory.CreateDirectory(Path.Combine("assets", folder));
            AddImagesToList(Path.Combine("assets", folder, "male"), MaleParts);
            AddImagesToList(Path.Combine("assets", folder, "female"), FemaleParts);
            AddImagesToList(Path.Combine("assets", folder, "unisex"), MaleParts);
            AddImagesToList(Path.Combine("assets", folder, "unisex"), FemaleParts);
            MaleParts = SortDictionary(MaleParts);
            FemaleParts = SortDictionary(FemaleParts);
        }

        public void PopulateList(bool male)
        {
            maleSelected = male;
            cmbItems.Items.Clear();
            cmbItems.Items.Add("None");
            if (male)
            {
                cmbItems.Items.AddRange(MaleParts.Keys.ToArray());
            }
            else
            {
                cmbItems.Items.AddRange(FemaleParts.Keys.ToArray());
            }
            cmbItems.SelectedIndex = 0;
            if (cmbItems.Items.IndexOf("base") > 0) cmbItems.SelectedIndex = cmbItems.Items.IndexOf("base");
            if (cmbItems.Items.IndexOf("Base") > 0) cmbItems.SelectedIndex = cmbItems.Items.IndexOf("Base");
            cmbItems_SelectedIndexChanged(null, null);
        }

        public void Randomize(Random rand)
        {
            if (lockBox.Tag == null)
                cmbItems.SelectedIndex = rand.Next(0, cmbItems.Items.Count);
        }

        private void AddImagesToList(string folder, Dictionary<string, string> parts)
        {
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var images = Directory.GetFiles(folder, "*.png");
            foreach (var img in images)
            {
                if (!parts.ContainsKey(Path.GetFileNameWithoutExtension(img)))
                {
                    parts.Add(Path.GetFileNameWithoutExtension(img), img);
                }
            }
        }

        private Dictionary<string, string> SortDictionary(Dictionary<string, string> dict)
        {
            var newDict = new Dictionary<string, string>();
            var strings = dict.Keys.ToArray();
            Array.Sort(strings, new AlphanumComparator());
            foreach (var str in strings)
            {
                newDict.Add(str, dict[str]);
            }
            return newDict;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Button)sender).BackColor = colorDialog.Color;
                ProcessHue();
                frmGenerator.DrawCharacter();
            }
        }

        private void LockBox_Click(object sender, EventArgs e)
        {
            if (lockBox.Tag != null)
            {
                lockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                lockBox.Tag = null;
            }
            else
            {
                lockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                lockBox.Tag = 1;
            }
        }

        private void intBar_ValueChanged(object sender, EventArgs e)
        {
            ProcessHue();
            frmGenerator.DrawCharacter();
        }

        private void alphaBar_ValueChanged(object sender, EventArgs e)
        {
            frmGenerator.DrawCharacter();
        }
    }
}

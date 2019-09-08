using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace Intersect_Character_Generator
{
    public partial class frmGenerator : Form
    {
        public static frmGenerator Current;

        //Temporary Variables
        private Project _project = new Project();
        private bool _drawing = false;
        private List<Layer> _layers = new List<Layer>();
        private SaveFileDialog _saveSpriteDialog;
        private SaveFileDialog _saveProjectDialog;
        private OpenFileDialog _openProjectDialog;
        private Random _rand = new Random();

        public frmGenerator()
        {
            InitializeComponent();
            Current = this;
        }

        private void frmGenerator_Load(object sender, EventArgs e)
        {
            _layers.Add(new Layer("bodies", cmbBody, btnBodyHue, colorDialog, trkBodyHueIntensity, trkBodyAlpha, picBodyLock, this));
            _layers.Add(new Layer("eyes", cmbEyes, btnEyesHue, colorDialog, trkEyesHueIntensity, trkEyesAlpha, picEyesLock, this));
            _layers.Add(new Layer("hair", cmbHair, btnHairHue, colorDialog, trkHairHueIntensity, trkHairAlpha, picHairLock, this));
            _layers.Add(new Layer("facialhair", cmbFacialHair, btnFacialHairHue, colorDialog, trkFacialHairHueIntensity, trkFacialHairAlpha, picFacialHairLock, this));
            _layers.Add(new Layer("headwear", cmbHeadwear, btnHeadwearHue, colorDialog, trkHeadwearHueIntensity, trkHeadwearAlpha, picHeadwearLock, this));
            _layers.Add(new Layer("shirt", cmbShirt, btnShirtHue, colorDialog, trkShirtHueIntensity, trkShirtAlpha, picShirtLock, this));
            _layers.Add(new Layer("shoulders", cmbShoulders, btnShouldersHue, colorDialog, trkShouldersHueIntensity, trkShouldersAlpha, picShouldersLock, this));
            _layers.Add(new Layer("gloves", cmbGloves, btnGlovesHue, colorDialog, trkGlovesHueIntensity, trkGlovesAlpha, picGlovesLock, this));
            _layers.Add(new Layer("pants", cmbPants, btnPantsHue, colorDialog, trkPantsHueIntensity, trkPantsAlpha, picPantsLock, this));
            _layers.Add(new Layer("waist", cmbWaist, btnWaistHue, colorDialog, trkWaistHueIntensity, trkWaistAlpha, picWaistLock, this));
            _layers.Add(new Layer("boots", cmbBoots, btnBootsHue, colorDialog, trkBootsHueIntensity, trkBootsAlpha, picBootsLock, this));
            _layers.Add(new Layer("accessories", cmbAccessory1, btnAccessory1Hue, colorDialog, trkAccessory1HueIntensity, trkAccessory1Alpha, picAccessory1Lock, this));
            _layers.Add(new Layer("accessories", cmbAccessory2, btnAccessory2Hue, colorDialog, trkAccessory2HueIntensity, trkAccessory2Alpha, picAccessory2Lock, this));
            _layers.Add(new Layer("accessories", cmbAccessory3, btnAccessory3Hue, colorDialog, trkAccessory3HueIntensity, trkAccessory3Alpha, picAccessory3Lock, this));
            _layers.Add(new Layer("accessories", cmbAccessory4, btnAccessory4Hue, colorDialog, trkAccessory4HueIntensity, trkAccessory4Alpha, picAccessory4Lock, this));

            _saveSpriteDialog = new SaveFileDialog();
            _saveSpriteDialog.Filter = "PNG Image|*.png";
            _saveSpriteDialog.Title = "Save Sprite";
            _saveSpriteDialog.RestoreDirectory = true;

            _saveProjectDialog = new SaveFileDialog();
            _saveProjectDialog.Filter = "Intersect Character Generator Project File |*.iprj";
            _saveProjectDialog.Title = "Save Intersect Character Generator Project";
            _saveProjectDialog.RestoreDirectory = true;

            _openProjectDialog = new OpenFileDialog();
            _openProjectDialog.Filter = "Intersect Character Generator Project File |*.iprj";
            _openProjectDialog.Title = "Select your project file";
            _openProjectDialog.RestoreDirectory = true;

            DrawCharacter();
        }


        #region "Controls"
        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Button)sender).BackColor = colorDialog.Color;
                DrawCharacter();
                if (sender == btnBackgroundColor) chkTransparent.Checked = false;
            }
        }

        private void rdoMale_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var layer in _layers)
            {
                layer.PopulateList(rdoMale.Checked);
            }
        }

        private void genericEvent_DrawCharacter(object sender, EventArgs e)
        {
            DrawCharacter();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            _saveSpriteDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (_saveSpriteDialog.FileName != "")
            {
                var sprite = RenderCharacter(!chkTransparent.Checked);
                sprite.Save(_saveSpriteDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                sprite.Dispose();
            }
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            var maleChecked = rdoMale.Checked;
            if (picGenderLock.Tag == null)
            {
                rdoMale.Checked = _rand.Next(0, 2) == 1;
                rdoFemale.Checked = !rdoMale.Checked;
            }
            if (maleChecked != rdoMale.Checked)
            {
                foreach (var layer in _layers)
                {
                    layer.PopulateList(rdoMale.Checked);
                }
            }
            foreach (var layer in _layers)
            {
                layer.Randomize(_rand);
            }
        }

        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.ascensiongamedev.com/topic/3189-intersect-character-generator/");
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/AscensionGameDev/Intersect-Character-Generator");
        }

        private void picGenderLock_Click(object sender, EventArgs e)
        {
            if (picGenderLock.Tag != null)
            {
                picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                picGenderLock.Tag = null;
            }
            else
            {
                picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                picGenderLock.Tag = 1;
            }
        }
        private void saveStateAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _saveProjectDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (_saveProjectDialog.FileName != "")
            {
                _project.ProjectPath = _saveProjectDialog.FileName;
                var json = JsonConvert.SerializeObject(_project, Formatting.Indented);
                File.WriteAllText(_saveProjectDialog.FileName, json);
            }
        }
        private void openStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_openProjectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var json = File.ReadAllText(_openProjectDialog.FileName);
                JsonConvert.PopulateObject(json, _project, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
                _project.ProjectPath = _openProjectDialog.FileName;
                this.Text = "Intersect Character Generator - " + _project.ProjectPath;
                DrawCharacter();
            }
        }

        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var json = JsonConvert.SerializeObject(_project, Formatting.Indented);
            File.WriteAllText(_project.ProjectPath, json);
            this.Text = "Intersect Character Generator - " + _project.ProjectPath;
        }
        #endregion

        #region "Rendering"
        public void DrawCharacter()
        {
            if (picSprite.BackgroundImage != null)
            {
                picSprite.BackgroundImage.Dispose();
                picSprite.BackgroundImage = null;
            }
            picSprite.BackgroundImage = RenderCharacter(true);
            picSprite.Size = new Size(picSprite.BackgroundImage.Size.Width * trackZoom.Value, picSprite.BackgroundImage.Size.Height * trackZoom.Value);
            picSprite.BackColor = Color.Transparent;
        }

        private Bitmap RenderCharacter(bool drawBackground)
        {
            var size = new Size(128, 192);

            //figure out size...
            var maxW = 0;
            var maxH = 0;
            foreach (var layer in _layers)
            {
                if (layer.GetWidth() > maxW) maxW = layer.GetWidth();
                if (layer.GetHeight() > maxH) maxH = layer.GetHeight();
            }

            if (maxW > 0) size.Width = maxW;
            if (maxH > 0) size.Height = maxH;

            var rect = new Rectangle(0, 0, size.Width, size.Height);
            var bmp = new Bitmap(size.Width,size.Height);
            var g = Graphics.FromImage(bmp);
            if (drawBackground)
            {
                if (chkTransparent.Checked)
                {
                    var transTile = Intersect_Character_Generator.Properties.Resources.transtile;
                    for (int x = 0; x < size.Width / transTile.Width + 1; x++)
                    {
                        for (int y = 0; y < size.Height / transTile.Height + 1; y++)
                        {
                            g.DrawImage(transTile, new Rectangle(x * transTile.Width, y * transTile.Height, transTile.Width, transTile.Height), new Rectangle(0, 0, transTile.Width, transTile.Height), GraphicsUnit.Pixel);
                        }
                    }
                }
                else
                {
                    g.FillRectangle(new SolidBrush(btnBackgroundColor.BackColor),rect);
                }
            }
            RenderLayers(g, size);
            g.Dispose();
            return bmp;
        }
        private void RenderLayers(Graphics g, Size size)
        {
            if (_layers.Count >= 15)
            {
                _layers[0].Draw(g, size.Width / 4, size.Height / 4); //Base
                _layers[1].Draw(g, size.Width / 4, size.Height / 4); //Eyes
                _layers[2].Draw(g, size.Width / 4, size.Height / 4); //Hair
                _layers[3].Draw(g, size.Width / 4, size.Height / 4); //Facial Hair
                _layers[4].Draw(g, size.Width / 4, size.Height / 4); //Headwear
                if (chkPantsAfterShirt.Checked)
                {
                    //Draw shirt before pants
                    _layers[5].Draw(g, size.Width / 4, size.Height / 4); //Shirt
                    _layers[6].Draw(g, size.Width / 4, size.Height / 4); //Shoulders
                    _layers[7].Draw(g, size.Width / 4, size.Height / 4); //Gloves          
                    if (chkBootsAfterPants.Checked)
                    {
                        _layers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        _layers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                        _layers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                    }
                    else
                    {
                        _layers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                        _layers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        _layers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                    }
                }
                else
                {
                    //Draw pants then shirt
                    if (chkBootsAfterPants.Checked)
                    {
                        _layers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        _layers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                        _layers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                    }
                    else
                    {
                        _layers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                        _layers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        _layers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                    }
                    _layers[5].Draw(g, size.Width / 4, size.Height / 4); //Shirt
                    _layers[6].Draw(g, size.Width / 4, size.Height / 4); //Shoulders
                    _layers[7].Draw(g, size.Width / 4, size.Height / 4); //Gloves
                }
                _layers[11].Draw(g, size.Width / 4, size.Height / 4); //Accessory 1
                _layers[12].Draw(g, size.Width / 4, size.Height / 4); //Accessory 2
                _layers[13].Draw(g, size.Width / 4, size.Height / 4); //Accessory 3
                _layers[14].Draw(g, size.Width / 4, size.Height / 4); //Accessory 4
            }
        }

        #endregion


        #region "Saving/Loading Classes"
        public class Project
        {
            private string _projectPath = "";
            public string ProjectPath
            {
                get
                {
                    return _projectPath;
                }
                set
                {
                    _projectPath = value;
                    Current.saveStateToolStripMenuItem.Enabled = !string.IsNullOrEmpty(_projectPath);
                }
            }
            public int Gender
            {
                get => frmGenerator.Current.rdoMale.Checked ? 0 : 1;
                set
                {
                    if (value == 0)
                        Current.rdoMale.Checked = true;
                    else
                        Current.rdoFemale.Checked = true;
                }
            }
            public bool GenderLock
            {
                get => Current.picGenderLock.Tag != null;
                set
                {
                    if (value)
                    {
                        Current.picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                        Current.picGenderLock.Tag = 1;
                    }
                    else
                    {
                        Current.picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                        Current.picGenderLock.Tag = null;
                    }
                }
            }
            public int BackgroundColor
            {
                get
                {
                    return Current.chkTransparent.Checked ? Color.Transparent.ToArgb() : Current.btnBackgroundColor.BackColor.ToArgb();
                }
                set
                {
                    if (value == Color.Transparent.ToArgb())
                    {
                        Current.chkTransparent.Checked = true;
                    }
                    else
                    {
                        Current.chkTransparent.Checked = false;
                        Current.btnBackgroundColor.BackColor = Color.FromArgb(value);
                    }
                }
            }

            //Layers -_-
            public LayerSettings Body { get; set; } = new LayerSettings(0);
            public LayerSettings Eyes { get; set; } = new LayerSettings(1);
            public LayerSettings Hair { get; set; } = new LayerSettings(2);
            public LayerSettings FacialHair { get; set; } = new LayerSettings(3);
            public LayerSettings Headwear { get; set; } = new LayerSettings(4);
            public LayerSettings Shirt { get; set; } = new LayerSettings(5);
            public LayerSettings Shoulders { get; set; } = new LayerSettings(6);
            public LayerSettings Gloves { get; set; } = new LayerSettings(7);
            public LayerSettings Pants { get; set; } = new LayerSettings(8);
            public LayerSettings Waist { get; set; } = new LayerSettings(9);
            public LayerSettings Boots { get; set; } = new LayerSettings(10);
            public LayerSettings Accessory1 { get; set; } = new LayerSettings(11);
            public LayerSettings Accessory2 { get; set; } = new LayerSettings(12);
            public LayerSettings Accessory3 { get; set; } = new LayerSettings(13);
            public LayerSettings Accessory4 { get; set; } = new LayerSettings(14);

            public bool ShirtTuckedIn
            {
                get => Current.chkPantsAfterShirt.Checked;
                set => Current.chkPantsAfterShirt.Checked = value;
            }
            public bool PantsTuckedIn
            {
                get => Current.chkBootsAfterPants.Checked;
                set => Current.chkBootsAfterPants.Checked = value;
            }
            public int Zoom
            {
                get => Current.trackZoom.Value;
                set => Current.trackZoom.Value = value;
            }
            public string SaveFilePath
            {
                get => Current._saveSpriteDialog.FileName;
                set => Current._saveSpriteDialog.FileName = value;
            }
        }
        public class LayerSettings
        {
            public int Index { get; set; }
            public LayerSettings(int i)
            {
                Index = i;
            }

            public string Graphic
            {
                get
                {
                    return Current._layers[Index].Get();
                }
                set
                {
                    Current._layers[Index].Set(value);
                }
            }

            public int Hue
            {
                get
                {
                    return Current._layers[Index].GetHue();
                }
                set
                {
                    Current._layers[Index].SetHue(value);
                }
            }

            public int HueIntensity
            {
                get
                {
                    return Current._layers[Index].GetHueIntensity();
                }
                set
                {
                    Current._layers[Index].SetHueIntensity(value);
                }
            }

            public int Alpha
            {
                get
                {
                    return Current._layers[Index].GetAlpha();
                }
                set
                {
                    Current._layers[Index].SetAlpha(value);
                }
            }

            public bool RandomizationLocked
            {
                get
                {
                    return Current._layers[Index].GetRandLock();
                }
                set
                {
                    Current._layers[Index].SetRandLock(value);
                }
            }


        }
        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DWGProps2009
{

    public class DWGProps2009Control : UserControl
    {

        private bool bDPROINIT;
        private bool bLock1;
        private bool bLock2;
        private Container components;
        private string DWGAUTHOR;
        private string DWGOPEN;
        private PictureBox DWGPreviewBox;
        private string DWGTITLE;
        private string DWGVER;

        public DWGProps2009Control()
        {
            // fault handler present
        }

        private void ~DWGProps2009Control()
        {
            // trial
        }

        public void DPROINIT(string args, int k)
        {
            // trial
        }

        public void DWGPreview(string args)
        {
            int i2;

            FileStream fileStream = null;
            if (bDPROINIT)
            {
                DWGVER = "";
                DWGTITLE = "";
                DWGAUTHOR = "";
                DWGOPEN = "";
                byte[] bArr3 = new byte[1];
                byte[] bArr1 = new byte[2];
                byte[] bArr2 = new byte[6];
                uint[] uiArr = new uint[1];
                UTF8Encoding utf8Encoding = new UTF8Encoding();
                try
                {
                    fileStream = File.OpenRead(args);
                }
                catch (IOException e)
                {
                    Console.WriteLine("File is locked.", e.GetType().Name);
                    DWGOPEN = "Locked";
                    return;
                }
                try
                {
                    if (6 == fileStream.Read(bArr2, 0, 6))
                    {
                        string s = utf8Encoding.GetString(bArr2);
                        if (s == "AC1012" || s == "AC1014" || s == "AC1015" || s == "AC1018" || s == "AC1021")
                        {
                            DWGVER = s;
                            BinaryReader binaryReader = new BinaryReader(fileStream);
                            fileStream.Seek((long)13, SeekOrigin.Begin);
                            int i8 = (int)binaryReader.ReadUInt32() - 1;
                            byte[] bArr7 = new byte[i8];
                            byte[] bArr6 = binaryReader.ReadBytes(i8);
                            int i18 = (int)binaryReader.ReadUInt32();
                            byte[] bArr5 = new byte[11];
                            byte[] bArr4 = binaryReader.ReadBytes(11);
                            int i7 = (int)binaryReader.ReadUInt32();
                            int i5 = (int)binaryReader.ReadUInt32();
                            MemoryStream memoryStream = new MemoryStream();
                            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                            fileStream.Seek((long)i7, SeekOrigin.Begin);
                            if (i5 > 100)
                            {
                                int i17 = binaryReader.ReadInt32();
                                int i16 = binaryReader.ReadInt32();
                                int i15 = binaryReader.ReadInt32();
                                short sh2 = binaryReader.ReadInt16();
                                short sh1 = binaryReader.ReadInt16();
                                int i14 = binaryReader.ReadInt32();
                                int i13 = binaryReader.ReadInt32();
                                int i12 = binaryReader.ReadInt32();
                                int i11 = binaryReader.ReadInt32();
                                int i6 = binaryReader.ReadInt32();
                                int i10 = binaryReader.ReadInt32();
                                int i9 = 0;
                                if ((sh1 != 24) && (sh1 != 32))
                                {
                                    if (i6 == 0)
                                    {
                                        int i4 = 1, i3 = 0;
                                        while (true)
                                        {
                                            if ((short)i3 < sh1)
                                            {
                                                i4 <<= 1;
                                                i3++;
                                            }
                                        }
                                        i2 = i4 << 2;
                                    }
                                    else
                                    {
                                        i2 = i6 << 2;
                                    }
                                }
                                else
                                {
                                    i2 = 0;
                                }
                                bArr1[0] = 66;
                                bArr1[1] = 77;
                                binaryWriter.Write(bArr1[0]);
                                binaryWriter.Write(bArr1[1]);
                                uiArr[0] = i5 + 14;
                                binaryWriter.Write(uiArr[0]);
                                uiArr[0] = 0;
                                binaryWriter.Write(uiArr[0]);
                                uiArr[0] = i2 + 54;
                                binaryWriter.Write(uiArr[0]);
                                fileStream.Seek((long)i7, SeekOrigin.Begin);
                                int i1 = 0;
                                while (true)
                                {
                                    if (i1 < i5)
                                    {
                                        fileStream.Read(bArr3, 0, 1);
                                        binaryWriter.Write(bArr3[0]);
                                        i1++;
                                    }
                                }
                                DWGPreviewBox.Image = Image.FromStream(memoryStream);
                            }
                            binaryReader.Close();
                            binaryWriter.Close();
                            memoryStream.Close();
                        }
                    }
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Dispose();
                }
            }
        }

        public void DWGPreviewClear()
        {
            Bitmap bitmap = new Bitmap(DWGPreviewBox.Width, DWGPreviewBox.Height);
            DWGPreviewBox.Image = bitmap;
            Color color = DWGPreviewBox.BackColor;
            Graphics.FromImage(DWGPreviewBox.Image).Clear(color);
            DWGVER = "";
            DWGTITLE = "";
            DWGAUTHOR = "";
            DWGOPEN = "";
        }

        public void DWGProps(string args)
        {
            // trial
        }

        public string GetDWGAUTHOR()
        {
            // trial
            return null;
        }

        public string GetDWGOPEN()
        {
            return DWGOPEN;
        }

        public string GetDWGTITLE()
        {
            return DWGTITLE;
        }

        public string GetDWGVER()
        {
            // trial
            return null;
        }

        private void InitializeComponent()
        {
            PictureBox pictureBox = new PictureBox();
            DWGPreviewBox = pictureBox;
            pictureBox.BeginInit();
            SuspendLayout();
            DWGPreviewBox.Dock = DockStyle.Fill;
            Point point = new Point(0, 0);
            DWGPreviewBox.Location = point;
            DWGPreviewBox.Name = "DWGPreviewBox";
            Size size = new Size(150, 150);
            DWGPreviewBox.Size = size;
            DWGPreviewBox.SizeMode = PictureBoxSizeMode.StretchImage;
            DWGPreviewBox.TabIndex = 0;
            DWGPreviewBox.TabStop = false;
            SizeF sizeF = new SizeF(6.0F, 12.0F);
            AutoScaleDimensions = sizeF;
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(DWGPreviewBox);
            Name = "DWGProps2009Control";
            DWGPreviewBox.EndInit();
            ResumeLayout(false);
        }

        protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool )
        {
            // trial
        }

    } // class DWGProps2009Control

}

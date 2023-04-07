using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Lab_MaHoa_Nhom123
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();      
        }

        private static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        private bool isEncryptFile = true;
        private delegate void btnEncryptDecrypt();
        private void button1_Click(object sender, EventArgs e)
        {   
            if ((!checkB_Auto.Checked) && (!checkb_tc.Checked))
            {
                MessageBox.Show("Hãy chọn ''Tạo key tự động'' hoặc ''Tạo key tùy chọn''","THÔNG BÁO",MessageBoxButtons.OK);
                return;
            }

            int keySize;
            if (checkB_Auto.Checked)
            {
                Random random = new Random();
                int index = random.Next(0, cbb_KichThuoc.Items.Count);
                keySize = int.Parse(cbb_KichThuoc.Items[index].ToString());   
            }
            else
            {
                int k = int.Parse(cbb_KichThuoc.SelectedItem.ToString());
                keySize = k; 
            }


                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);

     
            {
                string publicKeyXml = rsa.ToXmlString(false);
                string privateKeyXml = rsa.ToXmlString(true);
                txt_Public.Text = publicKeyXml;
                txt_Private.Text = privateKeyXml;
                RSAParameters publicKey = rsa.ExportParameters(true);
                byte[] n = publicKey.Modulus;
                byte[] s = publicKey.Exponent;
                byte[] d = publicKey.D;
                string nn = "";
                string ss = "";
                string dd = "";
                foreach (byte t  in n)
                    nn = nn + t;
                foreach (byte t in s)
                    ss = ss + t;
                foreach (byte t in d)
                    dd = dd + t;
                txt_N.Text = nn;
                txt_E.Text = ss;
                txt_D.Text = dd;
            }
            RSA = rsa;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           cbb_KichThuoc.Items.Add(512); 
           cbb_KichThuoc.Items.Add(1024);
           cbb_KichThuoc.Items.Add(2048);
           cbb_KichThuoc.Items.Add(4096);
           this.MaximumSize = new Size(1299, 721);
           this.MinimumSize = new Size(1299, 721);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã có khóa RSA nào được tạo hay chưa
            if (txt_N.Text == "")
            {
                MessageBox.Show("Không có Key", "THÔNG BÁO", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.Title = "Save an XML File";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                txt_linkKey.Text = filePath;
                string privateKeyXml = RSA.ToXmlString(true);
                File.WriteAllText(filePath, privateKeyXml);
                saveFileDialog.Dispose();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void txtBanRo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_Key_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btngiaima_Click(object sender, EventArgs e)
        {
            if (KhongPhaiTiengViet(txtBanRo.Text))
                txtGiaiMa.Text = GiaiMaVigenere(txtBanMa.Text, txt_Key.Text);
            else
            {
                string s = txtBanMa.Text;
                txtGiaiMa.Text = VigenereGiaiMa(ref s, txt_Key.Text);
            }
        }

        private void btnmahoa_Click(object sender, EventArgs e)
        {
            if (txt_Key.Text == "")
            {
                MessageBox.Show("Vui lòng nhập Key", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Key.Focus();
            }
            else
            {
                if (KhongPhaiTiengViet(txtBanRo.Text))
                    txtBanMa.Text = MaHoaVigenere(txtBanRo.Text, txt_Key.Text);
                else
                {
                    string s = txtBanRo.Text;
                    txtBanMa.Text = VigenereMaHoa(ref s, txt_Key.Text);
                }
            }
        }

        static string BoDau(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedString[i]);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        static string chuoi = "AÁÀẠẢÃĂẮẰẶẲẴẤẦẬẨẪBCDĐEÉÈẸẺẼÊẾỀỆỂỄGHIÍÌỊỈĨJKLMNOÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠPQRSTUÚÙỦỤŨƯỨỪỰỬỮVWXYÝỲỴỶỸZ~`!@#$%^&*();:?/>.<, ";
        
        #region Mã hóa tiếng việt
        public static int[] chuoi_mangchiso(string s)
        {
            int[] mang = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                mang[i] = chuoi.IndexOf(s[i]);
            }
            return mang;
        }

        public static string chiso_chuoi(int[] a)
        {
            string s = "";
            for (int i = 0; i < a.Length; i++)
                s += chuoi[a[i]];
            return s;
        }
        static string VigenereMaHoa(ref string s, string key)
        {
            string cipherText = "";
            key = key.ToUpper();
            s = s.ToUpper();
            int[] p = chuoi_mangchiso(s);
            int[] k = chuoi_mangchiso(key);
            int[] kq = new int[s.Length];


            for (int i = 0, j = 0; i < s.Length; i++)
            {
                kq[i] = (p[i] + k[j]) % chuoi.Length;
                j = ++j % key.Length;
            }
            cipherText = chiso_chuoi(kq);
            return cipherText;
        }


        static string VigenereGiaiMa(ref string s, string key)
        {
            key = key.ToUpper();
            s = s.ToUpper();
            string plainText = "";
            int[] c = chuoi_mangchiso(s);
            int[] k = chuoi_mangchiso(key);
            int[] kq = new int[s.Length];


            for (int i = 0, j = 0; i < s.Length; i++)
            {
                kq[i] = (c[i] - k[j]) % chuoi.Length;
                if (kq[i] < 0)
                    kq[i] = (c[i] + (chuoi.Length - k[j])) % chuoi.Length;
                j = ++j % key.Length;
            }
            plainText = chiso_chuoi(kq);
            return plainText;
        }
        #endregion Mã hóa Tiếng Việt

        #region Mã hóa Tiếng Anh
        static string MaHoaVigenere(string plaintext, string key)
        {
            key = BoDau(key);
            key = key.Replace(" ", "");
            key = key.ToUpper();
            string cipherText = "";
            int keyIndex = 0;
            foreach (char c in plaintext)
            {
                if (!char.IsLetter(c))
                {
                    cipherText += c;
                }
                else
                {

                    int keyChar = key[keyIndex] - 'A';
                    int plainChar = char.ToUpper(c) - 'A';
                    int cipherChar = (plainChar + keyChar) % 26;


                    cipherText += (char)(cipherChar + 'A');


                    keyIndex++;
                    if (keyIndex == key.Length)
                    {
                        keyIndex = 0;
                    }
                }
            }
            return cipherText;
        }

        static string GiaiMaVigenere(string ciphertext, string key)
        {
            key = BoDau(key);
            key = key.Replace(" ", "");
            key = key.ToUpper();
            string plainText = "";
            int keyIndex = 0;

            for (int i = 0; i < ciphertext.Length; i++)
            {
                char c = ciphertext[i];

                if (char.IsLetter(c))
                {
                    int keyChar = key[keyIndex];
                    int shift = keyChar - 'A';
                    if (char.IsLower(c))
                    {
                        plainText += (char)((c - 'a' - shift + 26) % 26 + 'a');
                    }
                    else
                    {
                        plainText += (char)((c - 'A' - shift + 26) % 26 + 'A');
                    }
                    keyIndex++;

                    if (keyIndex == key.Length)
                    {
                        keyIndex = 0;
                    }
                }
                else
                {
                    plainText += c;
                }
            }

            return plainText;
        }
        #endregion mã hóa tiếng anh
        public static bool KhongPhaiTiengViet(string input)
        {
            foreach (char c in input)
            {
                int unicode = (int)c;
                if ((unicode >= 192 && unicode <= 255) || (unicode >= 7840 && unicode <= 7935))
                {
                    return false;
                }
            }
            return true;
        }
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            txtBanMa.Text = "";
            txtBanRo.Text = "";
            txtGiaiMa.Text = "";
            txt_Key.Text = "";
        }

        private void txtGiaiMa_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBanMa_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkb_tc_CheckedChanged(object sender, EventArgs e)
        {
            if (checkb_tc.Checked == true)
            {
                groupBox3.Enabled = true;
                checkB_Auto.Checked = false;
            }
            else { groupBox3.Enabled = false; }
        }

        private void checkB_Auto_CheckedChanged(object sender, EventArgs e)
        {
            if (checkB_Auto.Checked == true)
                checkb_tc.Checked = false;
        }

        private void btn_OpenKeys_Click(object sender, EventArgs e)
        {
            txt_Public.Text = "";
            txt_Private.Text = "";
            txt_N.Text = "";
            txt_D.Text = "";
            txt_E.Text = "";
            txt_openfile.Text = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
       
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(openFileDialog.FileName);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters rsaParams = new RSAParameters();

                // Lấy thông tin chuỗi
                rsaParams.Modulus = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/Modulus").InnerText);
                rsaParams.Exponent = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/Exponent").InnerText);
                rsaParams.D = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/D").InnerText);
                rsaParams.InverseQ = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/InverseQ").InnerText);
                rsaParams.DP = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/DP").InnerText);
                rsaParams.Q = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/Q").InnerText);
                rsaParams.DQ = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/DQ").InnerText);
                rsaParams.P = Convert.FromBase64String(xmlDoc.SelectSingleNode("/RSAKeyValue/P").InnerText);
                rsa.ImportParameters(rsaParams);

                txt_openfile.Text = openFileDialog.FileName;
                RSA = rsa;
                // Sử dụng đối tượng RSA để mã hóa/giải mã
                string publicKeyXml = rsa.ToXmlString(false);
                string privateKeyXml = rsa.ToXmlString(true);
                txt_Public.Text = publicKeyXml;
                txt_Private.Text = privateKeyXml;
                RSAParameters publicKey = rsa.ExportParameters(true);
                byte[] n = publicKey.Modulus;
                byte[] s = publicKey.Exponent;
                byte[] d = publicKey.D;
                string nn = "";
                string ss = "";
                string dd = "";
                foreach (byte t in n)
                    nn = nn + t;
                foreach (byte t in s)
                    ss = ss + t;
                foreach (byte t in d)
                    dd = dd + t;
                txt_N.Text = nn;
                txt_E.Text = ss;
                txt_D.Text = dd;
                openFileDialog.Dispose();
            }
          
           
            
        }

        private void btn_xoaAll_Click(object sender, EventArgs e)
        {
            RSA.Clear();
            txt_Public.Text = "";
            txt_Private.Text = "";
            txt_openfile.Text = "";
            txt_N.Text = "";
            txt_E.Text = "";
            txt_D.Text = "";
            txt_linkKey.Text = "";
            if (checkB_Auto.Checked) { checkB_Auto.Checked = false; }
            if (checkb_tc.Checked) { checkb_tc.Checked = false;}
        }
        static byte[] encryptedData = null;
        static byte[] decryptedData = null;
        private void btn_MaHoa_Click(object sender, EventArgs e)
        {
            if (txt_BanRo.Text == "")
            {
                MessageBox.Show("Chưa nhập dữ liệu vào bản rõ", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_BanRo.Focus();
                return;
            }
            else
            if (txt_N.Text == "")
            {
                MessageBox.Show("Không có Key", "THÔNG BÁO", MessageBoxButtons.OK);
                return;
            }
            else
            {
                encryptedData = null;
                // Chuyển đổi chuỗi sang mảng byte
                byte[] originalData = Encoding.UTF8.GetBytes(txt_BanRo.Text);

                // Mã hóa chuỗi bằng khóa công khai
                encryptedData = RSA.Encrypt(originalData, false);

                txt_BanMa.Text = Convert.ToBase64String(encryptedData);
            }
        }

        private void btn_GiaiMa_Click(object sender, EventArgs e)
        {
            if (txt_BanRo.Text == "") 
            {
                MessageBox.Show("Chưa nhập dữ liệu vào bản rõ", "THÔNG BÁO", MessageBoxButtons.OK);
                txt_BanRo.Focus();
                return;
            }
            decryptedData = null;
            // Giải mã chuỗi bằng khóa bí mật
            decryptedData = RSA.Decrypt(encryptedData, false);

            txt_GiaiMa.Text = Encoding.UTF8.GetString(decryptedData);
        }

        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            txt_GiaiMa.Text = "";
            txt_BanMa.Text = "";
            txt_BanRo.Text = "";
            encryptedData = null;
            decryptedData = null;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RSA.Clear();
            RSA.Dispose();
        }

        private void btn_OPf1_Click(object sender, EventArgs e)
        {
            isEncryptFile = true;
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All Files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
                txt_fileinput.Text = op.FileName;
        }

        private void btn_OPf2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f1 = new FolderBrowserDialog();
            if (f1.ShowDialog() == DialogResult.OK)
            {
                this.txt_fileoutput.Text = f1.SelectedPath;
            }
        }

        private void btn_MaHoaF_Click(object sender, EventArgs e)
        {
            if (txt_fileoutput.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục Output");
                return;
            }
            
            btnEncryptDecrypt s = new btnEncryptDecrypt(btnEncryptClick);
            s.BeginInvoke(null, null);
        }

        private void btn_GiaiMaF_Click(object sender, EventArgs e)
        {
            if (txt_fileoutput.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục Output");
                return;
            }
            btnEncryptDecrypt s = new btnEncryptDecrypt(btnDecryptClick);
            s.BeginInvoke(null, null);
        }

        private void RSA_Algorithm(string inputFile, string outputFile, RSAParameters RSAKeyInfo, bool isEncrypt)
        {
            try
            {
                FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read); //Đọc file input
                FileStream fsCiperText = new FileStream(outputFile, FileMode.Create, FileAccess.Write); //Tạo file output
                fsCiperText.SetLength(0);
                byte[] bin, encryptedData;
                long rdlen = 0;
                long totlen = fsInput.Length;
                int len;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = 100;


                

                int maxBytesCanEncrypted;
                //RSA chỉ có thể mã hóa các khối dữ liệu ngắn hơn độ dài khóa, chia dữ liệu cho một số khối và sau đó mã hóa từng khối và sau đó hợp nhất chúng
                if (isEncrypt)
                    maxBytesCanEncrypted = ((RSA.KeySize - 384) / 8) + 37;// + 7: OAEP - Đệm mã hóa bất đối xứng tối ưu

                else
                    maxBytesCanEncrypted = (RSA.KeySize / 8);
                //Read from the input file, then encrypt and write to the output file.
                while (rdlen < totlen)
                {
                    if (totlen - rdlen < maxBytesCanEncrypted) maxBytesCanEncrypted = (int)(totlen - rdlen);
                    bin = new byte[maxBytesCanEncrypted];
                    len = fsInput.Read(bin, 0, maxBytesCanEncrypted);

                    if (isEncrypt) encryptedData = RSA.Encrypt(bin, false); //Mã Hoá
                    else encryptedData = RSA.Decrypt(bin, false); //Giải mã

                    fsCiperText.Write(encryptedData, 0, encryptedData.Length);
                    rdlen = rdlen + len;

                    this.label1f.Text = "Tên tệp xử lý : " + Path.GetFileName(inputFile) + "\t Thành công: " + ((long)(rdlen * 100) / totlen).ToString() + " %";
                    this.label1f.Update();
                    this.label1f.Refresh();
                    this.progressBar1.Value = (int)((rdlen * 100) / totlen);//thanh tiến trình
                }
                
                fsCiperText.Close(); //save file
                fsInput.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
        }
        private void btnEncryptClick()
        {
            
            if (this.txt_N.Text.Length == 0 || this.txt_D.Text.Length == 0 || this.txt_E.Text.Length == 0)
            {
                MessageBox.Show("Key không hợp lệ!");
                
                return;
            }

            try
            {
                if (txt_fileinput.Text.Length != 0 &&
                txt_N.Text.Length != 0)
                {

                    //Calculator time ex...
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();
                    string inputFileName = txt_fileinput.Text, outputFileName = "";

                    if (isEncryptFile)
                    {
                        outputFileName = txt_fileoutput.Text + "\\" + Path.GetFileName(txt_fileinput.Text) + ".lhde";
                    }
                    //get Keys.
                    if (isEncryptFile)
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), true);
                    else
                    {
                        string[] filePaths = Directory.GetFiles(inputFileName, "*");

                        if (filePaths.Length == 0 || (filePaths.Length == 1 && (Path.GetFileName(filePaths[0]) == "Thumbs.db")))
                        {
                            MessageBox.Show("Thư mục rỗng!");
                           
                            return;
                        }



                        // tbt.Text = Path.GetDirectoryName(outputFileName);
                        for (int i = 0; i < filePaths.Length; i++)
                        {
                            outputFileName = txt_fileoutput.Text + "\\" + Path.GetFileName(filePaths[i]);
                            if (Path.GetFileName(filePaths[i]) != "Thumbs.db")
                                RSA_Algorithm(filePaths[i], outputFileName + ".lhde", RSA.ExportParameters(true), true);
                        }
                    }
                    
                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Thời gian thực thi " + elapsedMs.ToString() + "s");
                }
                else
                {
                   
                    MessageBox.Show("Dữ liệu không đủ để mã hóa!");
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Failed: " + ex.Message);
            }
          
        }
        private void btnDecryptClick()
        {


            try
            {
                if (txt_fileinput.Text.Length != 0 &&

                   txt_N.Text.Length != 0)
                {
                    //Calculator time ex...
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    string inputFileName = txt_fileinput.Text, outputFileName = "";

                    if (isEncryptFile && Path.GetExtension(inputFileName) != ".lhde")
                    {
                        MessageBox.Show("Tệp tin này không được hỗ trợ đển giải mã!");

                        return;
                    }

                    if (isEncryptFile)
                    {

                        outputFileName = txt_fileoutput.Text + "\\" + Path.GetFileName(inputFileName.Substring(0, inputFileName.Length - 5));


                    }



                    if (isEncryptFile)
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), false);
                    else
                    {
                        string[] filePaths = Directory.GetFiles(inputFileName, "*.lhde", SearchOption.AllDirectories);
                        if (filePaths.Length == 0 || (filePaths.Length == 1 && (Path.GetFileName(filePaths[0]) == "Thumbs.db")))
                        {
                            MessageBox.Show("Thư mục rỗng!");

                            return;
                        }

                        for (int i = 0; i < filePaths.Length; i++)
                            if (Path.GetFileName(filePaths[i]) != "Thumbs.db")
                            {
                                outputFileName = txt_fileoutput.Text + "\\" + Path.GetFileName(filePaths[i].Substring(0, filePaths[i].Length - 5));
                                RSA_Algorithm(filePaths[i], outputFileName, RSA.ExportParameters(true), false);

                            }

                    }

                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Tổng thời gian thực thi: " + elapsedMs.ToString() + "s");
                }
                else
                {
                    MessageBox.Show("Không đủ điều kiện để giải mã !");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
       
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (this.progressBar1.Value > 0)
                this.progressBar1.Value = 0;
            txt_fileinput.Text = "";
            txt_fileoutput.Text = "";
            this.label1f.Text = "";
            this.label1f.Update();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đóng chương trình?", "Xác nhận đóng chương trình", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}

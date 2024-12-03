using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KriptoParaTakipSistemi
{
    public partial class GirişPaneli : Form
    {
        

        public GirişPaneli()
        {
            InitializeComponent();    
            // Büyütme ve küçültme butonlarını kaldır
            this.MaximizeBox = false; // Büyütme butonunu kaldır
            this.MinimizeBox = false; // Küçültme butonunu kaldır
        }

        // Giriş deneme sayısını takip eden sayaç
        private int loginAttemptCounter = 0;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtKullanıcıAdı.Text;
            string password = txtŞifre.Text;

            // Kullanıcı bilgilerini kontrol et
            if (username == "yyü" && password == "yyü")
            {
                // Giriş başarılıysa ana formu aç
                GenelPanel anaForm = new GenelPanel();
                anaForm.Show();

                // Login formunu gizle
                this.Hide();
            }
            else
            {
                // Yanlış kullanıcı adı veya şifre girişi
                loginAttemptCounter++; // Hatalı giriş sayacını artır

                if (loginAttemptCounter >= 3)
                {
                    // 3 hatalı girişten sonra uygulamadan çık
                    MessageBox.Show("3 kez hatalı giriş yaptınız. Uygulama kapatılacak.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); // Uygulamayı kapat
                }
                else
                {
                    // Kullanıcıya hatalı giriş uyarısı ver
                    MessageBox.Show($"Hatalı kullanıcı adı veya şifre! Kalan deneme hakkı: {3 - loginAttemptCounter}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Giriş alanlarını temizle
                    txtKullanıcıAdı.Clear();
                    txtŞifre.Clear();

                    // İmleci tekrar kullanıcı adı TextBox'ına odakla
                    txtKullanıcıAdı.Focus();
                }
            }
        }

       

        private void GirişForm_Load(object sender, EventArgs e)
        {

        }


        private void txtKullanıcıAdı_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtŞifre_TextChanged(object sender, EventArgs e)
        {
            txtŞifre.PasswordChar = '*';
        }

      
    }
}

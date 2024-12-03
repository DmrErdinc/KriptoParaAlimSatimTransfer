using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace KriptoParaTakipSistemi
{

    public partial class TransferPaneli : Form
    {


        string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";
        private void TransferPaneli_Load(object sender, EventArgs e)
        {

        }
        public TransferPaneli()
        {
            InitializeComponent();
        }
        // Formu temizlemek için fonksiyon
        private void ClearForm()
        {
            // ComboBox'ları temizle
            cmbCoin.SelectedIndex = -1;  // Seçili öğeyi kaldırır
            cmbAğ.SelectedIndex = -1;    // Seçili öğeyi kaldırır

            // TextBox'ları temizle
            txtMiktar.Clear();  // Miktar kutusunu temizler
            txtAğAdres.Clear(); // Ağ adresi kutusunu temizler
        }
        //************************************************************//

        // CoinGecko API'sinden kripto para fiyatını almak için method
        private async Task<decimal> GetCryptoPriceAsync(string coin)
        {
            string url = $"https://api.coingecko.com/api/v3/simple/price?ids={coin.ToLower()}&vs_currencies=usd";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(content);
                decimal price = json[coin.ToLower()]["usd"].ToObject<decimal>(); // USD cinsinden fiyat alıyoruz.
                return price;
            }
        }

        //************************************************************//

        private async void btnGönder_Click(object sender, EventArgs e)
        {

            // Eğer herhangi bir TextBox veya ComboBox boşsa uyarı ver
            if (string.IsNullOrWhiteSpace(txtAğAdres.Text) ||
                string.IsNullOrWhiteSpace(txtMiktar.Text) ||
                cmbAğ.SelectedIndex == -1 ||
                cmbCoin.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Tüm bilgileri doldurunuz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            // Kullanıcıya onay mesajı göster
            DialogResult result = MessageBox.Show(
                "Göndermek istediğinize emin misiniz?\nLütfen ağ ve adresi kontrol ediniz",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            string SeçilenKripto = cmbCoin.SelectedItem.ToString(); // Seçilen kripto türü
            string SeçilenAğ = cmbAğ.SelectedItem.ToString(); // Seçilen ağ türü
            decimal TransferTutarı = decimal.Parse(txtMiktar.Text); // Transfer tutarı

            // Bakiye sorgulama
            decimal Bakiye = VeritabanındanBakiyeGetir(SeçilenKripto);

            if (Bakiye >= TransferTutarı)
            {
                // Dinamik fiyatları almak için API'yi çağır
                decimal BirimFiyat = await GetCryptoPriceAsync(SeçilenKripto);
                decimal ToplamMiktar = TransferTutarı * BirimFiyat;

                // Transferi yap
                TransferYap(SeçilenKripto, SeçilenAğ, TransferTutarı, BirimFiyat, ToplamMiktar);
                BakiyeyiGüncelle(SeçilenKripto, TransferTutarı);
            }
            else
            {
                MessageBox.Show("Yeterli bakiye yok!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //************************************************************//

        private decimal VeritabanındanBakiyeGetir(string coin)
        {
            decimal Bakiye = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Bakiye FROM [Cüzdan1].[dbo].[Cuzdan] WHERE KriptoTur = @Coin";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Coin", coin);

                try
                {
                    conn.Open();
                    Bakiye = (decimal)cmd.ExecuteScalar(); // Bakiye değeri alınır
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message);
                }
            }

            return Bakiye;
        }

        //************************************************************//

        private async void TransferYap(string coin, string Ağ, decimal Miktar, decimal BirimFiyat, decimal ToplamMiktar)
        {
            // Güncel kripto fiyatını al
            decimal GüncelFiyat = await GetCryptoPriceAsync(coin);

            string AğAdresi = txtAğAdres.Text; // Ağ adresini al

            // Transferi veritabanına kaydet
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [Cüzdan1].[dbo].[Islemler] (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam, IslemTarihi, AğAdresi) " +
                               "VALUES (@Coin, @IslemTur, @Miktar, @BirimFiyat, @ToplamMiktar, @TransferGünü, @AğAdresi)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Coin", coin);
                cmd.Parameters.AddWithValue("@IslemTur", Ağ);
                cmd.Parameters.AddWithValue("@Miktar", Miktar);
                cmd.Parameters.AddWithValue("@BirimFiyat", BirimFiyat);
                cmd.Parameters.AddWithValue("@ToplamMiktar", ToplamMiktar);
                cmd.Parameters.AddWithValue("@TransferGünü", DateTime.Now);
                cmd.Parameters.AddWithValue("@AğAdresi", AğAdresi); // Ağ adresi ekleniyor

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery(); // Transfer işlemi kaydedilir
                    MessageBox.Show($"Transfer başarıyla gerçekleştirildi.\nGönderilen Miktar: {Miktar} {coin}\n" +
                                    $"Güncel Fiyat: {GüncelFiyat} USD\n" +
                                    $"Toplam Değer: {Miktar * GüncelFiyat} USD\n" +
                                    $"Ağ Adresi: {AğAdresi}",
                                    "Başarılı",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    // Gönderim işlemi tamamlandıktan sonra form elemanlarını temizle
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message);
                }
            }
        }

        //************************************************************//

        private void BakiyeyiGüncelle(string coin, decimal Miktar)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Cüzdan1].[dbo].[Cuzdan] SET Bakiye = Bakiye - @Miktar WHERE KriptoTur = @Coin";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Miktar", Miktar);
                cmd.Parameters.AddWithValue("@Coin", coin);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery(); // Bakiye güncellenir
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message);
                }
            }
        }

    }
}
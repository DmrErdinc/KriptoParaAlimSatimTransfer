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
    public partial class SolanaPaneli : Form
    {
        public SolanaPaneli()
        {
            InitializeComponent();
        }

        private async Task<decimal> GetSolanaPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=solana&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal SolanaFiyat = Convert.ToDecimal(prices["solana"]?["usd"]?.ToString());


                        // Solana fiyatını ekrana yaz
                        lblSolana.Text = $"Solana: ${SolanaFiyat:0.##}";

                        return SolanaFiyat;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
                return 0;
            }
        }

        private async void btnSolanaFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetSolanaPriceFromAPI();
        }

        private void SolanaPaneli_Load(object sender, EventArgs e)
        {
            btnSolanaFiyatYenile.PerformClick();

            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Solana bakiyesi için SQL sorgusu
            string SolQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Solana'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Solana bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(SolQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblSolanaBakiye.Text = $"Solana: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblSolanaBakiye.Text = "Solana Bakiyesi: 0";
                        }
                    }

                    // USDT bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(usdtQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblUsdtBakiye.Text = $"USDT Bakiyesi: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblUsdtBakiye.Text = "USDT Bakiyesi: 0";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
                }
            }

        }
        //***************************************************//
        private async void btnSolanaAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Solana miktarını al
                decimal SolMiktar = Convert.ToDecimal(txtSolanaAl.Text); // Kullanıcıdan alınan Solana miktarı (decimal olarak al)

                decimal SolanaFiyat = 0;

                // Solana fiyatını API'den almaya çalış
                SolanaFiyat = await GetSolanaPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (SolanaFiyat == 0)
                {
                    SolanaFiyat = GetLastSolanaPriceFromDB();

                    if (SolanaFiyat == 0)
                    {
                        MessageBox.Show("Solana fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = SolMiktar * SolanaFiyat;

                // USDT bakiyesini kontrol et
                string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";
                decimal usdtBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(usdtQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            usdtBakiye = Convert.ToDecimal(result); // USDT bakiyesini al
                        }
                    }
                }

                // USDT bakiyesi ve toplam tutarı yuvarlayalım (8 haneli hassasiyetle)
                usdtBakiye = Math.Round(usdtBakiye, 8);
                toplamTutar = Math.Round(toplamTutar, 8);

                // Yetersiz bakiye kontrolü
                if (usdtBakiye >= toplamTutar)
                {
                    // Alım işlemi için SQL sorgusu
                    string query = @"
        BEGIN TRANSACTION;
        -- USDT düşür
        UPDATE Cuzdan
        SET Bakiye = Bakiye - @ToplamTutar
        WHERE KriptoTur = 'USDT';

        -- Solana artır (girilen miktar kadar SOL eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Solana';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Solana', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", SolMiktar); // Girilen SOL miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", SolanaFiyat); // Alım yapılan Solana fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtSolanaAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    SolanaPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Solana alım işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz bakiye durumunda mesaj göster
                    MessageBox.Show($"Yetersiz USDT bakiyesi! Mevcut bakiyeniz: {usdtBakiye}, Gerekli tutar: {toplamTutar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        //***************************************************//


        private async void btnSolanaSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Solana miktarını al
                decimal SolMiktar = Convert.ToDecimal(txtSolanaMiktarSat.Text); // Kullanıcıdan alınan Solana miktarı

                decimal SolanaFiyat = 0;

                // Solana fiyatını API'den almaya çalış
                SolanaFiyat = await GetSolanaPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (SolanaFiyat == 0)
                {
                    SolanaFiyat = GetLastSolanaPriceFromDB();

                    if (SolanaFiyat == 0)
                    {
                        MessageBox.Show("Solana fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış işlemiyle kazanılacak toplam USDT tutarını hesapla
                decimal toplamTutar = SolMiktar * SolanaFiyat;

                // Solana bakiyesini kontrol et
                string SolQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Solana'";
                decimal SolBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SolQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            SolBakiye = Convert.ToDecimal(result); // Solana bakiyesini al
                        }
                    }
                }

                // Solana bakiyesi ve satılacak miktarı yuvarlayalım (8 haneli hassasiyetle)
                SolBakiye = Math.Round(SolBakiye, 8);
                SolMiktar = Math.Round(SolMiktar, 8);

                // Yetersiz Solana kontrolü
                if (SolBakiye >= SolMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
BEGIN TRANSACTION;
-- Solana düşür
UPDATE Cuzdan
SET Bakiye = Bakiye - @Miktar
WHERE KriptoTur = 'Solana';

-- USDT artır
UPDATE Cuzdan
SET Bakiye = Bakiye + @ToplamTutar
WHERE KriptoTur = 'USDT';

-- İşlemi kaydet
INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
VALUES ('Solana', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

COMMIT TRANSACTION;
";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekle
                            cmd.Parameters.AddWithValue("@Miktar", SolMiktar); // Satılan Solana miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", SolanaFiyat); // Satış yapılan fiyat
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtSolanaMiktarSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    SolanaPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Solana satım işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz Solana durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Solana bakiyesi! Mevcut bakiyeniz: {SolBakiye}, Satmak istediğiniz miktar: {SolMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        //***************************************************//
        private decimal GetLastSolanaPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Solana' ORDER BY IslemID DESC"; // Son işlem fiyatını al

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lastPrice = Convert.ToDecimal(result); // Fiyatı decimal olarak al
                    }
                }
            }

            return lastPrice;
        }

       
    }

}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KriptoParaTakipSistemi
{
    public partial class LitcoinPaneli : Form
    {
        public LitcoinPaneli()
        {
            InitializeComponent();
        }
        // Litecoin fiyatını API üzerinden çekme
        // Litecoin fiyatını API'den çekerken yuvarlama yapılması
        private async Task<decimal> GetLitecoinPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=Litecoin&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal LitecoinFiyat = Convert.ToDecimal(prices["litecoin"]?["usd"]?.ToString());


                        // Litecoin fiyatını ekrana yaz
                        lblLitecoin.Text = $"Litecoin: ${LitecoinFiyat:0.##}";

                        return LitecoinFiyat;
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
        private async void btnLitecoinFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetLitecoinPriceFromAPI();
        }

        private void LitcoinPaneli_Load(object sender, EventArgs e)
        {
            btnLitecoinFiyatYenile.PerformClick();
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Litecoin bakiyesi için SQL sorgusu
            string LtcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Litecoin'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Litecoin bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(LtcQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblLtcBakiye.Text = $"Litecoin: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblLtcBakiye.Text = "Litecoin Bakiyesi: 0";
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
        private async void btnLitecoinAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Litecoin miktarını al
                decimal LtcMiktar = Convert.ToDecimal(txtLitecoinAl.Text); // Kullanıcıdan alınan Litecoin miktarı (decimal olarak al)

                decimal LitecoinFiyat = 0;

                // Litecoin fiyatını API'den almaya çalış
                LitecoinFiyat = await GetLitecoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (LitecoinFiyat == 0)
                {
                    LitecoinFiyat = GetLastLitecoinPriceFromDB();

                    if (LitecoinFiyat == 0)
                    {
                        MessageBox.Show("Litecoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = LtcMiktar * LitecoinFiyat;

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

        -- Litecoin artır (girilen miktar kadar LTC eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Litecoin';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Litecoin', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", LtcMiktar); // Girilen LTC miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", LitecoinFiyat); // Alım yapılan Litecoin fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtLitecoinAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    LitcoinPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Litecoin alım işlemi başarıyla tamamlandı!");
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

        private async void btnLitecoinSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Litecoin miktarını al
                decimal LtcMiktar = Convert.ToDecimal(txtMiktarLtcSat.Text); // Kullanıcıdan alınan Litecoin miktarı

                decimal LitecoinFiyat = 0;

                // Litecoin fiyatını API'den almaya çalış
                LitecoinFiyat = await GetLitecoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (LitecoinFiyat == 0)
                {
                    LitecoinFiyat = GetLastLitecoinPriceFromDB();

                    if (LitecoinFiyat == 0)
                    {
                        MessageBox.Show("Litecoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış işlemiyle kazanılacak toplam USDT tutarını hesapla
                decimal toplamTutar = LtcMiktar * LitecoinFiyat;

                // Litecoin bakiyesini kontrol et
                string ltcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Litecoin'";
                decimal ltcBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(ltcQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            ltcBakiye = Convert.ToDecimal(result); // Litecoin bakiyesini al
                        }
                    }
                }

                // Litecoin bakiyesi ve satılacak miktarı yuvarlayalım (8 haneli hassasiyetle)
                ltcBakiye = Math.Round(ltcBakiye, 8);
                LtcMiktar = Math.Round(LtcMiktar, 8);

                // Yetersiz Litecoin kontrolü
                if (ltcBakiye >= LtcMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
BEGIN TRANSACTION;
-- Litecoin düşür
UPDATE Cuzdan
SET Bakiye = Bakiye - @Miktar
WHERE KriptoTur = 'Litecoin';

-- USDT artır
UPDATE Cuzdan
SET Bakiye = Bakiye + @ToplamTutar
WHERE KriptoTur = 'USDT';

-- İşlemi kaydet
INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
VALUES ('Litecoin', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

COMMIT TRANSACTION;
";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekle
                            cmd.Parameters.AddWithValue("@Miktar", LtcMiktar); // Satılan Litecoin miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", LitecoinFiyat); // Satış yapılan fiyat
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtMiktarLtcSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    LitcoinPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Litecoin satım işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz Litecoin durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Litecoin bakiyesi! Mevcut bakiyeniz: {ltcBakiye}, Satmak istediğiniz miktar: {LtcMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        //***************************************************//


        // Veritabanından son alınan Litecoin fiyatını çekmek için bir fonksiyon
        private decimal GetLastLitecoinPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Litecoin' ORDER BY IslemID DESC"; // Son işlem fiyatını al

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

        

        private void btnLtcSat_Click(object sender, EventArgs e)
        {

        }
    }
}

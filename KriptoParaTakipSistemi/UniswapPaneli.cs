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
    public partial class UniswapPaneli : Form
    {
        public UniswapPaneli()
        {
            InitializeComponent();
        }
        private void UniswapPaneli_Load(object sender, EventArgs e)
        {
            btnUniswapFiyatYenile.PerformClick();
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Uniswap bakiyesi için SQL sorgusu
            string UniQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Uniswap'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Uniswap bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(UniQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblUniswapBakiye.Text = $"Uniswap: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblUniswapBakiye.Text = "Uniswap Bakiyesi: 0";
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

        private async void btnUniswapFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetUniswapPriceFromAPI();
        }

        private async Task<decimal> GetUniswapPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=uniswap&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal UniswapFiyat = Convert.ToDecimal(prices["uniswap"]?["usd"]?.ToString());


                        // uniswap fiyatını ekrana yaz
                        lblUniswap.Text = $"Uniswap: ${UniswapFiyat:0.##}";

                        return UniswapFiyat;
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
        //***************************************************//
        private async void btnUniswapAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Uniswap miktarını al
                decimal UniMiktar = Convert.ToDecimal(txtUniswapAl.Text); // Kullanıcıdan alınan Uniswap miktarı (decimal olarak al)

                decimal UniswapFiyat = 0;

                // Uniswap fiyatını API'den almaya çalış
                UniswapFiyat = await GetUniswapPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (UniswapFiyat == 0)
                {
                    UniswapFiyat = GetLastUniwsapPriceFromDB();

                    if (UniswapFiyat == 0)
                    {
                        MessageBox.Show("Uniswap fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = UniMiktar * UniswapFiyat;

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

        -- Uniswap artır (girilen miktar kadar UNİ eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Uniswap';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Uniswap', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", UniMiktar); // Girilen UNİ miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", UniswapFiyat); // Alım yapılan Uniswap fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtUniswapAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    UniswapPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Uniswap alım işlemi başarıyla tamamlandı!");
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
        private async void btnUniswapSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Uniswap miktarını al
                decimal UniMiktar = Convert.ToDecimal(txtUniswapMiktarSat.Text); // Kullanıcıdan alınan Uniswap miktarı (decimal olarak al)

                decimal UniswapFiyat = 0;

                // Uniswap fiyatını API'den almaya çalış
                UniswapFiyat = await GetUniswapPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (UniswapFiyat == 0)
                {
                    UniswapFiyat = GetLastUniwsapPriceFromDB();

                    if (UniswapFiyat == 0)
                    {
                        MessageBox.Show("Uniswap fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = UniMiktar * UniswapFiyat;

                // Uniswap bakiyesini kontrol et
                string UniQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Uniswap'";
                decimal UniBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(UniQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            UniBakiye = Convert.ToDecimal(result); // Uniswap bakiyesini al
                        }
                    }
                }

                // Uniswap bakiyesi ve toplam tutarı yuvarlayalım (8 haneli hassasiyetle)
                UniBakiye = Math.Round(UniBakiye, 8);
                toplamTutar = Math.Round(toplamTutar, 8);

                // Yetersiz bakiye kontrolü
                if (UniBakiye >= UniMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
    BEGIN TRANSACTION;
    -- Uniswap düşür
    UPDATE Cuzdan
    SET Bakiye = Bakiye - @Miktar
    WHERE KriptoTur = 'Uniswap';

    -- USDT artır
    UPDATE Cuzdan
    SET Bakiye = Bakiye + @ToplamTutar
    WHERE KriptoTur = 'USDT';

    -- İşlemi kaydet
    INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
    VALUES ('Uniswap', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

    COMMIT TRANSACTION;
    ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", UniMiktar); // Satılan Uniswap miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", UniswapFiyat); // Satış yapılan Uniswap fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // TextBox'ı boşalt
                    txtUniswapMiktarSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    UniswapPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Uniswap satış işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz bakiye durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Uniswap bakiyesi! Mevcut bakiyeniz: {UniBakiye}, Gerekli miktar: {UniMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        //***************************************************//

        // Veritabanından son alınan Uniwsap fiyatını çekmek için bir fonksiyon
        private decimal GetLastUniwsapPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Uniswap ismini doğru kullanmalısınız
            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Uniswap' ORDER BY IslemID DESC";

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

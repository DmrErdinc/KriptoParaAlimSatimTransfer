using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Data.SqlClient;

namespace KriptoParaTakipSistemi
{
    public partial class DogecoinPaneli : Form
    {
        private static readonly HttpClient client = new HttpClient(); // HttpClient'i tekrar kullanılabilir hale getirin

        public DogecoinPaneli()
        {
            InitializeComponent();
            this.MaximizeBox = false; // Büyütme butonunu kaldır
            this.MinimizeBox = false; // Küçültme butonunu kaldır
        }

        private void DogecoinPaneli_Load(object sender, EventArgs e)
        {
            btnDogeFiyatYenile.PerformClick();
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Dogecoin bakiyesi için SQL sorgusu
            string dogeQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Dogecoin'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Dogecoin bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(dogeQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblDogeBakiye.Text = $"Dogecoin Bakiyesi: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblDogeBakiye.Text = "Dogecoin Bakiyesi: 0";
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

        // Dogecoin fiyatını API üzerinden çekme
        // Dogecoin fiyatını API'den çekerken yuvarlama yapılması
        private async Task<decimal> GetDogecoinPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=dogecoin&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal DogecoinFiyat = Convert.ToDecimal(prices["dogecoin"]?["usd"]?.ToString());


                        // Dogecoin fiyatını ekrana yaz
                        lblDogecoin.Text = $"Dogecoin: ${DogecoinFiyat:0.##}";

                        return DogecoinFiyat;
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
        private async void btnDogeFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetDogecoinPriceFromAPI();
        }
        //***************************************************//
        private async void btnDogeAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Bitcoin miktarını al
                decimal DogeMiktar = Convert.ToDecimal(txtDogeAl.Text); // Kullanıcıdan alınan Dogecoin miktarı (decimal olarak al)

                decimal DogecoinFiyat = 0;

                // Bitcoin fiyatını API'den almaya çalış
                DogecoinFiyat = await GetDogecoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (DogecoinFiyat == 0)
                {
                    DogecoinFiyat = GetLastDogecoinPriceFromDB();

                    if (DogecoinFiyat == 0)
                    {
                        MessageBox.Show("Dogecoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = DogeMiktar * DogecoinFiyat;

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

        -- Dogecoin artır (girilen miktar kadar Doge eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Dogecoin';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Dogecoin', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", DogeMiktar); // Girilen Doge miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", DogecoinFiyat); // Alım yapılan Bitcoin fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtDogeAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    DogecoinPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Dogecoin alım işlemi başarıyla tamamlandı!");
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
        private async void btnDogeSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Dogecoin miktarını al
                decimal DogeMiktar = Convert.ToDecimal(txtDogeMiktarSat.Text); // Kullanıcıdan alınan Dogecoin miktarı

                decimal DogecoinFiyat = 0;

                // Dogecoin fiyatını API'den almaya çalış
                DogecoinFiyat = await GetDogecoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (DogecoinFiyat == 0)
                {
                    DogecoinFiyat = GetLastDogecoinPriceFromDB();

                    if (DogecoinFiyat == 0)
                    {
                        MessageBox.Show("Dogecoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış işlemiyle kazanılacak toplam USDT tutarını hesapla
                decimal toplamTutar = DogeMiktar * DogecoinFiyat;

                // Dogecoin bakiyesini kontrol et
                string dogeQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Dogecoin'";
                decimal dogeBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(dogeQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            dogeBakiye = Convert.ToDecimal(result); // Dogecoin bakiyesini al
                        }
                    }
                }

                // Dogecoin bakiyesi ve toplam miktarı yuvarlayalım (8 haneli hassasiyetle)
                dogeBakiye = Math.Round(dogeBakiye, 8);
                DogeMiktar = Math.Round(DogeMiktar, 8);

                // Yetersiz Dogecoin kontrolü
                if (dogeBakiye >= DogeMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
BEGIN TRANSACTION;
-- Dogecoin düşür
UPDATE Cuzdan
SET Bakiye = Bakiye - @Miktar
WHERE KriptoTur = 'Dogecoin';

-- USDT artır
UPDATE Cuzdan
SET Bakiye = Bakiye + @ToplamTutar
WHERE KriptoTur = 'USDT';

-- İşlemi kaydet
INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
VALUES ('Dogecoin', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

COMMIT TRANSACTION;
";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", DogeMiktar); // Satılan Doge miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", DogecoinFiyat); // Satış yapılan fiyat
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtDogeMiktarSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    DogecoinPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Dogecoin satım işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz Dogecoin durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Dogecoin bakiyesi! Mevcut bakiyeniz: {dogeBakiye}, Satmak istediğiniz miktar: {DogeMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        //***************************************************//

        // Veritabanından son alınan Dogecoin fiyatını çekmek için bir fonksiyon
        private decimal GetLastDogecoinPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Dogecoin' ORDER BY IslemID DESC"; // Son işlem fiyatını al

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
        //***************************************************//
    }
}

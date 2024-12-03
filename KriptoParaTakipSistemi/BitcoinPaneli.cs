using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;


namespace KriptoParaTakipSistemi
{
    public partial class BitcoinPaneli : Form
    {
        public BitcoinPaneli()
        {
            InitializeComponent();
            // Büyütme ve küçültme butonlarını kaldır
            this.MaximizeBox = false; // Büyütme butonunu kaldır
            this.MinimizeBox = false; // Küçültme butonunu kaldır

            // Form yüklendiğinde bakiyeleri güncelle
            BitcoinForm_Load(null, null);
        }

        private void BitcoinForm_Load(object sender, EventArgs e)
        {
            btnBtcFiyatYenile.PerformClick();
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Bitcoin bakiyesi için SQL sorgusu
            string btcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Bitcoin'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Bitcoin bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(btcQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblBtcBakiye.Text = $"Bitcoin Bakiyesi: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblBtcBakiye.Text = "Bitcoin Bakiyesi: 0";
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
        //************************************************************//

        // Bitcoin fiyatını API üzerinden çekme
        // Bitcoin fiyatını API'den çekerken yuvarlama yapılması
        private async Task<decimal> GetBitcoinPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal bitcoinFiyat = Convert.ToDecimal(prices["bitcoin"]?["usd"]?.ToString());


                        // Bitcoin fiyatını ekrana yaz
                        lblBitcoin.Text = $"Bitcoin: ${bitcoinFiyat:0.##}";

                        return bitcoinFiyat;
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

        //************************************************************//
        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }


        private void bunifuTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void lblBitcoin_Click(object sender, EventArgs e)
        {

        }

        private async void btnBtcFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetBitcoinPriceFromAPI();
        }
        //************************************************************//
        // Bitcoin alım işlemi
        private async void btnBtcAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Bitcoin miktarını al
                decimal btcMiktar = Convert.ToDecimal(txtBtcAl.Text); // Kullanıcıdan alınan Bitcoin miktarı (decimal olarak al)

                decimal bitcoinFiyat = 0;

                // Bitcoin fiyatını API'den almaya çalış
                bitcoinFiyat = await GetBitcoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (bitcoinFiyat == 0)
                {
                    bitcoinFiyat = GetLastBitcoinPriceFromDB();

                    if (bitcoinFiyat == 0)
                    {
                        MessageBox.Show("Bitcoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = btcMiktar * bitcoinFiyat;

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

        -- Bitcoin artır (girilen miktar kadar BTC eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Bitcoin';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Bitcoin', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", btcMiktar); // Girilen BTC miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", bitcoinFiyat); // Alım yapılan Bitcoin fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtBtcAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    BitcoinForm_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Bitcoin alım işlemi başarıyla tamamlandı!");
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


        //************************************************************//
        private async void btnBtcSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Bitcoin miktarını al
                decimal btcMiktar = Convert.ToDecimal(txtBtcSat.Text); // Kullanıcıdan alınan Bitcoin miktarı (decimal olarak al)

                decimal bitcoinFiyat = 0;

                // Bitcoin fiyatını API'den almaya çalış
                bitcoinFiyat = await GetBitcoinPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (bitcoinFiyat == 0)
                {
                    bitcoinFiyat = GetLastBitcoinPriceFromDB();

                    if (bitcoinFiyat == 0)
                    {
                        MessageBox.Show("Bitcoin fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = btcMiktar * bitcoinFiyat;

                // Bitcoin bakiyesini kontrol et
                string btcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Bitcoin'";
                decimal btcBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(btcQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            btcBakiye = Convert.ToDecimal(result); // Bitcoin bakiyesini al
                        }
                    }
                }

                // Bitcoin bakiyesi ve toplam tutarı yuvarlayalım (8 haneli hassasiyetle)
                btcBakiye = Math.Round(btcBakiye, 8);
                toplamTutar = Math.Round(toplamTutar, 8);

                // Yetersiz bakiye kontrolü
                if (btcBakiye >= btcMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
    BEGIN TRANSACTION;
    -- Bitcoin düşür
    UPDATE Cuzdan
    SET Bakiye = Bakiye - @Miktar
    WHERE KriptoTur = 'Bitcoin';

    -- USDT artır
    UPDATE Cuzdan
    SET Bakiye = Bakiye + @ToplamTutar
    WHERE KriptoTur = 'USDT';

    -- İşlemi kaydet
    INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
    VALUES ('Bitcoin', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

    COMMIT TRANSACTION;
    ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", btcMiktar); // Satılan BTC miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", bitcoinFiyat); // Satış yapılan Bitcoin fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // TextBox'ı boşalt
                    txtBtcSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    BitcoinForm_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Bitcoin satış işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz bakiye durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Bitcoin bakiyesi! Mevcut bakiyeniz: {btcBakiye}, Gerekli miktar: {btcMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        //************************************************************//
        // Veritabanından son alınan Bitcoin fiyatını çekmek için bir fonksiyon
        private decimal GetLastBitcoinPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Bitcoin' ORDER BY IslemID DESC"; // Son işlem fiyatını al

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
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
    public partial class EthereumPaneli : Form
    {
        public EthereumPaneli()
        {
            InitializeComponent();
            // Büyütme ve küçültme butonlarını kaldır
            this.MaximizeBox = false; // Büyütme butonunu kaldır
            this.MinimizeBox = false; // Küçültme butonunu kaldır
        }

        private async void EthereumPaneli_Load(object sender, EventArgs e)
        {
            btnEthFiyatYenile.PerformClick();
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";
            // Ethereum bakiyesi için SQL sorgusu
            string EthQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Ethereum'";

            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Bağlantıyı aç

                    // Ethereum bakiyesi sorgusu
                    using (SqlCommand cmd = new SqlCommand(EthQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            lblEthBakiye.Text = $"Ethereum Bakiyesi: {Convert.ToDecimal(result):0.########}";
                        }
                        else
                        {
                            lblEthBakiye.Text = "Ethereum Bakiyesi: 0";
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
        // ethereum fiyatını API üzerinden çekme
        // ethereum fiyatını API'den çekerken yuvarlama yapılması
        private async Task<decimal> GetethereumPriceFromAPI()
        {
            try
            {
                using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
               
                {
                    string url = "https://api.coingecko.com/api/v3/simple/price?ids=ethereum&vs_currencies=usd";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject prices = JObject.Parse(responseBody);


                        decimal ethereumFiyat = Convert.ToDecimal(prices["ethereum"]?["usd"]?.ToString());


                        // ethereum fiyatını ekrana yaz
                        lblEthereum.Text = $"Ethereum: ${ethereumFiyat:0.##}";

                        return ethereumFiyat;
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
       


        private async void btnEthFiyatYenile_Click(object sender, EventArgs e)
        {
            await GetethereumPriceFromAPI();
        }

        private void lblEthAl_Click(object sender, EventArgs e)
        {

        }

        private void lblEthereum_Click(object sender, EventArgs e)
        {

        }

        private async void btnEthAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği Ethereum miktarını al
                decimal EthMiktar = Convert.ToDecimal(txtEthAl.Text); // Kullanıcıdan alınan Ethereum miktarı (decimal olarak al)

                decimal EthereumFiyat = 0;

                // Ethereum fiyatını API'den almaya çalış
                EthereumFiyat = await GetethereumPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (EthereumFiyat == 0)
                {
                    EthereumFiyat = GetLastEthereumPriceFromDB();

                    if (EthereumFiyat == 0)
                    {
                        MessageBox.Show("Ethereum fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Alım yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = EthMiktar * EthereumFiyat;

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

        -- Ethereum artır (girilen miktar kadar ETH eklenecek)
        UPDATE Cuzdan
        SET Bakiye = Bakiye + @Miktar
        WHERE KriptoTur = 'Ethereum';

        -- İşlemi kaydet
        INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
        VALUES ('Ethereum', 'Al', @Miktar, @BirimFiyat, @ToplamTutar);

        COMMIT TRANSACTION;
        ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", EthMiktar); // Girilen ETH miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", EthereumFiyat); // Alım yapılan Ethereum fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TextBox'ı boşalt
                    txtEthAl.Clear();

                    // Alım işleminden sonra bakiyeleri güncelle
                    EthereumPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Ethereum alım işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz bakiye durumunda mesaj göster
                    MessageBox.Show($"Yetersiz USDT bakiyesi! Mevcut bakiyeniz: {usdtBakiye}, Gerekli tutar: {toplamTutar}");
                    // TextBox'ı boşalt
                    txtEthAl.Clear();
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

      


        private async void btnEthSat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği ethereum miktarını al
                decimal EthMiktar = Convert.ToDecimal(txtEthMiktarSat.Text); // Kullanıcıdan alınan ethereum miktarı (decimal olarak al)

                decimal EthereumFiyat = 0;

                // Bitcoin fiyatını API'den almaya çalış
                EthereumFiyat = await GetethereumPriceFromAPI();

                // Eğer API'den alınan fiyat 0 ise, veritabanından fiyat al
                if (EthereumFiyat == 0)
                {
                    EthereumFiyat = GetLastEthereumPriceFromDB();

                    if (EthereumFiyat == 0)
                    {
                        MessageBox.Show("Ethereum fiyatı alınamadı ve veritabanında fiyat bilgisi yok.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("API'den fiyat alınamadı, veritabanından son fiyat kullanıldı.");
                    }
                }

                // Satış yapılacak toplam USDT tutarını hesapla
                decimal toplamTutar = EthMiktar * EthereumFiyat;

                // Ethereum bakiyesini kontrol et
                string EthQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Ethereum'";
                decimal EthBakiye = 0;

                string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(EthQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            EthBakiye = Convert.ToDecimal(result); // Ethereum bakiyesini al
                        }
                    }
                }

                // Ethereum bakiyesi ve toplam tutarı yuvarlayalım (8 haneli hassasiyetle)
                EthBakiye = Math.Round(EthBakiye, 8);
                toplamTutar = Math.Round(toplamTutar, 8);

                // Yetersiz bakiye kontrolü
                if (EthBakiye >= EthMiktar)
                {
                    // Satış işlemi için SQL sorgusu
                    string query = @"
    BEGIN TRANSACTION;
    -- Bitcoin düşür
    UPDATE Cuzdan
    SET Bakiye = Bakiye - @Miktar
    WHERE KriptoTur = 'Ethereum';

    -- USDT artır
    UPDATE Cuzdan
    SET Bakiye = Bakiye + @ToplamTutar
    WHERE KriptoTur = 'USDT';

    -- İşlemi kaydet
    INSERT INTO Islemler (KriptoTur, IslemTur, Miktar, BirimFiyat, Toplam)
    VALUES ('Ethereum', 'Sat', @Miktar, @BirimFiyat, @ToplamTutar);

    COMMIT TRANSACTION;
    ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametreleri doğru şekilde ekleyin
                            cmd.Parameters.AddWithValue("@Miktar", EthMiktar); // Satılan Eth miktarı
                            cmd.Parameters.AddWithValue("@BirimFiyat", EthereumFiyat); // Satış yapılan Eth fiyatı
                            cmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar); // Toplam USDT tutarı

                            // SQL sorgusunu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // TextBox'ı boşalt
                    txtEthMiktarSat.Clear();

                    // Satış işleminden sonra bakiyeleri güncelle
                    EthereumPaneli_Load(sender, e); // Bakiyeleri yeniden yükle

                    MessageBox.Show("Ethereum satış işlemi başarıyla tamamlandı!");
                }
                else
                {
                    // Yetersiz bakiye durumunda mesaj göster
                    MessageBox.Show($"Yetersiz Ethereum bakiyesi! Mevcut bakiyeniz: {EthBakiye}, Gerekli miktar: {EthMiktar}");
                }
            }
            catch (Exception ex)
            {
                // Daha ayrıntılı hata mesajı
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        // Veritabanından son alınan Ethereum fiyatını çekmek için bir fonksiyon
        private decimal GetLastEthereumPriceFromDB()
        {
            decimal lastPrice = 0;
            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            string query = "SELECT TOP 1 BirimFiyat FROM Islemler WHERE KriptoTur = 'Ethereum' ORDER BY IslemID DESC"; // Son işlem fiyatını al

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

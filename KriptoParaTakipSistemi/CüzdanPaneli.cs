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
    public partial class CüzdanPaneli : Form
    {
        string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";
        public CüzdanPaneli()
        {
            InitializeComponent();
        }

        //************************************************************//

        private async void CüzdanPaneli_Load(object sender, EventArgs e)
        {

            string connectionString = "Server=DMR-ERDINC;Database=Cüzdan1;Integrated Security=True;";

            // Bitcoin bakiyesi için SQL sorgusu
            string btcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Bitcoin'";
            // Dogecoin bakiyesi için SQL sorgusu
            string dogeQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Dogecoin'";
            // Ethereum bakiyesi için SQL sorgusu
            string EthQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Ethereum'";
            // Litecoin bakiyesi için SQL sorgusu
            string LtcQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Litecoin'";
            // Solana bakiyesi için SQL sorgusu
            string SolQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Solana'";
            // Uniswap bakiyesi için SQL sorgusu
            string UniQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Uniswap'";
            // USDT bakiyesi için SQL sorgusu
            string usdtQuery = "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'";

            string apiUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum,dogecoin,solana,litecoin,uniswap,tether&vs_currencies=usd";
           
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Bakiyeleri al
                    decimal btcBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Bitcoin'");
                    decimal ethBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Ethereum'");
                    decimal ltcBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Litecoin'");
                    decimal dogeBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Dogecoin'");
                    decimal solBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Solana'");
                    decimal uniBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Uniswap'");
                    decimal usdtBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'");

                    // API'den fiyatları al
                    HttpClient client = new HttpClient();
                    string response = await client.GetStringAsync(apiUrl);
                    JObject json = JObject.Parse(response);

                    Dictionary<string, decimal> fiyatlar = new Dictionary<string, decimal>
                    {
                        { "Bitcoin", (decimal)json["bitcoin"]["usd"] },
                        { "Ethereum", (decimal)json["ethereum"]["usd"] },
                        { "Dogecoin", (decimal)json["dogecoin"]["usd"] },
                        { "Solana", (decimal)json["solana"]["usd"] },
                        { "Litecoin", (decimal)json["litecoin"]["usd"] },
                        { "Uniswap", (decimal)json["uniswap"]["usd"] },
                        { "USDT", (decimal)json["tether"]["usd"] },
                       
                    };

                    // Fiyatları güncelle veya ekle
                    foreach (var item in fiyatlar)
                    {
                        UpdateFiyat(conn, item.Key, item.Value);
                    }


                    // Değer Kısmını Gösterir 
                    lblBtcBakiye.Text = $"{btcBakiye:0.##} BTC";
                    lblEthBakiye.Text = $"{ethBakiye:0.##} ETH";
                    lblLtcBakiye.Text = $"{ltcBakiye:0.##} LTC";
                    lblDogeBakiye.Text = $"{dogeBakiye:0.##} DOGE";
                    lblSolanaBakiye.Text = $"{solBakiye:0.##} SOL";
                    lblUniswapBakiye.Text = $"{uniBakiye:0.##} UNİ";
                    lblUsdtBakiye.Text = $"{usdtBakiye:0.##} USDT";
                    // Miktar Kısmını Gösterir API Alınmazsa
                    lblBtcDeğer.Text = $"{fiyatlar["Bitcoin"] * btcBakiye:0.##} USDT";
                    lblEthDeğer.Text = $"{fiyatlar["Ethereum"] * ethBakiye:0.##} USDT";
                    lblLtcDeğer.Text = $"{fiyatlar["Litecoin"] * ltcBakiye:0.##} USDT";
                    lblDogeDeğer.Text = $"{fiyatlar["Dogecoin"] * dogeBakiye:0.##} USDT";
                    lblSolDeğer.Text = $"{fiyatlar["Solana"] * solBakiye:0.##} USDT";
                    lblUniDeğer.Text = $"{fiyatlar["Uniswap"] * uniBakiye:0.##} USDT";
                    lblUsdtBakiye2.Text = $"{usdtBakiye:0.##} USDT";

                    // Toplam Değer
                    decimal toplamDeger = (fiyatlar["Bitcoin"] * btcBakiye) +
                                          (fiyatlar["Ethereum"] * ethBakiye) +
                                          (fiyatlar["Litecoin"] * ltcBakiye) +
                                          (fiyatlar["Dogecoin"] * dogeBakiye) +
                                          (fiyatlar["Solana"] * solBakiye) +
                                          (fiyatlar["Uniswap"] * uniBakiye) +
                                          (usdtBakiye * fiyatlar["USDT"]);

                    lblToplamDeğer.Text = $"{toplamDeger:0.##} USDT";


                   
                }
            }
            catch
            {
                // API başarısız olursa eski fiyatları kullan
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    decimal btcFiyat = FiyatAl(conn, "Bitcoin");
                    decimal ethFiyat = FiyatAl(conn, "Ethereum");
                    decimal ltcFiyat = FiyatAl(conn, "Litecoin");
                    decimal dogeFiyat = FiyatAl(conn, "Dogecoin");
                    decimal solFiyat = FiyatAl(conn, "Solana");
                    decimal uniFiyat = FiyatAl(conn, "Uniswap");
                    decimal usdtFiyat = FiyatAl(conn, "USDT");

                    // Toplam Değeri Hesapla API Alınmazsa
                    decimal btcBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Bitcoin'");
                    decimal ethBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Ethereum'");
                    decimal ltcBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Litecoin'");
                    decimal dogeBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Dogecoin'");
                    decimal solBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Solana'");
                    decimal uniBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'Uniswap'");
                    decimal usdtBakiye = BakiyeyiAl(conn, "SELECT Bakiye FROM Cuzdan WHERE KriptoTur = 'USDT'");

                    decimal toplamDeger = (btcFiyat * btcBakiye) +
                                          (ethFiyat * ethBakiye) +
                                          (ltcFiyat * ltcBakiye) +
                                          (dogeFiyat * dogeBakiye) +
                                          (solFiyat * solBakiye) +
                                          (uniFiyat * uniBakiye) +
                                          (usdtBakiye * usdtFiyat);

                    lblToplamDeğer.Text = $"{toplamDeger:0.##} USD";



                    // Değer Kısmını Gösterir API Alınmazsa
                    lblBtcDeğer.Text = (btcFiyat == 0) ? $"{btcBakiye:0.##} BTC" : $"{btcFiyat * btcBakiye:0.##} USDT";
                    lblEthDeğer.Text = (ethFiyat == 0) ? $"{ethBakiye:0.##} ETH" : $"{ethFiyat * ethBakiye:0.##} USDT";
                    lblLtcDeğer.Text = (ltcFiyat == 0) ? $"{ltcBakiye:0.##} LTC" : $"{ltcFiyat * ltcBakiye:0.##} USDT";
                    lblDogeDeğer.Text = (dogeFiyat == 0) ? $"{dogeBakiye:0.##} DOGE" : $"{dogeFiyat * dogeBakiye:0.##} USDT";
                    lblSolDeğer.Text = (solFiyat == 0) ? $"{solBakiye:0.##} SOL" : $"{solFiyat * solBakiye:0.##} USDT";
                    lblUniDeğer.Text = (uniFiyat == 0) ? $"{uniBakiye:0.##} UNI" : $"{uniFiyat * uniBakiye:0.##} USDT";
                    lblUsdtBakiye.Text = (usdtFiyat == 0) ? $"{usdtBakiye:0.##} USDT" : $"{usdtFiyat * usdtBakiye:0.##} USDT";

                    // Miktar Kısmını Gösterir API Alınmazsa
                    lblBtcBakiye.Text = $"{btcBakiye:0.##} BTC";
                    lblEthBakiye.Text = $"{ethBakiye:0.##} ETH";
                    lblLtcBakiye.Text = $"{ltcBakiye:0.##} LTC";
                    lblDogeBakiye.Text = $"{dogeBakiye:0.##} DOGE";
                    lblSolanaBakiye.Text = $"{solBakiye:0.##} SOL";
                    lblUniswapBakiye.Text = $"{uniBakiye:0.##} UNI";
                    lblUsdtBakiye2.Text = $"{usdtBakiye:0.##} USDT";

                }
            }

        }

        //************************************************************//

        // Bakiyeleri çekmek için yardımcı metot
        private decimal BakiyeyiAl(SqlConnection conn, string bakiyeSorgu)
        {
            using (SqlCommand cmd = new SqlCommand(bakiyeSorgu, conn))
            {
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDecimal(result);
                }
                return 0;
            }
        }

        //************************************************************//
        private void UpdateFiyat(SqlConnection conn, string kriptoTur, decimal fiyat)
        {
            string sorgu = @"
                IF EXISTS (SELECT 1 FROM KriptoFiyatlari WHERE KriptoTur = @KriptoTur)
                    UPDATE KriptoFiyatlari SET Fiyat = @Fiyat WHERE KriptoTur = @KriptoTur
                ELSE
                    INSERT INTO KriptoFiyatlari (KriptoTur, Fiyat) VALUES (@KriptoTur, @Fiyat)";
            using (SqlCommand cmd = new SqlCommand(sorgu, conn))
            {
                cmd.Parameters.AddWithValue("@KriptoTur", kriptoTur);
                cmd.Parameters.AddWithValue("@Fiyat", fiyat);
                cmd.ExecuteNonQuery();
            }
        }
        //************************************************************//
        private decimal FiyatAl(SqlConnection conn, string kriptoTur)
        {
            string sorgu = "SELECT Fiyat FROM KriptoFiyatlari WHERE KriptoTur = @KriptoTur";
            using (SqlCommand cmd = new SqlCommand(sorgu, conn))
            {
                cmd.Parameters.AddWithValue("@KriptoTur", kriptoTur);
                object result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }
    }
}

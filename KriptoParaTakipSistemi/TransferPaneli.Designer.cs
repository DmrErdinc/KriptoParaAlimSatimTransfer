namespace KriptoParaTakipSistemi
{
    partial class TransferPaneli
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransferPaneli));
            this.btnGönder = new System.Windows.Forms.Button();
            this.txtMiktar = new System.Windows.Forms.TextBox();
            this.txtAğAdres = new System.Windows.Forms.TextBox();
            this.cmbAğ = new System.Windows.Forms.ComboBox();
            this.cmbCoin = new System.Windows.Forms.ComboBox();
            this.lblAğAdres = new System.Windows.Forms.Label();
            this.lblTransferMiktar = new System.Windows.Forms.Label();
            this.lblAğ = new System.Windows.Forms.Label();
            this.lblCoin = new System.Windows.Forms.Label();
            this.bunifuPictureBox3 = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.bunifuPictureBox1 = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.lblBilgi = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuPictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGönder
            // 
            this.btnGönder.BackColor = System.Drawing.Color.LightGray;
            this.btnGönder.BackgroundImage = global::KriptoParaTakipSistemi.Properties.Resources.peer_to_peer;
            this.btnGönder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnGönder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGönder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGönder.Location = new System.Drawing.Point(560, 222);
            this.btnGönder.Name = "btnGönder";
            this.btnGönder.Size = new System.Drawing.Size(68, 64);
            this.btnGönder.TabIndex = 20;
            this.btnGönder.UseVisualStyleBackColor = false;
            this.btnGönder.Click += new System.EventHandler(this.btnGönder_Click);
            // 
            // txtMiktar
            // 
            this.txtMiktar.Location = new System.Drawing.Point(463, 263);
            this.txtMiktar.Name = "txtMiktar";
            this.txtMiktar.Size = new System.Drawing.Size(91, 20);
            this.txtMiktar.TabIndex = 19;
            // 
            // txtAğAdres
            // 
            this.txtAğAdres.Location = new System.Drawing.Point(463, 197);
            this.txtAğAdres.Name = "txtAğAdres";
            this.txtAğAdres.Size = new System.Drawing.Size(165, 20);
            this.txtAğAdres.TabIndex = 18;
            // 
            // cmbAğ
            // 
            this.cmbAğ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAğ.FormattingEnabled = true;
            this.cmbAğ.Items.AddRange(new object[] {
            "ERC-20",
            "TRC-20",
            "BEP-20",
            "SPL",
            "C-Chain",
            "MATİC",
            "Layer-2"});
            this.cmbAğ.Location = new System.Drawing.Point(463, 229);
            this.cmbAğ.Name = "cmbAğ";
            this.cmbAğ.Size = new System.Drawing.Size(91, 21);
            this.cmbAğ.TabIndex = 17;
            // 
            // cmbCoin
            // 
            this.cmbCoin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoin.FormattingEnabled = true;
            this.cmbCoin.Items.AddRange(new object[] {
            "Bitcoin",
            "Ethereum",
            "Litecoin",
            "Dogecoin",
            "Solana",
            "Uniswap"});
            this.cmbCoin.Location = new System.Drawing.Point(463, 164);
            this.cmbCoin.Name = "cmbCoin";
            this.cmbCoin.Size = new System.Drawing.Size(165, 21);
            this.cmbCoin.TabIndex = 16;
            // 
            // lblAğAdres
            // 
            this.lblAğAdres.AutoSize = true;
            this.lblAğAdres.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAğAdres.ForeColor = System.Drawing.Color.White;
            this.lblAğAdres.Location = new System.Drawing.Point(308, 194);
            this.lblAğAdres.Name = "lblAğAdres";
            this.lblAğAdres.Size = new System.Drawing.Size(149, 21);
            this.lblAğAdres.TabIndex = 15;
            this.lblAğAdres.Text = "Ağ Adresini giriniz:";
            // 
            // lblTransferMiktar
            // 
            this.lblTransferMiktar.AutoSize = true;
            this.lblTransferMiktar.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTransferMiktar.ForeColor = System.Drawing.Color.White;
            this.lblTransferMiktar.Location = new System.Drawing.Point(394, 260);
            this.lblTransferMiktar.Name = "lblTransferMiktar";
            this.lblTransferMiktar.Size = new System.Drawing.Size(63, 21);
            this.lblTransferMiktar.TabIndex = 14;
            this.lblTransferMiktar.Text = "Miktar:";
            // 
            // lblAğ
            // 
            this.lblAğ.AutoSize = true;
            this.lblAğ.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAğ.ForeColor = System.Drawing.Color.White;
            this.lblAğ.Location = new System.Drawing.Point(366, 226);
            this.lblAğ.Name = "lblAğ";
            this.lblAğ.Size = new System.Drawing.Size(91, 21);
            this.lblAğ.TabIndex = 13;
            this.lblAğ.Text = "Ağ seçiniz:";
            // 
            // lblCoin
            // 
            this.lblCoin.AutoSize = true;
            this.lblCoin.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCoin.ForeColor = System.Drawing.Color.White;
            this.lblCoin.Location = new System.Drawing.Point(172, 164);
            this.lblCoin.Name = "lblCoin";
            this.lblCoin.Size = new System.Drawing.Size(285, 21);
            this.lblCoin.TabIndex = 12;
            this.lblCoin.Text = "Tranfer Etmek istediğin Coini seçiniz:";
            // 
            // bunifuPictureBox3
            // 
            this.bunifuPictureBox3.AllowFocused = false;
            this.bunifuPictureBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bunifuPictureBox3.AutoSizeHeight = true;
            this.bunifuPictureBox3.BorderRadius = 0;
            this.bunifuPictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("bunifuPictureBox3.Image")));
            this.bunifuPictureBox3.IsCircle = false;
            this.bunifuPictureBox3.Location = new System.Drawing.Point(0, -2);
            this.bunifuPictureBox3.Name = "bunifuPictureBox3";
            this.bunifuPictureBox3.Size = new System.Drawing.Size(70, 70);
            this.bunifuPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuPictureBox3.TabIndex = 23;
            this.bunifuPictureBox3.TabStop = false;
            this.bunifuPictureBox3.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Custom;
            // 
            // bunifuPictureBox1
            // 
            this.bunifuPictureBox1.AllowFocused = false;
            this.bunifuPictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bunifuPictureBox1.AutoSizeHeight = true;
            this.bunifuPictureBox1.BorderRadius = 45;
            this.bunifuPictureBox1.Image = global::KriptoParaTakipSistemi.Properties.Resources.Transfer;
            this.bunifuPictureBox1.IsCircle = false;
            this.bunifuPictureBox1.Location = new System.Drawing.Point(1, 358);
            this.bunifuPictureBox1.Name = "bunifuPictureBox1";
            this.bunifuPictureBox1.Size = new System.Drawing.Size(91, 91);
            this.bunifuPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuPictureBox1.TabIndex = 21;
            this.bunifuPictureBox1.TabStop = false;
            this.bunifuPictureBox1.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Circle;
            // 
            // lblBilgi
            // 
            this.lblBilgi.AutoSize = true;
            this.lblBilgi.Font = new System.Drawing.Font("Century Gothic", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBilgi.ForeColor = System.Drawing.Color.White;
            this.lblBilgi.Location = new System.Drawing.Point(98, 396);
            this.lblBilgi.Name = "lblBilgi";
            this.lblBilgi.Size = new System.Drawing.Size(680, 19);
            this.lblBilgi.TabIndex = 24;
            this.lblBilgi.Text = "Lütfen Transfer İşlemlerinize Dikkat Ediniz Kaybolan Coinlerden Sitemiz Sorumlu D" +
    "eğildir!!";
            // 
            // TransferPaneli
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(44)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblBilgi);
            this.Controls.Add(this.bunifuPictureBox3);
            this.Controls.Add(this.bunifuPictureBox1);
            this.Controls.Add(this.btnGönder);
            this.Controls.Add(this.txtMiktar);
            this.Controls.Add(this.txtAğAdres);
            this.Controls.Add(this.cmbAğ);
            this.Controls.Add(this.cmbCoin);
            this.Controls.Add(this.lblAğAdres);
            this.Controls.Add(this.lblTransferMiktar);
            this.Controls.Add(this.lblAğ);
            this.Controls.Add(this.lblCoin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TransferPaneli";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TransferPaneli";
            this.Load += new System.EventHandler(this.TransferPaneli_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bunifuPictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuPictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.UI.WinForms.BunifuPictureBox bunifuPictureBox3;
        private Bunifu.UI.WinForms.BunifuPictureBox bunifuPictureBox1;
        private System.Windows.Forms.Button btnGönder;
        private System.Windows.Forms.TextBox txtMiktar;
        private System.Windows.Forms.TextBox txtAğAdres;
        private System.Windows.Forms.ComboBox cmbAğ;
        private System.Windows.Forms.ComboBox cmbCoin;
        private System.Windows.Forms.Label lblAğAdres;
        private System.Windows.Forms.Label lblTransferMiktar;
        private System.Windows.Forms.Label lblAğ;
        private System.Windows.Forms.Label lblCoin;
        private System.Windows.Forms.Label lblBilgi;
    }
}
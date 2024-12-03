namespace KriptoParaTakipSistemi
{
    partial class GirişPaneli
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GirişPaneli));
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtKullanıcıAdı = new System.Windows.Forms.TextBox();
            this.txtŞifre = new System.Windows.Forms.TextBox();
            this.ptcrFoto = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.ptcrAmblem = new System.Windows.Forms.PictureBox();
            this.ptcrFoto2 = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.ptcrFoto3 = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.lblHoşGeldin = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrAmblem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto3)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(85, 190);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Şifre:";
            // 
            // btnLogin
            // 
            this.btnLogin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLogin.BackgroundImage")));
            this.btnLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Location = new System.Drawing.Point(269, 157);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(62, 60);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.TabStop = false;
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(34, 162);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "KullanıcıAdı:";
            // 
            // txtKullanıcıAdı
            // 
            this.txtKullanıcıAdı.AcceptsReturn = true;
            this.txtKullanıcıAdı.AcceptsTab = true;
            this.txtKullanıcıAdı.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtKullanıcıAdı.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKullanıcıAdı.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtKullanıcıAdı.ForeColor = System.Drawing.Color.Black;
            this.txtKullanıcıAdı.Location = new System.Drawing.Point(128, 160);
            this.txtKullanıcıAdı.Name = "txtKullanıcıAdı";
            this.txtKullanıcıAdı.Size = new System.Drawing.Size(133, 24);
            this.txtKullanıcıAdı.TabIndex = 0;
            // 
            // txtŞifre
            // 
            this.txtŞifre.AcceptsReturn = true;
            this.txtŞifre.AcceptsTab = true;
            this.txtŞifre.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtŞifre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtŞifre.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtŞifre.ForeColor = System.Drawing.Color.Black;
            this.txtŞifre.Location = new System.Drawing.Point(128, 190);
            this.txtŞifre.Name = "txtŞifre";
            this.txtŞifre.Size = new System.Drawing.Size(133, 24);
            this.txtŞifre.TabIndex = 1;
            this.txtŞifre.TextChanged += new System.EventHandler(this.txtŞifre_TextChanged);
            // 
            // ptcrFoto
            // 
            this.ptcrFoto.AllowFocused = false;
            this.ptcrFoto.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ptcrFoto.AutoSizeHeight = false;
            this.ptcrFoto.BorderRadius = 45;
            this.ptcrFoto.Image = ((System.Drawing.Image)(resources.GetObject("ptcrFoto.Image")));
            this.ptcrFoto.IsCircle = false;
            this.ptcrFoto.Location = new System.Drawing.Point(2, 0);
            this.ptcrFoto.Name = "ptcrFoto";
            this.ptcrFoto.Size = new System.Drawing.Size(91, 91);
            this.ptcrFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ptcrFoto.TabIndex = 6;
            this.ptcrFoto.TabStop = false;
            this.ptcrFoto.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Circle;
            // 
            // ptcrAmblem
            // 
            this.ptcrAmblem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ptcrAmblem.Dock = System.Windows.Forms.DockStyle.Right;
            this.ptcrAmblem.Image = ((System.Drawing.Image)(resources.GetObject("ptcrAmblem.Image")));
            this.ptcrAmblem.Location = new System.Drawing.Point(339, 0);
            this.ptcrAmblem.Name = "ptcrAmblem";
            this.ptcrAmblem.Size = new System.Drawing.Size(76, 224);
            this.ptcrAmblem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptcrAmblem.TabIndex = 8;
            this.ptcrAmblem.TabStop = false;
            // 
            // ptcrFoto2
            // 
            this.ptcrFoto2.AllowFocused = false;
            this.ptcrFoto2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ptcrFoto2.AutoSizeHeight = false;
            this.ptcrFoto2.BorderRadius = 0;
            this.ptcrFoto2.Image = ((System.Drawing.Image)(resources.GetObject("ptcrFoto2.Image")));
            this.ptcrFoto2.IsCircle = false;
            this.ptcrFoto2.Location = new System.Drawing.Point(2, 160);
            this.ptcrFoto2.Name = "ptcrFoto2";
            this.ptcrFoto2.Size = new System.Drawing.Size(24, 24);
            this.ptcrFoto2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ptcrFoto2.TabIndex = 10;
            this.ptcrFoto2.TabStop = false;
            this.ptcrFoto2.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Custom;
            // 
            // ptcrFoto3
            // 
            this.ptcrFoto3.AllowFocused = false;
            this.ptcrFoto3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ptcrFoto3.AutoSizeHeight = false;
            this.ptcrFoto3.BorderRadius = 0;
            this.ptcrFoto3.Image = ((System.Drawing.Image)(resources.GetObject("ptcrFoto3.Image")));
            this.ptcrFoto3.IsCircle = false;
            this.ptcrFoto3.Location = new System.Drawing.Point(2, 190);
            this.ptcrFoto3.Name = "ptcrFoto3";
            this.ptcrFoto3.Size = new System.Drawing.Size(24, 24);
            this.ptcrFoto3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ptcrFoto3.TabIndex = 11;
            this.ptcrFoto3.TabStop = false;
            this.ptcrFoto3.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Custom;
            // 
            // lblHoşGeldin
            // 
            this.lblHoşGeldin.AutoSize = true;
            this.lblHoşGeldin.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHoşGeldin.ForeColor = System.Drawing.Color.Black;
            this.lblHoşGeldin.Location = new System.Drawing.Point(99, 40);
            this.lblHoşGeldin.Name = "lblHoşGeldin";
            this.lblHoşGeldin.Size = new System.Drawing.Size(138, 19);
            this.lblHoşGeldin.TabIndex = 12;
            this.lblHoşGeldin.Text = "Hoşgeldin Erdinç";
            // 
            // GirişPaneli
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(75)))), ((int)(((byte)(105)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(415, 224);
            this.Controls.Add(this.lblHoşGeldin);
            this.Controls.Add(this.ptcrFoto3);
            this.Controls.Add(this.ptcrFoto2);
            this.Controls.Add(this.ptcrAmblem);
            this.Controls.Add(this.ptcrFoto);
            this.Controls.Add(this.txtŞifre);
            this.Controls.Add(this.txtKullanıcıAdı);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLogin);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimizeBox = false;
            this.Name = "GirişPaneli";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BinanceTR";
            this.TransparencyKey = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.GirişForm_Load);
            this.Enter += new System.EventHandler(this.btnLogin_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrAmblem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptcrFoto3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKullanıcıAdı;
        private System.Windows.Forms.TextBox txtŞifre;
        private Bunifu.UI.WinForms.BunifuPictureBox ptcrFoto;
        private System.Windows.Forms.PictureBox ptcrAmblem;
        private Bunifu.UI.WinForms.BunifuPictureBox ptcrFoto2;
        private Bunifu.UI.WinForms.BunifuPictureBox ptcrFoto3;
        private System.Windows.Forms.Label lblHoşGeldin;
    }
}
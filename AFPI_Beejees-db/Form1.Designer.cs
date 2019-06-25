namespace AFPI_Beejees_db
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.txtUN = new System.Windows.Forms.TextBox();
            this.txtPW = new System.Windows.Forms.TextBox();
            this.label62 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtUN
            // 
            this.txtUN.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtUN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUN.Font = new System.Drawing.Font("Google Sans", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUN.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtUN.Location = new System.Drawing.Point(108, 349);
            this.txtUN.Name = "txtUN";
            this.txtUN.Size = new System.Drawing.Size(302, 41);
            this.txtUN.TabIndex = 2;
            this.txtUN.Text = "NTASA";
            this.txtUN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // txtPW
            // 
            this.txtPW.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtPW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPW.Font = new System.Drawing.Font("Google Sans", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPW.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtPW.Location = new System.Drawing.Point(108, 450);
            this.txtPW.Name = "txtPW";
            this.txtPW.PasswordChar = '●';
            this.txtPW.Size = new System.Drawing.Size(302, 41);
            this.txtPW.TabIndex = 3;
            this.txtPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("Google Sans", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label62.Location = new System.Drawing.Point(103, 142);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(141, 26);
            this.label62.TabIndex = 119;
            this.label62.Text = "Server Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Google Sans", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(103, 320);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 26);
            this.label2.TabIndex = 121;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Google Sans", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(103, 421);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 26);
            this.label3.TabIndex = 122;
            this.label3.Text = "Password:";
            // 
            // buttonLogin
            // 
            this.buttonLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogin.Location = new System.Drawing.Point(183, 612);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(133, 45);
            this.buttonLogin.TabIndex = 123;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Google Sans", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(103, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 26);
            this.label1.TabIndex = 120;
            this.label1.Text = "DB Name:";
            // 
            // checkBox
            // 
            this.checkBox.AutoSize = true;
            this.checkBox.Location = new System.Drawing.Point(148, 509);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(224, 29);
            this.checkBox.TabIndex = 125;
            this.checkBox.Text = "Integrated Security";
            this.checkBox.UseVisualStyleBackColor = true;
            // 
            // txtSN
            // 
            this.txtSN.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtSN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSN.Font = new System.Drawing.Font("Google Sans", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSN.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtSN.Location = new System.Drawing.Point(108, 171);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(302, 41);
            this.txtSN.TabIndex = 0;
            this.txtSN.Text = "172.18.9.69";
            this.txtSN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // txtDB
            // 
            this.txtDB.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtDB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDB.Font = new System.Drawing.Font("Google Sans", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDB.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtDB.Location = new System.Drawing.Point(108, 259);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(302, 41);
            this.txtDB.TabIndex = 1;
            this.txtDB.Text = "AFPI-BEEJEES-DB";
            this.txtDB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(4, 676);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 33);
            this.button1.TabIndex = 126;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(124, 556);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(138, 29);
            this.radioButton1.TabIndex = 127;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "BEEJEES";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(296, 556);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(86, 29);
            this.radioButton2.TabIndex = 128;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "BCS";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 712);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSN);
            this.Controls.Add(this.txtDB);
            this.Controls.Add(this.checkBox);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label62);
            this.Controls.Add(this.txtUN);
            this.Controls.Add(this.txtPW);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Text = "LOGIN SERVER";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtUN;
        private System.Windows.Forms.TextBox txtPW;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}
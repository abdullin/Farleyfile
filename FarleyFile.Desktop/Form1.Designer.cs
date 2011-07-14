namespace FarleyFile
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this._input = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this._panel = new System.Windows.Forms.Panel();
            this._rich = new System.Windows.Forms.RichTextBox();
            this._status = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._input);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 464);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 72);
            this.panel1.TabIndex = 0;
            // 
            // textBox1
            // 
            this._input.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._input.Dock = System.Windows.Forms.DockStyle.Fill;
            this._input.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._input.Location = new System.Drawing.Point(0, 0);
            this._input.Multiline = true;
            this._input.Name = "_input";
            this._input.Size = new System.Drawing.Size(740, 72);
            this._input.TabIndex = 0;
            this._input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 461);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(740, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this._panel.Controls.Add(this._rich);
            this._panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panel.Location = new System.Drawing.Point(0, 0);
            this._panel.Name = "_panel";
            this._panel.Padding = new System.Windows.Forms.Padding(5);
            this._panel.Size = new System.Drawing.Size(740, 461);
            this._panel.TabIndex = 2;
            // 
            // _rich
            // 
            this._rich.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._rich.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rich.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._rich.Location = new System.Drawing.Point(5, 5);
            this._rich.Name = "_rich";
            this._rich.ReadOnly = true;
            this._rich.Size = new System.Drawing.Size(730, 451);
            this._rich.TabIndex = 0;
            this._rich.Text = "";
            // 
            // label1
            // 
            this._status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._status.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._status.Location = new System.Drawing.Point(0, 536);
            this._status.Name = "_status";
            this._status.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._status.Size = new System.Drawing.Size(740, 23);
            this._status.TabIndex = 1;
            this._status.Text = "Farley File";
            this._status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 559);
            this.Controls.Add(this._panel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._status);
            this.Name = "Form1";
            this.Text = "FarleyFile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox _input;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel _panel;
        private System.Windows.Forms.RichTextBox _rich;
        private System.Windows.Forms.Label _status;
    }
}


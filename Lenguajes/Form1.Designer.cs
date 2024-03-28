
namespace Lenguajes
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            printDialog1 = new PrintDialog();
            lb_Titulo = new Label();
            lb_lugarA = new Label();
            cb_lugarA = new ComboBox();
            cb_lugarB = new ComboBox();
            lb_lugarB = new Label();
            btn_viajar = new Button();
            btn_salir = new Button();
            lb_obstaculos = new Label();
            numericUpDown1 = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // printDialog1
            // 
            printDialog1.UseEXDialog = true;
            // 
            // lb_Titulo
            // 
            lb_Titulo.AutoSize = true;
            lb_Titulo.BackColor = Color.Transparent;
            lb_Titulo.Font = new Font("Elephant", 28.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lb_Titulo.Location = new Point(306, 151);
            lb_Titulo.Name = "lb_Titulo";
            lb_Titulo.Size = new Size(600, 60);
            lb_Titulo.TabIndex = 0;
            lb_Titulo.Text = "Tegucigalpa Pathfinder";
            // 
            // lb_lugarA
            // 
            lb_lugarA.AutoSize = true;
            lb_lugarA.BackColor = Color.Transparent;
            lb_lugarA.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lb_lugarA.Location = new Point(372, 296);
            lb_lugarA.Name = "lb_lugarA";
            lb_lugarA.Size = new Size(127, 38);
            lb_lugarA.TabIndex = 1;
            lb_lugarA.Text = "Lugar A:";
            // 
            // cb_lugarA
            // 
            cb_lugarA.FormattingEnabled = true;
            cb_lugarA.Items.AddRange(new object[] { "UNITEC", "LA COLONIA", "CASA", "MALL", "AEROPUERTO" });
            cb_lugarA.Location = new Point(503, 305);
            cb_lugarA.Margin = new Padding(3, 4, 3, 4);
            cb_lugarA.Name = "cb_lugarA";
            cb_lugarA.Size = new Size(204, 28);
            cb_lugarA.TabIndex = 2;
            // 
            // cb_lugarB
            // 
            cb_lugarB.FormattingEnabled = true;
            cb_lugarB.Items.AddRange(new object[] { "UNITEC", "LA COLONIA", "CASA", "MALL", "AEROPUERTO" });
            cb_lugarB.Location = new Point(503, 399);
            cb_lugarB.Margin = new Padding(3, 4, 3, 4);
            cb_lugarB.Name = "cb_lugarB";
            cb_lugarB.Size = new Size(204, 28);
            cb_lugarB.TabIndex = 4;
            cb_lugarB.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // lb_lugarB
            // 
            lb_lugarB.AutoSize = true;
            lb_lugarB.BackColor = Color.Transparent;
            lb_lugarB.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lb_lugarB.ForeColor = Color.Black;
            lb_lugarB.Location = new Point(372, 390);
            lb_lugarB.Name = "lb_lugarB";
            lb_lugarB.Size = new Size(125, 38);
            lb_lugarB.TabIndex = 3;
            lb_lugarB.Text = "Lugar B:";
            // 
            // btn_viajar
            // 
            btn_viajar.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_viajar.Location = new Point(390, 578);
            btn_viajar.Margin = new Padding(3, 4, 3, 4);
            btn_viajar.Name = "btn_viajar";
            btn_viajar.Size = new Size(421, 73);
            btn_viajar.TabIndex = 5;
            btn_viajar.Text = "Viajar";
            btn_viajar.UseVisualStyleBackColor = true;
            btn_viajar.Click += Btn_viajar_Click;
            // 
            // btn_salir
            // 
            btn_salir.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_salir.Location = new Point(390, 703);
            btn_salir.Margin = new Padding(3, 4, 3, 4);
            btn_salir.Name = "btn_salir";
            btn_salir.Size = new Size(421, 73);
            btn_salir.TabIndex = 6;
            btn_salir.Text = "Salir";
            btn_salir.UseVisualStyleBackColor = true;
            btn_salir.Click += btnExit_Click;
            // 
            // lb_obstaculos
            // 
            lb_obstaculos.AutoSize = true;
            lb_obstaculos.BackColor = Color.Transparent;
            lb_obstaculos.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lb_obstaculos.ForeColor = Color.Black;
            lb_obstaculos.Location = new Point(264, 471);
            lb_obstaculos.Name = "lb_obstaculos";
            lb_obstaculos.Size = new Size(233, 38);
            lb_obstaculos.TabIndex = 7;
            lb_obstaculos.Text = "# de Obstaculos:";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(503, 482);
            numericUpDown1.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(204, 27);
            numericUpDown1.TabIndex = 8;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Red;
            BackgroundImage = Properties.Resources.TegusSketch;
            ClientSize = new Size(1146, 835);
            Controls.Add(numericUpDown1);
            Controls.Add(lb_obstaculos);
            Controls.Add(btn_salir);
            Controls.Add(btn_viajar);
            Controls.Add(cb_lugarB);
            Controls.Add(lb_lugarB);
            Controls.Add(cb_lugarA);
            Controls.Add(lb_lugarA);
            Controls.Add(lb_Titulo);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Pantalla Inicial";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private PrintDialog printDialog1;
        private Label lb_Titulo;
        private Label lb_lugarA;
        private ComboBox cb_lugarA;
        private ComboBox cb_lugarB;
        private Label lb_lugarB;
        private Button btn_viajar;
        private Button btn_salir;
        private Label lb_obstaculos;
        private NumericUpDown numericUpDown1;
    }
}

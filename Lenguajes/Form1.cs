namespace Lenguajes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        private void Btn_viajar_Click(object sender, EventArgs e)
        {
            if(cb_lugarA.SelectedIndex == -1 || cb_lugarB.SelectedIndex == -1)
            {
                /*
                MessageBox.Show(this, "Porfavor elegir el punto de partida y de destino apropiados"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);*/

            }
            else
            {
                string start = (string)cb_lugarA.SelectedItem;
                string end = (string)cb_lugarB.SelectedItem;
                int obstacles = (int)numericUpDown1.Value;

                Form2 form2 = new Form2(start, end, obstacles);
                form2.Show();
                this.Hide();
            }


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close(); // Cierra el formulario actual
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}

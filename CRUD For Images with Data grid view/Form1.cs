using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace CRUD_For_Images_with_Data_grid_view
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;Initial Catalog=crudimages;username=root; password=");
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.JPG;*.PNG;*.GIF)|*.jpg;*.png;*.gif";

            if(opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillDGV("");
        }

        public void FillDGV( string valueToSearch)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `image` WHERE CONCAT (`ID`,`Name`,`Description`) LIKE '%" + valueToSearch + "%'", con);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataTable tbl = new DataTable();
            adp.Fill(tbl);

            dataGridView1.RowTemplate.Height = 60;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.DataSource = tbl;

            DataGridViewImageColumn imgcol = new DataGridViewImageColumn();
            imgcol = (DataGridViewImageColumn)dataGridView1.Columns[3];
            imgcol.ImageLayout = DataGridViewImageCellLayout.Stretch;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Byte[] img = (Byte[])dataGridView1.CurrentRow.Cells[3].Value;

            MemoryStream ms = new MemoryStream(img);

            pictureBox1.Image = Image.FromStream(ms);

            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();

        }
        public void ExecMyQuery(MySqlCommand mcomd, string myMsg)
        {
            con.Open();
            if (mcomd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show(myMsg);
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }
            con.Close();

            FillDGV("");
        }

        private void InsertBtn_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
            Byte[] img = ms.ToArray();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO `image`(`ID`, `Name`, `Description`, `Image`) VALUES (@id,@name,@desc,@img)", con);
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBox1.Text;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = textBox2.Text;
            cmd.Parameters.Add("@desc", MySqlDbType.VarChar).Value = textBox3.Text;
            cmd.Parameters.Add("@img", MySqlDbType.Blob).Value = img;

            ExecMyQuery(cmd, "Data Inserted!");
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {

            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
            Byte[] img = ms.ToArray();

            MySqlCommand cmd = new MySqlCommand("UPDATE `image` SET `Name`=@name,`Description`=@desc,`Image`=@img WHERE `ID`= @id", con);
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBox1.Text;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = textBox2.Text;
            cmd.Parameters.Add("@desc", MySqlDbType.VarChar).Value = textBox3.Text;
            cmd.Parameters.Add("@img", MySqlDbType.Blob).Value = img;

            ExecMyQuery(cmd, "Data Updated!");

        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {

            MySqlCommand cmd = new MySqlCommand("DELETE FROM `image` WHERE `ID`= @id", con);
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBox1.Text;
         

            ExecMyQuery(cmd, "Data Deleted!");
            clearFields();

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            FillDGV(textBox4.Text);
        }

        private void btn_Find_Click(object sender, EventArgs e)
        {

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `image` WHERE ID= @id", con);
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBox1.Text;

            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataTable tbl = new DataTable();
            adp.Fill(tbl);

            if (tbl.Rows.Count <=0)
            {
                MessageBox.Show("No data Found");
                clearFields();
            }
            else
            {
                textBox1.Text = tbl.Rows[0][0].ToString();
                textBox2.Text = tbl.Rows[0][1].ToString();
                textBox3.Text = tbl.Rows[0][2].ToString();

                byte[] img = (byte[])tbl.Rows[0][3];
                MemoryStream ms = new MemoryStream(img);
                pictureBox1.Image = Image.FromStream(ms);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        public void clearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            pictureBox1.Image = null;
        }
             
    }
}

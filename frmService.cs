using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SalonSpaMasterDetails
{
    public partial class frmService : Form
    {
        SqlConnection sqlcon = new SqlConnection(@"Data Source=WIN-5LQNAGUND50\SA;Initial Catalog=SalonSpaMasterDetails;Integrated Security=True");

        public frmService()
        {
            InitializeComponent();
        }

        private void frmService_Load(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void LoadGrid()
        {
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from Service", sqlcon);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText =
                    "insert into Service(ServiceID,ServiceName,Cost) values(" + txtId.Text + ",'" + txtServiceName.Text + "'," + txtCost.Text + ")";
                sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Service Inserted Successfully");
                LoadGrid();
                txtId.Clear();
                txtServiceName.Clear();
                txtCost.Clear();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sqlcon.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText =
                    "update Service set ServiceName='" + txtServiceName.Text + "', Cost=" + txtCost.Text + " where ServiceID=" + txtId.Text;
                sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Service Updated Successfully");
                LoadGrid();
                txtId.Clear();
                txtServiceName.Clear();
                txtCost.Clear();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sqlcon.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText = "delete from Service where ServiceID=" + txtId.Text;
                sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Service Deleted Successfully");
                LoadGrid();
                txtId.Clear();
                txtServiceName.Clear();
                txtCost.Clear();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sqlcon.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtId.Clear();
            txtServiceName.Clear();
            txtCost.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            sqlcon.Open();
            SqlDataAdapter sda = new SqlDataAdapter("select * from Service where ServiceID=" + id, sqlcon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtId.Text = dt.Rows[0][0].ToString();
                txtServiceName.Text = dt.Rows[0][1].ToString();
                txtCost.Text = dt.Rows[0][2].ToString();
            }
            sqlcon.Close();
        }
    }
}

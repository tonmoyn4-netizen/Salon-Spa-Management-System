using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SalonSpaMasterDetails
{
    public partial class frmBookingInfo : Form
    {
        SqlConnection sqlcon = new SqlConnection(@"Data Source=WIN-5LQNAGUND50\SA;Initial Catalog=SalonSpaMasterDetails;Integrated Security=True");
        SqlTransaction tran;

        public frmBookingInfo()
        {
            InitializeComponent();
        }

        private void frmBookingInfo_Load(object sender, EventArgs e)
        {
            LoadGrid();
            LoadServiceCombo();
        }

        private void LoadServiceCombo()
        {
            sqlcon.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from Service", sqlcon);
            DataSet ds = new DataSet();
            sqlda.Fill(ds);

            DataRow newRow = ds.Tables[0].NewRow();
            newRow[0] = "-1";
            newRow[1] = "---Select Service---";
            newRow[2] = "0";
            ds.Tables[0].Rows.InsertAt(newRow, 0);

            cmbService.DataSource = ds.Tables[0];
            cmbService.DisplayMember = "ServiceName";
            cmbService.ValueMember = "ServiceID";
            sqlcon.Close();
        }

        private void LoadGrid()
        {
            sqlcon.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter(
                @"select BookingID, CustomerName, ServiceName, CustomerImage, IsMember, TotalAmount
                  from Booking b join Service s on b.ServiceID=s.ServiceID", sqlcon);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            dataGridView2.DataSource = dt;
            sqlcon.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog1.FileName);
                this.pictureBox1.Image = img;
                txtPicture.Text = openFileDialog1.FileName;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
        }

        private void clearData()
        {
            txtId.Clear();
            txtName.Clear();
            if (cmbService.Items.Count > 0)
                cmbService.SelectedIndex = 0;
            txtPicture.Clear();
            pictureBox1.Image = null;
            rdoYes.Checked = false;
            rdoNo.Checked = true;
            txtTotalAmount.Clear();
            dataGridView1.Rows.Clear();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                Image img = Image.FromFile(txtPicture.Text);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);

                sqlcon.Open();
                tran = sqlcon.BeginTransaction();

                SqlCommand sqlcmd = new SqlCommand(
                    "insert into Booking(BookingID,CustomerName,ServiceID,CustomerImage,IsMember,TotalAmount) values (@bi,@cn,@si,@p,@im,@t)",
                    sqlcon, tran);

                sqlcmd.Parameters.AddWithValue("@bi", txtId.Text);
                sqlcmd.Parameters.AddWithValue("@cn", txtName.Text);
                sqlcmd.Parameters.AddWithValue("@si", cmbService.SelectedValue);
                sqlcmd.Parameters.Add(new SqlParameter("@p", SqlDbType.VarBinary) { Value = ms.ToArray() });
                sqlcmd.Parameters.AddWithValue("@im", rdoYes.Checked);
                sqlcmd.Parameters.AddWithValue("@t", txtTotalAmount.Text);
                sqlcmd.ExecuteNonQuery();

                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.IsNewRow)
                        continue;

                    SqlCommand sqlcmd1 = new SqlCommand(
                        @"insert into BookingDetails(BookingID,ItemName,Quantity) values(@bi,@in,@q)",
                        sqlcon, tran);

                    sqlcmd1.Parameters.AddWithValue("@bi", Convert.ToInt32(txtId.Text));
                    sqlcmd1.Parameters.AddWithValue("@in", dgvRow.Cells["dgvtxtItemName"].Value);
                    sqlcmd1.Parameters.AddWithValue("@q", dgvRow.Cells["dgvtxtQuantity"].Value);
                    sqlcmd1.ExecuteNonQuery();
                }

                tran.Commit();
                MessageBox.Show("Insert Successfull", "Insert Alert");
                clearData();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Invalid Input\n" + ex.Message);
                sqlcon.Close();
            }

            LoadGrid();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[0].Value);
            loadGridData(id);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                tran = sqlcon.BeginTransaction();

                SqlCommand sqlcmd1 = new SqlCommand(
                    @"delete from BookingDetails where BookingID=" + Convert.ToInt32(txtId.Text), sqlcon, tran);
                sqlcmd1.ExecuteNonQuery();

                SqlCommand sqlcmd = new SqlCommand(
                    @"delete from Booking where BookingID=" + txtId.Text, sqlcon, tran);
                sqlcmd.ExecuteNonQuery();

                tran.Commit();
                MessageBox.Show("Delete Successfully !!", "Delete Message");
                clearData();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Data Not Valid !!" + ex.Message);
                sqlcon.Close();
            }

            LoadGrid();
        }

        private void loadGridData(int id)
        {
            sqlcon.Open();
            SqlDataAdapter sda = new SqlDataAdapter(
                @"select BookingID,CustomerName,ServiceID,CustomerImage,IsMember,TotalAmount from Booking where BookingID=" + id,
                sqlcon);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtId.Text = dt.Rows[0][0].ToString();
                txtName.Text = dt.Rows[0][1].ToString();
                cmbService.SelectedValue = dt.Rows[0][2].ToString();

                MemoryStream ms = new MemoryStream((byte[])dt.Rows[0][3]);
                Image img = Image.FromStream(ms);
                pictureBox1.Image = img;

                bool isMember = Convert.ToBoolean(dt.Rows[0][4]);
                rdoYes.Checked = isMember;
                rdoNo.Checked = !isMember;

                txtTotalAmount.Text = dt.Rows[0][5].ToString();
            }
            sqlcon.Close();

            sqlcon.Open();
            SqlDataAdapter sda1 = new SqlDataAdapter(
                @"select BookingDetailID,ItemName,Quantity from BookingDetails where BookingID=" + id,
                sqlcon);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);

            dataGridView1.Rows.Clear();
            foreach (DataRow row in dt1.Rows)
            {
                dataGridView1.Rows.Add(row["ItemName"].ToString(), row["Quantity"].ToString());
            }
            sqlcon.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                tran = sqlcon.BeginTransaction();

                if (txtPicture.Text != "")
                {
                    Image img = Image.FromFile(txtPicture.Text);
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Bmp);

                    SqlCommand sqlcmd = new SqlCommand(
                        @"update Booking set CustomerName=@cn, ServiceID=@si, CustomerImage=@p, IsMember=@im, TotalAmount=@t where BookingID=@bi",
                        sqlcon, tran);

                    sqlcmd.Parameters.AddWithValue("@bi", txtId.Text);
                    sqlcmd.Parameters.AddWithValue("@cn", txtName.Text);
                    sqlcmd.Parameters.AddWithValue("@si", cmbService.SelectedValue);
                    sqlcmd.Parameters.Add(new SqlParameter("@p", SqlDbType.VarBinary) { Value = ms.ToArray() });
                    sqlcmd.Parameters.AddWithValue("@im", rdoYes.Checked);
                    sqlcmd.Parameters.AddWithValue("@t", txtTotalAmount.Text);
                    sqlcmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand sqlcmd = new SqlCommand(
                        @"update Booking set CustomerName=@cn, ServiceID=@si, IsMember=@im, TotalAmount=@t where BookingID=@bi",
                        sqlcon, tran);

                    sqlcmd.Parameters.AddWithValue("@bi", txtId.Text);
                    sqlcmd.Parameters.AddWithValue("@cn", txtName.Text);
                    sqlcmd.Parameters.AddWithValue("@si", cmbService.SelectedValue);
                    sqlcmd.Parameters.AddWithValue("@im", rdoYes.Checked);
                    sqlcmd.Parameters.AddWithValue("@t", txtTotalAmount.Text);
                    sqlcmd.ExecuteNonQuery();
                }

                SqlCommand delcmd = new SqlCommand(
                    @"delete from BookingDetails where BookingID=@id", sqlcon, tran);
                delcmd.Parameters.AddWithValue("@id", txtId.Text);
                delcmd.ExecuteNonQuery();

                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.IsNewRow)
                        continue;

                    SqlCommand sqlcmd1 = new SqlCommand(
                        @"insert into BookingDetails(BookingID,ItemName,Quantity) values(@bi,@in,@q)",
                        sqlcon, tran);

                    sqlcmd1.Parameters.AddWithValue("@bi", txtId.Text);
                    sqlcmd1.Parameters.AddWithValue("@in", dgvRow.Cells["dgvtxtItemName"].Value);
                    sqlcmd1.Parameters.AddWithValue("@q", dgvRow.Cells["dgvtxtQuantity"].Value);
                    sqlcmd1.ExecuteNonQuery();
                }

                tran.Commit();
                MessageBox.Show("Update Successfully", "Update Message");
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Invalid Input\n" + ex.Message);
                sqlcon.Close();
            }

            LoadGrid();
            loadGridData(Convert.ToInt32(txtId.Text));
        }
    }
}

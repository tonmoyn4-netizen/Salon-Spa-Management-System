using System;
using System.Windows.Forms;

namespace SalonSpaMasterDetails
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void serviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmService service = new frmService();
            service.ShowDialog();
        }

        private void bookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBookingInfo booking = new frmBookingInfo();
            booking.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.ShowDialog();
        }
    }
}

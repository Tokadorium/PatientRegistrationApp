using System;
using System.Windows.Forms;

namespace PatientRegistrationApp.Forms
{
    public partial class CountInputForm : Form
    {
        public int? Count { get; private set; }

        public CountInputForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtCount.Text.Trim(), out int count) && count > 0)
            {
                Count = count;
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid positive number.");
                txtCount.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

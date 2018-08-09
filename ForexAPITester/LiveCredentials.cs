using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForexAPITester
{
    public partial class LiveCredentials : Form
    {
        APICredentials dataCredentials = new APICredentials();
        APICredentials tradeCredentials = new APICredentials();
        public bool IsConfirmed { get; set; }
        public LiveCredentials()
        {
            InitializeComponent();
        }

        private void txtSave_Click(object sender, EventArgs e)
        {
            if (txtTradingPassword.Text != dataCredentials.Password)
            {
                MessageBox.Show("Incorrect password");
            }
            else
            {
                IsConfirmed = true;
                this.Close();
            }
        }

        private void LiveCredentials_Load(object sender, EventArgs e)
        {
            Library.ReadCredentials(ref dataCredentials, ref tradeCredentials);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}

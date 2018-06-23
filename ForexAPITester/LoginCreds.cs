using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForexAPITester
{
    public partial class LoginCreds : Form
    {
        public LoginCreds()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var userData = new FileStream(Application.UserAppDataPath + "\\appcreds.txt", FileMode.OpenOrCreate);
            userData.Position = 0;
            SimpleAES simpleAES = new SimpleAES();
            using (StreamWriter userWriter = new StreamWriter(userData))
            {
                userWriter.WriteLine($"UserName|{simpleAES.ByteArrToString(simpleAES.Encrypt(txtUsername.Text + Library.ENCRYPTSALT))}");
                userWriter.WriteLine($"Password|{simpleAES.ByteArrToString(simpleAES.Encrypt(txtPassword.Text + Library.ENCRYPTSALT))}");
                userWriter.WriteLine($"DataAPIKey|{txtLiveAPIKey.Text}");
                userWriter.WriteLine($"TradeUserName|{simpleAES.ByteArrToString(simpleAES.Encrypt(txtTradeUserName.Text + Library.ENCRYPTSALT))}");
                userWriter.WriteLine($"TradePassword|{simpleAES.ByteArrToString(simpleAES.Encrypt(txtTradePassword.Text + Library.ENCRYPTSALT))}");
                userWriter.WriteLine($"TradeAPIKey|{txtDemoAPIKey.Text}");
                userWriter.Flush();
                userWriter.Close();
            }
            this.Close();
        }
    }
}

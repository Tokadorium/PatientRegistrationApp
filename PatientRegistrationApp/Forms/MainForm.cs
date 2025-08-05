using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class MainForm : Form
    {
        public User LoggedUser { get; private set; }
        public MainForm(User loggedUser)
        {
            InitializeComponent();
            LoggedUser = loggedUser;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {LoggedUser.FirstName}!";
        }


    }
}

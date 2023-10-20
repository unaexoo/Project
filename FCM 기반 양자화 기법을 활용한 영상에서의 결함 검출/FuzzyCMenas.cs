using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ceramic2
{
    public partial class FuzzyCMenas : Form
    {
        public int countOfClass;
        public FuzzyCMenas()
        {
            InitializeComponent();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void check_button_Click(object sender, EventArgs e)
        {
            try
            {
                countOfClass = Int32.Parse(clusterCount.Text);

                this.DialogResult = DialogResult.OK;
            }
            catch (FormatException)
            {
                MessageBox.Show("숫자만 입력하세요.");
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

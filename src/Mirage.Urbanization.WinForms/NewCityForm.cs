using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms
{
    public partial class NewCityForm : Form
    {
        public NewCityForm()
        {
            InitializeComponent();
            sizeTrackBar.Minimum = TerraformingOptions.MinWidthAndHeight;
            sizeTrackBar.Maximum = TerraformingOptions.MaxWidthAndHeight;

            treesTrackBar.Minimum = TerraformingOptions.MinWoodlands;
            treesTrackBar.Maximum = TerraformingOptions.MaxWoodlands;

            lakesTrackBar.Minimum = TerraformingOptions.MinLakes;
            lakesTrackBar.Maximum = TerraformingOptions.MaxLakes;

            CenterValue(sizeTrackBar);
            CenterValue(treesTrackBar);
            CenterValue(lakesTrackBar);
        }

        private static void CenterValue(TrackBar trackBar)
        {
            trackBar.Value = (trackBar.Maximum / 2);
        }

        public TerraformingOptions GetTerraformingOptions()
        {
            var options = new TerraformingOptions();
            options.SetZoneWidthAndHeight(sizeTrackBar.Value);
            options.SetWoodlands(treesTrackBar.Value);
            options.SetLakes(lakesTrackBar.Value);
            options.HorizontalRiver = checkBoxHorizontalRiver.Checked;
            options.VerticalRiver = checkBoxVerticalRiver.Checked;

            options.NorthCoast = cbNorthCoast.Checked;
            options.SouthCoast = cbSouthCoast.Checked;
            options.EastCoast = cbEastCoast.Checked;
            options.WestCoast = cbWestCoast.Checked;

            return options;
        }

        private void okNewCityButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityManagement.WebUI.Controls;
using System.Web.UI.WebControls;

namespace FIM2010SampleOTPActivity
{
    partial class CellOTPGateUI : ActivitySettingsPart
    {
        protected override void CreateChildControls()
        {
            Table controlLayoutTable;
            controlLayoutTable = new Table();

            //Width is set to 100% of the control size
            controlLayoutTable.Width = Unit.Percentage(100.0);
            controlLayoutTable.BorderWidth = 0;
            controlLayoutTable.CellPadding = 2;
            //Add a TableRow for each textbox in the UI 
            controlLayoutTable.Rows.Add(this.AddTableRowLabel("This activity needs no configuration"));
            this.Controls.Add(controlLayoutTable);

            base.CreateChildControls();
        }

        //Create a TableRow that contains a label and a textbox.
        private TableRow AddTableRowLabel(String labelText)
        {
            TableRow row = new TableRow();
            TableCell labelCell = new TableCell();
            TableCell controlCell = new TableCell();
            Label oLabel = new Label();

            oLabel.Text = labelText;
            oLabel.CssClass = base.LabelCssClass;
            labelCell.Controls.Add(oLabel);
            row.Cells.Add(labelCell);
            row.Cells.Add(controlCell);
            return row;
        }
    }
}

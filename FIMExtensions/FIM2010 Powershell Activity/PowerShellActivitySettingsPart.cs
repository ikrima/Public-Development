using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Workflow.ComponentModel;
using Microsoft.IdentityManagement.WebUI.Controls;
using Microsoft.ResourceManagement.Workflow.Activities;

namespace FimExtensions.FimActivityLibrary
{
    class PowerShellActivitySettingsPart : ActivitySettingsPart
    {
        /// <summary>
        /// Called when a user clicks the Save button in the Workflow Designer. 
        /// Returns an instance of the Activity class that 
        /// has its properties set to the values entered into the text box controls
        /// used in the UI of the activity. 
        /// </summary>
        public override Activity GenerateActivityOnWorkflow(SequentialWorkflow workflow)
        {
            PowerShellActivity powerShellActivity = new PowerShellActivity();
            powerShellActivity.Script = this.GetText("txtScript");
            powerShellActivity.PowerShellModule = this.GetText("txtPowerShellModule");
            powerShellActivity.PowerShellVariables = this.GetText("txtPowerShellVariables");
            powerShellActivity.WorkflowDataNameForOutput = this.GetText("txtWorkflowDataNameForOutput");

            return powerShellActivity;
        }

        /// <summary>
        /// Called by FIM when the UI for the activity must be reloaded.
        /// It passes us an instance of our workflow activity so that we can
        /// extract the values of the properties to display in the UI.
        /// </summary>
        public override void LoadActivitySettings(Activity activity)
        {
            PowerShellActivity powerShellActivity = activity as PowerShellActivity;
            if (null != powerShellActivity)
            {                
                this.SetText("txtPowerShellVariables", powerShellActivity.PowerShellVariables);
                this.SetText("txtPowerShellModule", powerShellActivity.PowerShellModule);
                this.SetText("txtScript", powerShellActivity.Script);
            }
        }

        /// <summary>
        /// Saves the activity settings.
        /// </summary>
        public override ActivitySettingsPartData PersistSettings()
        {           
            ActivitySettingsPartData activitySettingsPartData = new ActivitySettingsPartData();
            activitySettingsPartData["PowerShellVariables"] = this.GetText("txtPowerShellVariables");
            activitySettingsPartData["PowerShellModule"] = this.GetText("txtPowerShellModule");
            activitySettingsPartData["Script"] = this.GetText("txtScript");
            activitySettingsPartData["WorkflowDataNameForOutput"] = this.GetText("txtWorkflowDataNameForOutput");
            return activitySettingsPartData;
        }

        public override void RestoreSettings(ActivitySettingsPartData activitySettingsPartData)
        {
            if (activitySettingsPartData != null)
            {               
                this.SetText("txtPowerShellVariables", (String)activitySettingsPartData["PowerShellVariables"]);
                this.SetText("txtPowerShellModule", (String)activitySettingsPartData["PowerShellModule"]);
                this.SetText("txtScript", (String)activitySettingsPartData["Script"]);            
            }
        }

        public override void SwitchMode(ActivitySettingsPartMode mode)
        {
            bool readOnly = (mode == ActivitySettingsPartMode.View);
            this.SetTextBoxReadOnlyOption("txtPowerShellVariables", readOnly);
            this.SetTextBoxReadOnlyOption("txtPowerShellModule", readOnly);
            this.SetTextBoxReadOnlyOption("txtScript", readOnly);
            this.SetTextBoxReadOnlyOption("txtWorkflowDataNameForOutput", readOnly);     
        }

        public override string Title
        {
            get { return "FIM Extensions PowerShell Activity"; }
        }

        public override bool ValidateInputs()
        {
            return true;
        }

        protected override void CreateChildControls()
        {
            Table controlLayoutTable;
            controlLayoutTable = new Table();

            //Width is set to 100% of the control size
            controlLayoutTable.Width = Unit.Percentage(100.0);
            controlLayoutTable.BorderWidth = 0;
            controlLayoutTable.CellPadding = 2;
            //Add a TableRow for each textbox in the UI 
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("PowerShell Module to Load:", "txtPowerShellModule", 400, 100, false, "[Optional] Enter the path of a module to load for the PowerShell session."));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("WorkflowData item(s) to use as PowerShell variables:", "txtPowerShellVariables", 400, 100, false, "[Optional] Enter a comma separated list of WorkflowData items to use as PowerShell variables."));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("WorkflowData item to store PowerShell script output:", "txtWorkflowDataNameForOutput", 400, 100, false, "[Optional] Enter the name of a WorkflowData item to store the PowerShell script output."));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("PowerShell Script:", "txtScript", 400, 100, true, "Get-Process | Select -First 5"));
            this.Controls.Add(controlLayoutTable);
            
            base.CreateChildControls();
        }

        //Create a TableRow that contains a label and a textbox.
        private TableRow AddTableRowTextBox(String labelText, String controlID, int width, int
                                             maxLength, Boolean multiLine, String defaultValue)
        {
            TableRow row = new TableRow();
            TableCell labelCell = new TableCell();
            TableCell controlCell = new TableCell();
            Label oLabel = new Label();
            TextBox oText = new TextBox();

            oLabel.Text = labelText;
            oLabel.CssClass = base.LabelCssClass;
            labelCell.Controls.Add(oLabel);
            oText.ID = controlID;
            oText.CssClass = base.TextBoxCssClass;
            oText.Text = defaultValue;
            oText.MaxLength = maxLength;
            oText.Width = width;
            if (multiLine)
            {
                oText.TextMode = TextBoxMode.MultiLine;
                oText.Rows = System.Math.Min(6, (maxLength + 60) / 60);
                oText.Wrap = true;
            }
            controlCell.Controls.Add(oText);
            row.Cells.Add(labelCell);
            row.Cells.Add(controlCell);
            return row;
        }
        string GetText(string textBoxID)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            return textBox.Text ?? String.Empty;
        }
        void SetText(string textBoxID, string text)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            if (textBox != null)
                textBox.Text = text;
            else
                textBox.Text = "";
        }

        //Set the text box to read mode or read/write mode
        void SetTextBoxReadOnlyOption(string textBoxID, bool readOnly)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            textBox.ReadOnly = readOnly;
        }
    }
}

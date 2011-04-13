using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityManagement.WebUI.Controls;
using Microsoft.ResourceManagement.Workflow.Activities;
using System.Workflow.ComponentModel;

namespace FIM2010SampleOTPActivity
{
    public partial class CellOTPGateUI : ActivitySettingsPart
    {
        public override Activity GenerateActivityOnWorkflow(SequentialWorkflow workflow)
        {
            CellOTPGate cellOTPGate = new CellOTPGate();

            AuthenticationGateActivity authNGateActivity = new AuthenticationGateActivity();
            authNGateActivity.AuthenticationGate = cellOTPGate;

            return authNGateActivity;
        }

        public override void LoadActivitySettings(Activity activity)
        {
        }

        public override ActivitySettingsPartData PersistSettings()
        {
            //Nothing to track yet; might add resolver grammer
            ActivitySettingsPartData data = new ActivitySettingsPartData();
            return data;
        }

        public override void RestoreSettings(ActivitySettingsPartData data)
        {
        }

        public override void SwitchMode(ActivitySettingsPartMode mode)
        {
        }

        public override string Title
        {
            get { return "Cellphone OTP Gate"; }
        }

        public override bool ValidateInputs()
        {
            return true;
        }
    }
}


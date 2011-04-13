using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace FIM2010SampleOTPActivity
{
    public partial class SMSPasswordReset
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        [System.CodeDom.Compiler.GeneratedCode("", "")]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.codeActivity1 = new System.Workflow.Activities.CodeActivity();
            this.readResourceActivity1 = new Microsoft.ResourceManagement.Workflow.Activities.ReadResourceActivity();
            // 
            // codeActivity1
            // 
            this.codeActivity1.Name = "codeActivity1";
            this.codeActivity1.ExecuteCode += new System.EventHandler(this.SendOTPPassword);
            // 
            // readResourceActivity1
            // 
            this.readResourceActivity1.ActorId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.readResourceActivity1.Name = "readResourceActivity1";
            activitybind1.Name = "SMSPasswordReset";
            activitybind1.Path = "CurrentActor";
            this.readResourceActivity1.ResourceId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.readResourceActivity1.SelectionAttributes = null;
            this.readResourceActivity1.SetBinding(Microsoft.ResourceManagement.Workflow.Activities.ReadResourceActivity.ResourceProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // SMSPasswordReset
            // 
            this.Activities.Add(this.readResourceActivity1);
            this.Activities.Add(this.codeActivity1);
            this.Name = "SMSPasswordReset";
            this.CanModifyActivities = false;

        }

        #endregion

        private CodeActivity codeActivity1;

        private Microsoft.ResourceManagement.Workflow.Activities.ReadResourceActivity readResourceActivity1;


    }
}

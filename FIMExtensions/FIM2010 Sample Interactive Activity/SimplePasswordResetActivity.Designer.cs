using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace FIM2010SampleInteractiveActivity
{
    public partial class SimplePasswordReset
    {
        #region Activity Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        [System.CodeDom.Compiler.GeneratedCode("", "")]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Activities.WorkflowServiceAttributes workflowserviceattributes1 = new System.Workflow.Activities.WorkflowServiceAttributes();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.WorkflowServiceAttributes workflowserviceattributes2 = new System.Workflow.Activities.WorkflowServiceAttributes();
            this.xmlInteractiveActivity1 = new Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity();
            workflowserviceattributes1.ConfigurationName = "Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity";
            workflowserviceattributes1.Name = "XmlInteractiveActivity";
            // 
            // xmlInteractiveActivity1
            // 
            this.xmlInteractiveActivity1.CreateFaultMessage = null;
            this.xmlInteractiveActivity1.CreateRequestMessage = null;
            this.xmlInteractiveActivity1.CreateResponseMessage = null;
            activitybind1.Name = "SimplePasswordReset";
            activitybind1.Path = "UserSubmittedDocument";
            activitybind2.Name = "SimplePasswordReset";
            activitybind2.Path = "ObjectTypeRequested";
            this.xmlInteractiveActivity1.EnableDefaultOperationValidation = true;
            activitybind3.Name = "SimplePasswordReset";
            activitybind3.Path = "AccessList";
            this.xmlInteractiveActivity1.MetadataFaultMessage = null;
            this.xmlInteractiveActivity1.MetadataRequestMessage = null;
            this.xmlInteractiveActivity1.MetadataResponseMessage = null;
            this.xmlInteractiveActivity1.Name = "xmlInteractiveActivity1";
            this.xmlInteractiveActivity1.ReferenceProperties = null;
            this.xmlInteractiveActivity1.ResourceAddress = null;
            this.xmlInteractiveActivity1.TimeoutDuration = System.TimeSpan.Parse("00:05:00");
            this.xmlInteractiveActivity1.ValidateSamlToken = true;
            this.xmlInteractiveActivity1.ReceiveTimeout += new System.EventHandler(this.ReceiveTimeout);
            this.xmlInteractiveActivity1.XmlDocumentValidation += new System.EventHandler<Microsoft.ResourceManagement.Workflow.Activities.XmlDocumentValidationEventArgs>(this.AttemptSimplePasswordReset);
            this.xmlInteractiveActivity1.CreateOperationValidation += new System.EventHandler<System.Workflow.Activities.OperationValidationEventArgs>(this.CustomUserValidation);
            this.xmlInteractiveActivity1.MetadataGetOperationValidation += new System.EventHandler<System.Workflow.Activities.OperationValidationEventArgs>(this.CustomUserValidation);
            this.xmlInteractiveActivity1.SetValue(System.Workflow.Activities.ReceiveActivity.WorkflowServiceAttributesProperty, workflowserviceattributes1);
            this.xmlInteractiveActivity1.SetBinding(Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity.EndpointAccessUserListProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.xmlInteractiveActivity1.SetBinding(Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity.DocumentTypeProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.xmlInteractiveActivity1.SetBinding(Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity.DocumentProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            workflowserviceattributes2.ConfigurationName = "FIM2010SampleInteractiveActivity.Activity1";
            workflowserviceattributes2.Name = "SimplePasswordReset";
            // 
            // SimplePasswordReset
            // 
            this.Activities.Add(this.xmlInteractiveActivity1);
            this.Name = "SimplePasswordReset";
            this.SetValue(System.Workflow.Activities.ReceiveActivity.WorkflowServiceAttributesProperty, workflowserviceattributes2);
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.ResourceManagement.Workflow.Activities.XmlInteractiveActivity xmlInteractiveActivity1;












    }
}

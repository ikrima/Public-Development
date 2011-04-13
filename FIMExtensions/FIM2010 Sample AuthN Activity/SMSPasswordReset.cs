using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Linq;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.ResourceManagement.Client;
using Microsoft.ResourceManagement.Workflow.Activities;
using Microsoft.ResourceManagement.ObjectModel.ResourceTypes;
using System.DirectoryServices.AccountManagement;

namespace FIM2010SampleOTPActivity
{
    public partial class SMSPasswordReset : SequenceActivity
    {
        public SMSPasswordReset()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SequentialWorkflow containingSequentialWorkflow = null;
            SequentialWorkflow.TryGetContainingWorkflow(this, out containingSequentialWorkflow);

            this.readResourceActivity1.ActorId = containingSequentialWorkflow.ActorId;
            return base.Execute(executionContext);
        }

        public Microsoft.ResourceManagement.WebServices.WSResourceManagement.ResourceType CurrentActor = new Microsoft.ResourceManagement.WebServices.WSResourceManagement.ResourceType();

        private void SendOTPPassword(object sender, EventArgs e)
        {
            string userCellPhone;
            SequentialWorkflow containingSequentialWorkflow = null;
            SequentialWorkflow.TryGetContainingWorkflow(this, out containingSequentialWorkflow);

            using (DefaultClient client = new DefaultClient())
            {
                client.RefreshSchema();


                Guid targetUser;
                if (containingSequentialWorkflow.ActorId == CellOTPGate.AnonymousID)
                { targetUser = containingSequentialWorkflow.TargetId; }
                else
                { targetUser = containingSequentialWorkflow.ActorId; }

                RmPerson person = client.Get(new Microsoft.ResourceManagement.ObjectModel.RmReference(targetUser.ToString())) as RmPerson;
                userCellPhone = person.MobilePhone;
            }


            Random randGen = new Random();
            var generatedPassword = "p@ssword" + randGen.Next(0, 100).ToString();

            CellGatewayWrapper.SendTextMessage(userCellPhone, CellCarriers.ATT, generatedPassword);

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, (string)this.CurrentActor["AccountName"]))
                {
                    user.SetPassword(generatedPassword);
                }
            }
        }
    }
}

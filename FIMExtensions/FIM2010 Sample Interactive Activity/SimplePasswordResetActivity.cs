using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.ResourceManagement.Workflow.Activities;
using Microsoft.ResourceManagement.WebServices.WSAddressing;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WorkflowContract;
using Microsoft.ResourceManagement.Client;

namespace FIM2010SampleInteractiveActivity
{
    public partial class SimplePasswordReset : SequenceActivity
    {
        public readonly static Type ObjectTypeRequested = typeof(SimplePWResetDocument);

        public SimplePasswordReset()
        {
            InitializeComponent();
        }

        protected override void Initialize(IServiceProvider provider)
        {

            base.Initialize(provider);
        }

        public static DependencyProperty AccessListProperty = DependencyProperty.Register("AccessList", typeof(System.Collections.ObjectModel.Collection<Microsoft.ResourceManagement.WebServices.UniqueIdentifier>), typeof(FIM2010SampleInteractiveActivity.SimplePasswordReset));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Parameters")]
        public System.Collections.ObjectModel.Collection<Microsoft.ResourceManagement.WebServices.UniqueIdentifier> AccessList
        {
            get
            {
                return ((System.Collections.ObjectModel.Collection<Microsoft.ResourceManagement.WebServices.UniqueIdentifier>)(base.GetValue(FIM2010SampleInteractiveActivity.SimplePasswordReset.AccessListProperty)));
            }
            set
            {
                base.SetValue(FIM2010SampleInteractiveActivity.SimplePasswordReset.AccessListProperty, value);
            }
        }

        public void CustomUserValidation(object sender, OperationValidationEventArgs e)
        {
            //Let's say we had extra permissions that were outside of FIM that we needed to check
            //We could do these calls here

            e.IsValid = true;
        }

        public void AttemptSimplePasswordReset(object sender, XmlDocumentValidationEventArgs e)
        {
            if (e.XmlDocument is SimplePWResetDocument)
            {
                this.xmlInteractiveActivity1.ReferenceProperties = new ReferencePropertiesType(new Microsoft.ResourceManagement.WebServices.UniqueIdentifier("SimplePWResetDocument"));
                this.xmlInteractiveActivity1.ResourceAddress = new Uri("http://NotUsableAddress/");
            }
            else
            {
                this.xmlInteractiveActivity1.CreateFaultMessage = new FaultException(
                                                                        MessageFault.CreateFault(
                                                                            FaultCode.CreateSenderFaultCode("DataRequiredFault", "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault"),
                                                                            new FaultReason(new FaultReasonText("DataRequiredFaultReason")),
                                                                            new DataRequiredFault(SimplePasswordReset.ObjectTypeRequested, "Your password was not long enough"),
                                                                            new ClientSerializer(typeof(DataRequiredFault))));
            }
        }

        public void ReceiveTimeout(object sender, EventArgs e)
        {
            throw new Exception("Workflow timed out!");
        }

        public static DependencyProperty UserSubmittedDocumentProperty = DependencyProperty.Register("UserSubmittedDocument", typeof(System.Object), typeof(FIM2010SampleInteractiveActivity.SimplePasswordReset));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Activity Data")]
        public Object UserSubmittedDocument
        {
            get
            {
                return ((object)(base.GetValue(FIM2010SampleInteractiveActivity.SimplePasswordReset.UserSubmittedDocumentProperty)));
            }
            set
            {
                base.SetValue(FIM2010SampleInteractiveActivity.SimplePasswordReset.UserSubmittedDocumentProperty, value);
            }
        }

    }
}

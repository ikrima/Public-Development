using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using Microsoft.ResourceManagement.Workflow.Runtime;
using Microsoft.ResourceManagement.Data;
using Microsoft.ResourceManagement.Workflow.Activities;
using Microsoft.ResourceManagement;
using Microsoft.ResourceManagement.Client;
using Microsoft.ResourceManagement.ObjectModel.ResourceTypes;
using System.ServiceModel;

namespace FIM2010SampleOTPActivity
{

    [Serializable]
    public class CellOTPGate : AuthenticationGate
    {
        public static readonly Guid AnonymousID = new Guid("B0B36673-D43B-4CFA-A7A2-AFF14FD90522");
        public static readonly Guid activityGuid = new Guid("18360067-61EE-4752-AA04-07B2290D3F89");

        public string currentInstanceOTP = string.Empty;
        public string userCellPhone = string.Empty;

        [NonSerialized]
        WorkflowAuthenticationChallenge authenticationChallengeField;


        protected override void InitializeAuthenticationGate(IServiceProvider provider)
        {
            
            // When the activity is first loaded, we're going to try to retrieve the user info from the registration data
            if (this.AuthenticationGateActivity.RegistrationData == null ||
                string.IsNullOrEmpty(this.userCellPhone = UnicodeEncoding.Unicode.GetString(this.AuthenticationGateActivity.RegistrationData)))
            {
                //Looks like our cell phone data was not stored in registration data
                //Default to FIM store
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    using (DefaultClient client = new DefaultClient())
                    {
                        client.RefreshSchema();

                        SequentialWorkflow containingSequentialWorkflow = null;
                        SequentialWorkflow.TryGetContainingWorkflow(this.AuthenticationGateActivity, out containingSequentialWorkflow);

                        Guid targetUser;
                        if (containingSequentialWorkflow.ActorId == CellOTPGate.AnonymousID)
                        { targetUser = containingSequentialWorkflow.TargetId; }
                        else
                        { targetUser = containingSequentialWorkflow.ActorId; }

                        RmPerson person = client.Get(new Microsoft.ResourceManagement.ObjectModel.RmReference(targetUser.ToString())) as RmPerson;
                        this.userCellPhone = person.MobilePhone;

                    }
                }
            }

            base.InitializeAuthenticationGate(provider);
        }

        //The Guid of the activity to tie the client dll with this activity.  Used for versioning
        public override Guid GateTypeId
        {
            get
            {
                return activityGuid;
            }
        }

        // this gate must be displayed at registration time
        public override bool RequiresInteractiveRegistration
        {
            get
            {
                /* Could expand it here and only return true if we don't already have a phone from the user object in the
                 * FIM Data store or any other data store or even if we had previously stored it
                 * Use this.AuthenticationGateActivity.RegistrationData to access the registration data previously stored */

                return true;
            }
        }

        public override WorkflowAuthenticationChallenge AuthenticationChallenge
        {
            get
            {
                if (this.Mode == AuthenticationWorkflowMode.Registration)
                {

                    this.authenticationChallengeField = new WorkflowAuthenticationChallenge();
                    this.authenticationChallengeField.Name = "CellPhoneOTP";
                    this.authenticationChallengeField.ActivityGuid = this.GateTypeId;
                    this.authenticationChallengeField.SetHash(this.authenticationChallengeField.ActivityGuid.ToByteArray());
                    this.authenticationChallengeField.SetData(UnicodeEncoding.Unicode.GetBytes("Please enter the phone you want to use for PW Reset."));

                    return this.authenticationChallengeField;
                }
                else
                {
                    this.authenticationChallengeField = new WorkflowAuthenticationChallenge();
                    this.authenticationChallengeField.Name = "CellPhoneOTP";
                    this.authenticationChallengeField.ActivityGuid = this.GateTypeId;
                    this.authenticationChallengeField.SetHash(this.authenticationChallengeField.ActivityGuid.ToByteArray());
                    this.authenticationChallengeField.SetData(UnicodeEncoding.Unicode.GetBytes("Please enter the code sent to this phone number " + this.userCellPhone));

                    //Send out that OTP
                    Random randGen = new Random();
                    this.currentInstanceOTP = randGen.Next(100, 999).ToString();
                    CellGatewayWrapper.SendTextMessage(this.userCellPhone, CellCarriers.ATT, this.currentInstanceOTP);


                    return this.authenticationChallengeField;
                }
            }
        }


        public override byte[] RegisterUser(WorkflowAuthenticationResponse responseData)
        {
            //Since we're going to flow out the phone attribute, we don't need to retrieve data from the user

            //If we did need to store registration data (for example, we were going to ask the user for his phone number instead of flowing it, we would:
            // Take the data that he submitted (let's say it's the phone number). The format of this is controlled by what your activity on the client side submits
            // Then we would do any necessary parsing or data transformation.  For example, let's say we needed to determine the Carrier Type and store that.
            // Once we have the data, we just return the byte stream that we want to persist

            string userCustomNumber = UnicodeEncoding.Unicode.GetString(responseData.data);

            //We could expand this by making another client call to store the number into the FIM Person object
            return UnicodeEncoding.Unicode.GetBytes(userCustomNumber);
        }

        public override bool TryValidateUser(byte[] registrationData, WorkflowAuthenticationResponse responseData, out string validationError)
        {
            //Your validation errors must start with this prefix to show up on the request status details
            validationError = AuthenticationGate.AuthNValidationError;

            if (responseData == null)
            {
                throw new ArgumentNullException("responseData");
            }

            //At this point, if we had some registration data, it would be provided in registrationData
            //string customActivityData = UnicodeEncoding.Unicode.GetString(registrationData);
            // In the case of some activities, we might need to compare this data with the submitted data. For example, the QA activity needs to make sure the 
            //answers submitted match the ones provided during registration
            // For the OTP Authentication Gate, we don't need to do that.  We just take the otp code and compare it against what we generated

            string userSubmittedOTP = UnicodeEncoding.Unicode.GetString(responseData.data);

            if (userSubmittedOTP != this.currentInstanceOTP)
            {
                validationError += "The submitted one time pin was incorrect.";
                return false;
            }

            return true;
        }

    }
}
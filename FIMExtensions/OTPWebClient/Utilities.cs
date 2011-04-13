using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.ObjectModel.ResourceTypes;
using Microsoft.ResourceManagement.ObjectModel;
using Microsoft.ResourceManagement.Client.WsTransfer;
using Microsoft.ResourceManagement.Client;
using System.Xml.Schema;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.Client;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace FIMExtensions.OTP
{
    class Utilities
    {
        internal static void TestOTPBusiness()
        {
            AuthenticationRequiredException authnException = null;
            WorkflowAuthenticationChallenge workflowAuthenticationChallenge = null;

            //Initiate OTP Reset
            try
            {
                OTPReset("ilm-vm-serverad", "jdoe", null, null);
            }
            catch (AuthenticationRequiredException exception)
            {
                authnException = exception;
            }

            //Go to STS to get the challenge
            Utilities.OTPGateChallengeResponse(null /* we don't have anything to respond yet*/, ref authnException, out workflowAuthenticationChallenge);
            Console.WriteLine(UnicodeEncoding.Unicode.GetString(workflowAuthenticationChallenge.data));

            //Now send our challenge response aka the OTP Pin
            string otpTestPin = Console.ReadLine();
            var workflowChallengeResponse = new WorkflowAuthenticationResponse();
            workflowChallengeResponse.data = UnicodeEncoding.Unicode.GetBytes(otpTestPin);

            var securityToken = Utilities.OTPGateChallengeResponse(workflowChallengeResponse, ref authnException, out workflowAuthenticationChallenge);

            //Now we have a security token.  Time to go back to the MT to resubmit our initial request
            Utilities.OTPReset("ilm-vm-serverad", "jdoe", securityToken, authnException.InitialContextMessageProperty);

            //Bi-winning
        }

        public static ContextualSecurityToken OTPGateChallengeResponse(WorkflowAuthenticationResponse gateResponse, 
                                                   ref AuthenticationRequiredException authNException, 
                                                   out WorkflowAuthenticationChallenge workflowAuthenticationChallenge)
        {
            AuthenticationChallengeResponseType[] authenticationChallengeResponses = null;


            if (gateResponse != null)
            {
                AuthenticationChallengeResponseType authenticationChallengeResponse = new AuthenticationChallengeResponseType();
                authenticationChallengeResponse.Response = new ClientSerializer(
                    typeof(WorkflowAuthenticationResponse)).WriteObjectToXmlElement(gateResponse);

                authenticationChallengeResponses = new AuthenticationChallengeResponseType[] { authenticationChallengeResponse };
            }

            ContextualSecurityToken authNSecurityToken = null;
            workflowAuthenticationChallenge = null;

            try
            {
                MessageBuffer messageBuffer;
                authNSecurityToken = authNException.Authenticate(authenticationChallengeResponses, out messageBuffer);
            }
            catch (AuthenticationRequiredException exception)
            {
                authNException = exception;
                workflowAuthenticationChallenge = (WorkflowAuthenticationChallenge)new Microsoft.ResourceManagement.Client.ClientSerializer(
                        typeof(WorkflowAuthenticationChallenge)).ReadObjectFromXmlNode(
                            authNException.AuthenticationChallenges[0].Challenge);
            }

            return authNSecurityToken;
        }

        public static void OTPReset(string domain, string username, ContextualSecurityToken authNSecurityToken, ContextMessageProperty contextMessageProperty)
        {
            // Create Anonymouse RmPerson and set ObjectID to Domain\User
            // The ObjectID attribute will become ResourceReferenceProperty in the message header
            RmPerson user = new RmPerson();
            RmReference domainAndUsernameReference = new RmReference();
            domainAndUsernameReference.DomainAndUserNameValue = domain + '\\' + username;
            user.ObjectID = domainAndUsernameReference;
            PutResponse putResponse;
            putResponse = new PutResponse();
            string STSEndpoint = String.Empty;
            bool putSuccess = false; //This should always stay false with these calls unless no password reset workflow or qa authn workflow is attached.

            var alternateClient = new AlternateClient();
            var mexClient = new MexClient();
            XmlSchemaSet metadata = mexClient.Get();
            var requestFactory = new RmRequestFactory(metadata);

            // Set ResetPassword to true
            // Need a transaction to watch changes to the user
            using (RmResourceChanges transaction = new RmResourceChanges(user))
            {
                transaction.BeginChanges();

                user.ResetPassword = "True";

                try
                {
                    if (transaction.RmObject.ObjectID.Value.Split('\\').Length != 2)
                    {
                        throw new ArgumentException("User Identity must be specified by netbios domain in this format: Domain name\\user name.");
                    }

                    PutRequest alternateEPrequest = requestFactory.CreatePutRequest(transaction);

                    try
                    {
                        alternateClient.Put(alternateEPrequest, out putResponse, authNSecurityToken, contextMessageProperty);
                        putSuccess = true;
                    }
                    catch (System.ServiceModel.FaultException<Microsoft.ResourceManagement.Client.Faults.AuthenticationRequiredFault> authNFault)
                    {

                        Microsoft.ResourceManagement.WebServices.WSResourceManagement.AuthenticationRequiredFault msAuthNFault =
                            new Microsoft.ResourceManagement.WebServices.WSResourceManagement.AuthenticationRequiredFault(authNFault.Detail.SecurityTokenServiceAddress,
                                                                                             authNFault.Detail.UserRegistered.GetValueOrDefault(),
                                                                                             authNFault.Detail.UserLockedOut.GetValueOrDefault());

                        ContextMessageProperty responseContext;

                        if (ContextMessageProperty.TryGet(putResponse.Message, out responseContext) == false)
                        {
                            throw new InvalidOperationException("Could not retrieve security context message property even though we received an AuthN Fault. Something is fundamentally broken. Ensure assembly versions are correct and upgrades did not change protocol.");
                        }

                        throw new AuthenticationRequiredException(authNFault.Reason.ToString(),
                                                                 msAuthNFault,
                                                                 responseContext);
                    }
                }
                finally
                {
                    if (putSuccess == true)
                    {
                        transaction.AcceptChanges();
                    }
                    else
                    {
                        transaction.DiscardChanges();
                    }
                }
            }
        }
    }
}

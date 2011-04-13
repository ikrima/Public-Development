using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ResourceManagement.WebServices.Client;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using FIMExtensions.OTP;
using System.Text;

namespace OTPWebClient
{
    public partial class _Default : System.Web.UI.Page
    {
        int stage;

        string jqueryUIscript = @"var available_indexes = [0];
$(function () {
            

            // Accordion
            //$(""#accordion"").accordion({ header: ""h3"" });
            $('#accordion').accordion({
                header: 'h3',
                change: function (event, ui) {
                    var newIndex = $(ui.newHeader).index('h3');
                    if (jQuery.inArray(newIndex, available_indexes) == -1) {
                        var oldIndex = $(ui.oldHeader).index('h3');
                        $(this).accordion(""activate"", oldIndex);
                        alert('That panel is not yet available');
                    }
                }
            });

            // Button
            $(""#sendOTPButton,#validateOTPButton"").button();

            //hover states on the static widgets
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);

        });";

        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.ClientScript.RegisterClientScriptInclude("jquery", "js/jquery-1.5.1.min.js");
            //Page.ClientScript.RegisterClientScriptInclude("jqueryUI", "js/jquery-ui-1.8.11.custom.min.js");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SomeKey", jqueryUIscript, true);
            if (!IsPostBack)
            {
                stage = 0;
            }

            ScriptManager.RegisterClientScriptInclude(this.Page, this.GetType(), "jquery", "js/jquery-1.5.1.min.js");
            ScriptManager.RegisterClientScriptInclude(this.Page, this.GetType(), "jqueryUI", "js/jquery-ui-1.8.11.custom.min.js");
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SomeKey", jqueryUIscript, true);
        }

        protected void sendOTPButton_Click(object sender, EventArgs e)
        {
            AuthenticationRequiredException authnException = null;
            WorkflowAuthenticationChallenge workflowAuthenticationChallenge = null;

            string[] userDetails = this.domainUserName.Text.Split('\\');


            //Initiate OTP Reset
            try
            {
                Utilities.OTPReset(userDetails[0], userDetails[1], null, null);
            }
            catch (AuthenticationRequiredException exception)
            {
                authnException = exception;
            }

            //Go to STS to get the challenge
            Utilities.OTPGateChallengeResponse(null /* we don't have anything to respond yet*/, ref authnException, out workflowAuthenticationChallenge);
            this.otpGateInstructions.Text = UnicodeEncoding.Unicode.GetString(workflowAuthenticationChallenge.data);

            HttpContext.Current.Cache.Insert("authNExcep", authnException);

            stage = 1;
            ScriptManager sm = ScriptManager.GetCurrent(Page);
            if (sm.IsInAsyncPostBack)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "anotherKey", "available_indexes.push(" + stage + ");$('#accordion').accordion('activate', " + stage + ");", true);
            }
        }

        protected void validateOTPButton_Click(object sender, EventArgs e)
        {
            //Now send our challenge response aka the OTP Pin
            string[] userDetails = this.domainUserName.Text.Split('\\');
            WorkflowAuthenticationChallenge workflowAuthenticationChallenge = null;
            var workflowChallengeResponse = new WorkflowAuthenticationResponse();
            workflowChallengeResponse.data = UnicodeEncoding.Unicode.GetBytes(this.otpInput.Text);

            var authnException = HttpContext.Current.Cache.Get("authNExcep") as AuthenticationRequiredException;
            var securityToken = Utilities.OTPGateChallengeResponse(workflowChallengeResponse, ref authnException, out workflowAuthenticationChallenge);

            //Now we have a security token.  Time to go back to the MT to resubmit our initial request
            try
            {
                Utilities.OTPReset(userDetails[0], userDetails[1], securityToken, authnException.InitialContextMessageProperty);

                //Bi-winning
                this.otpvalidationResults.Text = "You are winning so radically before our first cup of coffee. Your new password has been sent to your phone. I take it back, you are bi-winning.";
            }
            catch
            {
                this.otpvalidationResults.Text = "Stop trying to hack other people's accounts by guessing passwords; this is FIM not facebook. Or maybe you can't type your pin from your phone correctly.  That's prolly worse. Either way, you just won two side orders of FAIL.";
            }

            stage = 2;
            ScriptManager sm = ScriptManager.GetCurrent(Page);
            if (sm.IsInAsyncPostBack)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "anotherKey", "available_indexes.push(" + stage + ");$('#accordion').accordion('activate', " + stage + ");", true);
            }

        }

        
    }
}
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OTPWebClient._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="css/smoothness/jquery-ui-1.8.11.custom.css" rel="stylesheet" />
    <link href="http://fonts.googleapis.com/css?family=Josefin+Sans:100,100italic,300,300italic,400,400italic,600,600italic,700,700italic"
        rel="stylesheet" type="text/css" />
    <style type="text/css">
        h2
        {
            font-family: 'Josefin Sans' , serif;
            font-size: 49px;
            font-style: normal;
            font-weight: 400;
            text-shadow: 2px 2px 2px #aaa;
            text-decoration: none;
            text-transform: none;
            letter-spacing: 0em;
            word-spacing: 0em;
            line-height: 1.2;
        }
        
        .watermarked
        {
            font-family: 'Josefin Sans' , serif;
            font-size: 49px;
            color: Gray;
            font-style: normal;
            font-weight: 400;
            text-decoration: none;
            text-transform: none;
            letter-spacing: 0em;
            word-spacing: 0em;
            line-height: 1.2;
        }
        
    </style><%--
    <script type="text/javascript" src="js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.8.11.custom.min.js"></script>
    <script type="text/javascript">
        $(function () {
            var available_indexes = [0];

            // Accordion
            //$("#accordion").accordion({ header: "h3" });
            $('#accordion').accordion({
                header: 'h3',
                change: function (event, ui) {
                    var newIndex = $(ui.newHeader).index('h3');
                    if (jQuery.inArray(newIndex, available_indexes) == -1) {
                        var oldIndex = $(ui.oldHeader).index('h3');
                        $(this).accordion("activate", oldIndex);
                        alert('That panel is not yet available');
                    }
                }
            });

            // Button
            $("#sendOTPButton,#validateOTPButton").button();

            //hover states on the static widgets
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);

        });
    </script>--%>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h2 class="demoHeaders">
                One Time Password Reset</h2>
            <div id="accordion">
                <div>
                    <h3>
                        <a href="#">Enter User Details</a></h3>
                    <div>
                        <asp:TextBox ID="domainUserName" runat="server" />
                        <ajx:TextBoxWatermarkExtender ID="domainUserName_TextBoxWatermarkExtender" runat="server"
                            Enabled="True" TargetControlID="domainUserName" WatermarkText="Domain\UserName"
                            WatermarkCssClass="watermarked">
                        </ajx:TextBoxWatermarkExtender>
                        <asp:Button ID="sendOTPButton" runat="server" OnClick="sendOTPButton_Click" Text="Send OTP" />
                    </div>
                </div>
                <div>
                    <h3>
                        <a href="#">Enter your OTP</a></h3>
                    <div>
                        <asp:Label ID="otpGateInstructions" runat="server" />
                        <asp:TextBox ID="otpInput" runat="server" />
                        <asp:Button ID="validateOTPButton" runat="server" OnClick="validateOTPButton_Click" Text="Validate OTP" />
                        </div>
                </div>
                <div>
                    <h3>
                        <a href="#">Results</a></h3>
                    <div>
                        <asp:Label ID="otpvalidationResults" runat="server"/>
                        </div>
                </div>
            </div>
     
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>

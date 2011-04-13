using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Microsoft.ResourceManagement.Client;

namespace FIM2010SampleOTPActivity
{
    public enum CellCarriers
    {
        ATT
    }

    public class CellGatewayWrapper
    {
       

        private static string RetrieveCellCarrierMailbox(CellCarriers carrier, string cellPhone)
        {
            string cellmailboxaddress;
            switch (carrier)
            {
                case CellCarriers.ATT:
                    cellmailboxaddress = String.Format("{0}@txt.att.net", cellPhone);
                    break;

                default:
                    throw new Exception("Unrecognized carrier");
            }

            return cellmailboxaddress;
        }

        public static void SendTextMessage(string cellNumber, CellCarriers carrier, string otpCode)
        {
            try
            {
                string cellEmail = RetrieveCellCarrierMailbox(carrier, cellNumber);

                MailMessage message = new MailMessage();
                message.From = new MailAddress("omnipresentfim@example.com");
                message.To.Add(new MailAddress(cellEmail));

                message.Subject = "One Time Pin";
                message.Body = otpCode;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("dummy@example.com", "test!");
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
            catch (SmtpException exception)
            {
                LoggingUtility.TraceException(exception, System.Diagnostics.TraceEventType.Error);
            }
        }
    }

}

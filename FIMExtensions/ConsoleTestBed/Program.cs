using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIM2010SampleOTPActivity;
using FIMExtensions.OTP;

namespace ConsoleTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            Utilities.TestOTPBusiness();

            CellGatewayWrapper.SendTextMessage("5125607446", CellCarriers.ATT, "123");

            
        }
    }
}

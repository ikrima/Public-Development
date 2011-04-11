using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIM2010SampleAuthNActivity;

namespace ConsoleTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            CellGatewayWrapper.SendTextMessage("5125607446", CellCarriers.ATT, "123");
        }
    }
}

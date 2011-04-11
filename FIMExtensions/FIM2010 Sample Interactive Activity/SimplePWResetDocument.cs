using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FIM2010SampleInteractiveActivity
{
    [Serializable]
    [XmlRoot("SimplePWResetDocument")]
    public class SimplePWResetDocument
    {
        public string password;
    }
}

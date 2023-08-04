using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    [DelimitedRecord(";")]
    [IgnoreFirst]
    public class Testmodel1
    {
        [FieldOrder(1), FieldCaption("IKA-AZ")]
        public string IKAAZ { get; set; }
        [FieldOrder(2), FieldCaption("LPS Number")]
        public string LPSNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models
{
    public struct Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

    }
}

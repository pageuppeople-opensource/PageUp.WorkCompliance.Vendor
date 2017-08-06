using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PageUp.Compliance.Vendor.Core
{
    public class ComplianceStatus
    {
        public string Status { get; set; }
        public string[] Notes { get; set; }
        public DateTime Created { get; set; }
    }
}

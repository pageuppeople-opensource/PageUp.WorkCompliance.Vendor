using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PageUp.Compliance.Vendor.Core
{
    public class ComplianceInvitation
    {
        public string InvitationId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LocationCode { get; set; }
        public string PositionCode { get; set; }
        public Site Site  {get; set;}
        public string EmployeeId { get; set; }
        public int? OnboardingId { get; set; }
        public string ExternalReferenceId { get; set; }
        public string IsPending { get; set; }
        public ComplianceStatus[] Statuses { get; set; }

    }

    public class Site
    {
        public string City { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string SiteCode { get; set; }
    }
}

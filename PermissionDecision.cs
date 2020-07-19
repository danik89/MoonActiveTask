using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCheckerService.DAL
{
    public class PermissionDecision
    {
        public string PlateNumber { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProhibitedReason Kind { get; set; }
        public Decision Decision { get; set; }
    }

    public enum Decision
    {
        Approved = 0,
        Declined = 1
    }

    public enum ProhibitedReason
    {
        PublicTransport = 0,
        MilitaryLaw = 1,
        CType = 2,
        OperatedByGas = 3,
        None = 4
    }
}

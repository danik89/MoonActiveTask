using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermissionCheckerService.DAL;
using System.Text.RegularExpressions;

namespace PermissionCheckerService.BL
{
    public class RulesChecker
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PermissionDecision CheckRules(string plateNumber)
        {
            _log.Debug("Checking the plateNumber with all standards");
            PermissionDecision _decision = new PermissionDecision();
            _decision.TimeStamp = DateTime.Now;
            _decision.Decision = Decision.Approved;
            _decision.Kind = ProhibitedReason.None;
            _decision.PlateNumber = plateNumber;
            if (CheckIfPublicTransport(plateNumber))//checking if the vehicle is public transport vehicle
            {
                _log.Debug(string.Format("Vehicle with plate number {0} is a public transportation vehicle, and its entrance is declined. time is {1}", plateNumber, _decision.TimeStamp.ToString()));
                _decision.Decision = Decision.Declined;
                _decision.Kind = ProhibitedReason.PublicTransport;
            }
            else if (CheckIfMilitaryLaw(plateNumber))//checking if the vehicle is military or law vehicle
            {
                _log.Debug(string.Format("Vehicle with plate number {0} is a military\\law enforcement vehicle, and its entrance is declined. time is {1}", plateNumber, _decision.TimeStamp.ToString()));
                _decision.Decision = Decision.Declined;
                _decision.Kind = ProhibitedReason.MilitaryLaw;
            }
            else if (CheckIfCType(plateNumber))//checking if it ends with 85,86,87,88,89,00
            {
                _log.Debug(string.Format("Vehicle with plate number {0} is declined due to its 2 last digits. time is {1}", plateNumber, _decision.TimeStamp.ToString()));
                _decision.Decision = Decision.Declined;
                _decision.Kind = ProhibitedReason.CType;
            }
            else if (CheckIfByGas(plateNumber))//checking if its operated by gas
            {
                _log.Debug(string.Format("Vehicle with plate number {0} is operated by gas, and its entrance is declined. time is {1}", plateNumber, _decision.TimeStamp.ToString()));
                _decision.Decision = Decision.Declined;
                _decision.Kind = ProhibitedReason.OperatedByGas;
            }
            if (_decision.Decision == Decision.Approved)
                _log.Debug(string.Format("Vehicle with plate number {0} has approved to enter the parking lot. time is {1}", plateNumber, _decision.TimeStamp.ToString()));
            return _decision;
        }

        private bool CheckIfByGas(string plateNumber)
        {
            _log.Debug("Checking if the vehicle is operated by gas");
            int num;
            if (plateNumber.Length == 7 || plateNumber.Length == 8)
            {
                 int.TryParse(plateNumber, out num);
                if (num > 0)
                    return num % 7 == 0;
                else
                {
                    var error = "Could not parse the string into numbers";
                    _log.Error(error);
                    throw new Exception(error);
                }
            }
            return false;
        }
        private bool CheckIfCType(string plateNumber)
        {
            _log.Debug("Checking if the vehicle is a C-type vehicle");//i just called it c-type vehicle instead of a long name :)

            return plateNumber.Length == 7 && (plateNumber.EndsWith("85") || plateNumber.EndsWith("86") ||
                plateNumber.EndsWith("87") || plateNumber.EndsWith("88") || plateNumber.EndsWith("89") ||
                plateNumber.EndsWith("00"));
        }

        private bool CheckIfMilitaryLaw(string plateNumber)
        {
            _log.Debug("Checking if the vehicle is a military\\law enforcement vehicle");

            return plateNumber.Any(x => char.IsLetter(x));
        }

        private bool CheckIfPublicTransport(string plateNumber)
        {
            _log.Debug("Checking if the vehicle is a public transportation vehicle");

            return plateNumber.EndsWith("25") || plateNumber.EndsWith("26");
        }
    }
}

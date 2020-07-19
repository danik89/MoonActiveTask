using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PermissionCheckerService.BL;
using log4net;
using System.Threading.Tasks;

namespace PermissionCheckerService
{
    public class PermissionCheckerService : IPermissionCheckerService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermissionCheckerLogic _permissionCheckerLogic;

        public PermissionCheckerService()
        {
            _permissionCheckerLogic = new PermissionCheckerLogic();
        }
        
        public async Task<bool> CheckPermission(string picturePath)
        {
            log.Debug("CheckPermission started...");
             return await _permissionCheckerLogic.HandlePermissionLogic(picturePath);//gets a result - to enter the vehicle or not
        } 
    } 
}

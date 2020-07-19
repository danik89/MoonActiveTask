using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionServiceRunner
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int result;

        static int Main(string[] args)
        {
            MainAsync().Wait();
            return result;
        }

        static async Task MainAsync()
        {
            string errorStr = string.Empty;
            log.Debug("PermissionCheckerService begins...");
            log.Debug("Creating an instance of  PermissionCheckerService");
            PermissionCheckerService.PermissionCheckerService service = new PermissionCheckerService.PermissionCheckerService();
            var picPath = ConfigurationSettings.AppSettings["picPath"];//getting the path of the image from the config file
            if (picPath != null)
            {
                string picString = picPath.ToString();
                if (picString != string.Empty)
                {
                    result = await service.CheckPermission(picString) ? 1 : 0;
                    if (result == 1)
                        log.Debug("The vehicle can enter the parking lot");
                    else
                        log.Debug("The vehicle can't enter the parking lot");
                }
                else
                {
                    errorStr = "Could not find value for the 'picPath' key in the appSettings section";
                    log.Error(errorStr);
                }
            }
            else
            {
                errorStr = "Could not find the 'picPath' key in the appSettings section";
                log.Error(errorStr);
            }
        }
    }
}

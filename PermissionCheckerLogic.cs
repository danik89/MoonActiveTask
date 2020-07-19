using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermissionCheckerService.DAL;
using System.IO;
using PermissionCheckerService.BL.APIHandler;

namespace PermissionCheckerService.BL
{
    public class PermissionCheckerLogic
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private RulesChecker _rulesChecker;
        private OCRHandler _apiHandler;
        private string _textResult;
        private PermissionCheckerDB _dbHandler;

        public PermissionCheckerLogic()
        {
			//constructor
            _rulesChecker = new RulesChecker();
            _apiHandler = new OCRHandler();
            _dbHandler = new PermissionCheckerDB();
        }

        internal async Task<bool> HandlePermissionLogic(string picPath)
        {
            string errorStr = string.Empty;
            _log.Debug("HandlePermissionLogic started...");
            var result = await CheckPermission(picPath); //checking for permission to enter a vehicle
            if (result != null)//we got some answer
            {
                try
                {
                    _dbHandler.LogInDB(result);//trying to log the result into the db
                }
                catch (Exception ex)
                {
                    errorStr = String.Format("Error while logging into database. Exception = {0}", ex.Message);
                    _log.Error(errorStr);
                }
                return result.Decision == Decision.Approved;//if decision is to approve - return true
            }
            else
                return false;
        }

        private async Task<PermissionDecision> CheckPermission(string picPath)
        {
            _log.Debug("CheckPermission in PermissionCheckerLogic started...");
            try
            {
                if (!checkIfValidPic(picPath))//checking if its a picture, if it exists and valid extension
                {
                    _log.Error("Could not continue parsing with the current path, exiting");
                    return null;
                }
                else
                {
                    var res = await _apiHandler.ParseImageToText(picPath);//calling the api handler to parse the image into text
                    if (res == 1)
                    {
                        _textResult = handleBadChars(_apiHandler.TextResult);//result has returned with spaces, comas and seperators that need to remove

                        if (!string.IsNullOrEmpty(_textResult))
                        {
                            return _rulesChecker.CheckRules(_textResult);//checking the text that we recieved with the entrance rules 
                        }
                        else
                        {
                            _log.Error("Could not get text from the picture");
                        }
                    }
                    else
                    {
                        _log.Error("An error occurd while parsing the image into text");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("An error occurd while parsing the image into text. Exception = {0}", ex.Message));
            }
            return null;
        }

        private string handleBadChars(string textResult)
        {//removing unwanted chars
            _log.Debug("Checking for bad chars");
            string tempStr = string.Empty;
            try
            {
                tempStr = textResult.Trim();
                int end = tempStr.IndexOf("\t");//checking the index of backslash t
                if (end >= 0)
                {
                    tempStr = tempStr.Remove(end);
                }
                tempStr = tempStr.Replace(":", string.Empty);
                tempStr = tempStr.Replace(" ", string.Empty);
                tempStr = tempStr.Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Exception: {0}", ex.Message));
                tempStr = string.Empty;
            }
            return tempStr;
        }

        private bool checkIfValidPic(string picture)
        {
            _log.Debug("Checking if the path provided exists and has the right extension");
            if (File.Exists(picture))//checking if file exists
            {
                _log.Debug("file exists!");
                var ext = Path.GetExtension(picture);//getting the extension
                _log.Debug(string.Format("Extension is {0}", ext));
                if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".jpeg"))//expected these extensions
                    return true;
                else
                    _log.Error("The file does not have the right extension for parsing");
            }
            else
                _log.Error("The file does not exists");
            return false;
        }
    }
}


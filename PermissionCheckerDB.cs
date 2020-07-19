using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCheckerService.DAL
{
    public class PermissionCheckerDB
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string connectionString;
        private string sql;

        public void LogInDB(PermissionDecision current)
        {
            string errorStr = string.Empty;

            if (getDataFromAppConfig())//retrieving data from config file - connection string and query
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    try
                    {
                        _log.Debug(string.Format("Opening connection for Connection string = {0}", connectionString));
                        cnn.Open();//opening connection to the database
                        _log.Debug("Connection has opened successfully");

                        using (SqlCommand cmd = new SqlCommand(sql, cnn))//creating a sql query with parameters
                        {
                            _log.Debug(string.Format("Setting parameters for the query"));
                            cmd.Parameters.Add("@num", SqlDbType.Char).Value = current.PlateNumber;
                            cmd.Parameters.Add("@time", SqlDbType.DateTime).Value = current.TimeStamp;
                            cmd.Parameters.Add("@reason", SqlDbType.Int).Value = current.Kind;
                            cmd.Parameters.Add("@decision", SqlDbType.Int).Value = current.Decision;

                            _log.Debug("Executing the query");
                            int rowsAdded = cmd.ExecuteNonQuery();
                            if (rowsAdded > 0)//checking if the insert succeeded
                                _log.Debug("Data was added to the database");
                            else
                                _log.Error("The data was not added to the database");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorStr = string.Format("Exception while inserting to database, exception: {0}", ex.Message);
                        _log.Error(errorStr);
                    }
                }
            }
        }


        private bool getDataFromAppConfig()
        {
            bool res = false;
            string errorStr;
            var connectionStr = ConfigurationSettings.AppSettings["connectionString"];
            if (connectionStr == null)
            {
                errorStr = "Could not find the 'connectionString' key in the appSettings section";
                _log.Error(errorStr);
            }
            else
            {
                connectionString = connectionStr.ToString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    errorStr = "Could not find value for the 'connectionString' key in the appSettings section";
                    _log.Error(errorStr);
                }
                else
                {
                    var query = ConfigurationSettings.AppSettings["insertQuery"];
                    if (query == null)
                    {
                        errorStr = "Could not find the 'insertQuery' key in the appSettings section";
                        _log.Error(errorStr);
                    }
                    else
                    {
                        sql = query.ToString();
                        if (string.IsNullOrEmpty(sql))
                        {
                            errorStr = "Could not find value for the 'insertQuery' key in the appSettings section";
                            _log.Error(errorStr);
                        }
                        else
                            res = true;
                    }
                }
            }
            return res;
        }
    }
}
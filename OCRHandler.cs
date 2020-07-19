using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCheckerService.BL.APIHandler
{
    public class OCRHandler
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string textResult;

        public string TextResult
        {
            get { return textResult; }
        }

        public async Task<int> ParseImageToText(string imagePath)
        {
            try
            {
                string errorStr = string.Empty;
                _log.Debug("ParseImageToText started...");
                _log.Debug("Creating the httpClient");
                HttpClient httpClient = new HttpClient();//creating an http client
                httpClient.Timeout = new TimeSpan(1, 1, 1);

                _log.Debug("Creating the message to send");
                MultipartFormDataContent form = new MultipartFormDataContent();
                var apiKey = ConfigurationSettings.AppSettings["apiKey"];//getting the api ket from the config file
                if (apiKey != null)
                {
                    var apiString = apiKey.ToString();
                    if (apiString == string.Empty)
                    {
                        errorStr = "Could not find value for the 'apiKey' key in the appSettings section";
                        _log.Error(errorStr);
                        throw new Exception(errorStr);
                    }
                    else
                    {
                        _log.Debug(string.Format("api key from App.config is: {0}", apiString));
                        form.Add(new StringContent(apiString), "apikey"); //Adding all paremters for the api
                        form.Add(new StringContent("eng"), "language");
                        form.Add(new StringContent("2"), "ocrengine");
                        form.Add(new StringContent("true"), "scale");
                        form.Add(new StringContent("true"), "istable");

                        if (string.IsNullOrEmpty(imagePath) == false)//checking if path is ok
                        {
                            _log.Debug("Reading all bytes from the image provided");
                            byte[] imageData = File.ReadAllBytes(imagePath);//reading all bytes from the image
                            form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                        }

                        _log.Debug(String.Format("Calling PostAsync to the API at {0} with the message", "https://api.ocr.space/parse/image"));
                        HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/parse/image", form);//calling post 

                        _log.Debug(string.Format("Response status code: {0}, response success status code: {1}", response.StatusCode,
                            response.IsSuccessStatusCode));
                        _log.Debug("Reading all response's content");
                        string strContent = await response.Content.ReadAsStringAsync();
                        _log.Debug("Deserializing the content");
                        Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

                        if (ocrResult.OCRExitCode == 1)//result is ok - starting to deserialize content
                        {
                            _log.Debug("Successfully deserialized the content");
                            for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
                            {
                                textResult = textResult + ocrResult.ParsedResults[i].ParsedText;
                            }
                            _log.Debug(string.Format("Text result is {0}", textResult));
                            return 1;
                        }
                        else
                        {
                            errorStr = "Content could not be deserialized";
                            _log.Error(errorStr);
                            throw new Exception(errorStr);
                        }
                    }
                }
                else
                {
                    errorStr = "Could not find 'apiKey' key in the appSettings section";
                    _log.Error(errorStr);
                    throw new Exception(errorStr);
                }
            }
            catch (Exception exception)
            {
                _log.Error(String.Format("Exception: {0}", exception.Message));
                return 0;
            }
        }
    }
}

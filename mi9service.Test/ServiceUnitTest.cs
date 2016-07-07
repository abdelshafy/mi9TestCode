using AwesomeService.Controllers;
using AwesomeService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AwesomeService.Test
{
    /// <summary>
    /// Service Test Suit
    /// </summary>
    [TestClass]
    public class ServiceUnitTest
    {
        /// <summary>
        /// Should Return Http Status Bad Request 400 For NullInput
        /// </summary>
        [TestMethod]
        public void Should_Return_Http_Status_Bad_Request_400_For_Null_Input()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;            
            
            // Act            
            var response = controller.Post(null);
            var result = response.Content.ReadAsAsync<Error>().Result;
            var statusCode = response.StatusCode;
            
            // Assert            
            Assert.IsNotNull(statusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
            Assert.IsTrue(result.error.Contains("Could not decode request"));
        }

       /// <summary>
        /// Should_Return_Http_Status_Bad_Request_400_For_Input_Without_Payload
       /// </summary>
        [TestMethod]   
        public void Should_Return_Http_Status_Bad_Request_400_For_Input_Without_Payload()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;
            var input = DataWithoutPayload();
            // Act            
            var response = controller.Post(input);
            var result = response.Content.ReadAsAsync<Error>().Result;
            var statusCode = response.StatusCode;

            // Assert            
            Assert.IsNotNull(statusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
            Assert.IsTrue(result.error.Contains("Could not decode request"));
        }

        /// <summary>
        /// Should_Return_Error_Message_Could_Not_Decode_For_Invalid_Input
        /// </summary>
        [TestMethod]
        public void Should_Return_Error_Message_Could_Not_Decode_For_Invalid_Input()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;
            var input = DataWithoutPayload();
            // Act            
            var response = controller.Post(input);
            var result = response.Content.ReadAsAsync<Error>().Result;
            var statusCode = response.StatusCode;

            // Assert            
            Assert.IsTrue(result.error.Contains("Could not decode request"));
        }

        /// <summary>
        /// Should_Return_HTTP_Status_OK_For_Valid_Input
        /// </summary>
        [TestMethod]
        public void Should_Return_HTTP_Status_OK_For_Valid_Input()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;
            var input = InputWithTwoShowsMatchTheCriteria();
            // Act            
            var response = controller.Post(input);
            var result = response.Content.ReadAsAsync<Output>().Result;
            var statusCode = response.StatusCode;
            // Assert            
            Assert.IsNotNull(statusCode);
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
        }

        /// <summary>
        /// Should_Return_Valid_JSON_Response_For_Valid_Input
        /// </summary>
        [TestMethod]
        public void Should_Return_Valid_JSON_Response_For_Valid_Input()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;            
            var input = InputWithTwoShowsMatchTheCriteria();

            // Act
           //Debug.WriteLine(input);
            var response = controller.Post(input);
            var result = response.Content.ReadAsAsync<Output>().Result;
            var statusCode = response.StatusCode;
            // Assert           
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.response.Count);
        }

        /// <summary>
        /// Should_Return_7_Valid_Results_From_The_Exam_Sample_Input
        /// </summary>
        [TestMethod]
        public void Should_Return_7_Valid_Results_From_The_Exam_Sample_Input()
        {
            // Arrange
            ServiceController controller = new ServiceController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            controller.Request.Method = HttpMethod.Post;
            var input = ReadExamSampleRequest();
            Debug.WriteLine("Input -->" + JsonConvert.SerializeObject(input));
            // Act
            var response = controller.Post(input);
            var result = response.Content.ReadAsAsync<Output>().Result;
            var statusCode = response.StatusCode;
            // Assert           
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.response);
            Assert.AreEqual(7, result.response.Count);

            Debug.WriteLine("Output -->" + JsonConvert.SerializeObject(result, formatting: Formatting.Indented));
        }

        /// <summary>
        /// Reads the sample codign challenge json and pass it to the test methods
        /// </summary>
        /// <returns></returns>
        private Show ReadExamSampleRequest()
        {
            #region ReadExamSampleRequest
            //using (StreamReader sr = new StreamReader("../../SampleJSON.txt"))
            //{
            //    String json = sr.ReadToEnd();
            //    return JsonConvert.DeserializeObject<Input>(json);
            //}
            var json = new WebClient().DownloadString("https://raw.githubusercontent.com/mi9/coding-challenge-samples/master/sample_request.json");
            return JsonConvert.DeserializeObject<Show>(json);
            #endregion     
        }

        /// <summary>
        /// another sample good data for testing
        /// </summary>
        /// <returns></returns>
        private Show InputWithTwoShowsMatchTheCriteria()
        {
            #region InputWithTwoShowsMatchTheCriteria - Good Data
            return new Show
                {
                    payload = new List<Payload>()  {
                   new Payload { 
                    country = "Australia",
                    description = "",
                    drm = true,
                    episodeCount = 2,
                    genre = "",
                    image = new Image {
                       showImage = ""
                   },
                    language = "",
                    nextEpisode =  new NextEpisode{
                       channel = "",
                       channelLogo = "",
                       date = null,
                       html = "",
                       url = "http://go.ninemsn.com.au/"
                    },
                    primaryColour = "",
                    seasons = new List<Season>(){
                       new Season {
                        slug =  "show/thetaste/season/1"
                       }
                    },
                    slug = "",
                    title = "",
                    tvChannel = ""
                   },

                   new Payload { 
                    country = "Australia",
                    description = "",
                    drm = true,
                    episodeCount = 1,
                    genre = "",
                    image = new Image {
                       showImage = ""
                   },
                    language = "",
                    nextEpisode =  new NextEpisode{
                       channel = "",
                       channelLogo = "",
                       date = null,
                       html = "",
                       url = "http://go.ninemsn.com.au/"
                    },
                    primaryColour = "",
                    seasons = new List<Season>(){
                       new Season {
                        slug =  "show/thetaste/season/1"
                       }
                    },
                    slug = "",
                    title = "",
                    tvChannel = ""
                   },

                }

                }; 
            #endregion

        }

        /// <summary>
        /// Sample bad data for testing.
        /// </summary>
        /// <returns></returns>
        private Show DataWithoutPayload()
        {
            #region DataWithoutPayload
            return new Show
                {
                    totalRecords = 0
                }; 
            #endregion
        }



    }
}

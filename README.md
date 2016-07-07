Welcome to mi9service 
=====================


#### mi9service is written in C# using ASP.NET Web Api for mi9 coding challenge.

This repository contains 2 projects ( service and unit test). in addition to an example of service use and technical documents. 

* mi9service project

mi9service is the actual implementation of the service as per specification on []()
The project is hosted on  AppHarbor in the following address and automatically gets updated with any changes to this Git repository.
The project is also automatically being compiled on a Continuous Integration server () and the following badge shows the live build status of the project including the unit test results.
[![Build status](https://ci.appveyor.com/api/projects/status/lwu2aha3rmp44xqm)](https://ci.appveyor.com/project/mehdiromi/mi9service)
[![Test Results](https://ci.appveyor.com/api/projects/status/lwu2aha3rmp44xqm)](https://ci.appveyor.com/project/mehdiromi/mi9service/build/tests)

* mi9service.Test

mi9service.Test is the unit test project for testing the service. The test result is available in the following address:
[https://ci.appveyor.com/project/mehdiromi/mi9service/build/tests](https://ci.appveyor.com/project/mehdiromi/mi9service/build/tests)

##### There is also a Node.js service developed for testing the service from another server. The Node.js project is located in a separate repository in the following URL:  [https://github.com/mehdiromi/mi9client](https://github.com/mehdiromi/mi9client)
The client is also hosted in Heroku in the following address:  [http://mi9client.herokuapp.com](http://mi9client.herokuapp.com).
The client is developed for the purpose of e2e testing the service using Node.js on Express, Jade and Jquery.

#### Service details:
The service is hosted at appharbor.com in the following address:  [http://awesomeservice.apphb.com](http://awesomeservice.apphb.com).  The service automatically reads the github repo and re-compile and deploy the project whenever there is any new commit.


#### How does the service work?

The service is web api controller that is only configured to accept requests from everywhere on the internet ( for the purposes of this exam) and allows Cross Origin resource sharing.

The controller is simply reads an input param mapped exactly to the sample JSON request and try pass it to `ShowsWithDrmAndAtleastOneEpisode` method to filter the inputs ( drm = true  and episodeCount > 0)  using LINQ and returns a new type with the valid data.

The application has 3 models for this project:  Error, Output, Show

###### Error
This is the error type which returns bad request as http status code.

```
    /// <summary>
    /// Error type for returning error as Http Response.
    /// </summary>
    public class Error
    {
        public string error { get; set; }
    }
```

###### Output
Is the valid output when application returns OK status code :
```
    /// <summary>
    /// The response type. includes image, slug and title.
    /// </summary>
    public class Response
    {
        public string image { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
    }

    /// <summary>
    /// Wrapper for the Response
    /// </summary>
    public  class Output
    {
        public List<Response> response { get; set; }
    }
```

###### Show
This is the request class containing a list of payloads:

```
    /// <summary>
    /// Image type
    /// </summary>
    public class Image
    {
        public string showImage { get; set; }
    }
    /// <summary>
    /// NextEpisode type
    /// </summary>
    public class NextEpisode
    {
        public object channel { get; set; }
        public string channelLogo { get; set; }
        public object date { get; set; }
        public string html { get; set; }
        public string url { get; set; }
    }

    /// <summary>
    /// Season type used in Payload
    /// </summary>
    public class Season
    {
        public string slug { get; set; }
    }

    /// <summary>
    /// The request payload type
    /// </summary>
    public class Payload
    {
        public string country { get; set; }
        public string description { get; set; }
        public bool drm { get; set; }
        public int episodeCount { get; set; }
        public string genre { get; set; }
        public Image image { get; set; }
        public string language { get; set; }
        public NextEpisode nextEpisode { get; set; }
        public string primaryColour { get; set; }
        public List<Season> seasons { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public string tvChannel { get; set; }
    }

    /// <summary>
    /// The input show 
    /// </summary>
    public class Show
    {
        public List<Payload> payload { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public int totalRecords { get; set; }
    }
```

#### Exception handling
If there is an exception in mapping the JSON to C# class or invalid data,  the controller returns an Error type :

```
 return Request.CreateResponse<Error>(HttpStatusCode.BadRequest, new Error {
                    error = "Could not decode request: JSON parsing failed"
                });
				
				
```


If everything is ok, the controller returns OK response as below:

```
 return Request.CreateResponse<Output>(HttpStatusCode.OK, 
	ShowsWithDrmAndAtleastOneEpisode(input)
	);
```

The actual implementation of the controller :
 
```
   /// <summary>
    /// ServiceController is the main Web API for the coding challenge.
    /// This Web Api is served on the root path of the service and is an standalone service.
    /// For this exercise, access from all origins with all REST verbs is allowed however the service only implements POST.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServiceController : ApiController
    {
        /// <summary>
        /// Post action method receives a HttpRequestMessage with a content of type Show and returns HttpResponseMessage with of type Response.
        /// </summary>
        /// <param name="input">JSON input of the type show</param>
        /// <returns>HttpResponseMesage with Response or Error types</returns>
        [HttpPost]
        public HttpResponseMessage Post(Show input)
        {
            if (input == null)
            {
                return Request.CreateResponse<Error>(HttpStatusCode.BadRequest, 
					new Error {
						error = "Could not decode request: JSON parsing failed"
                });
            }
            try
            {
                return Request.CreateResponse<Output>(HttpStatusCode.OK, 
					ShowsWithDrmAndAtleastOneEpisode(input));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<Error>(HttpStatusCode.BadRequest, 
					new Error {
						error = "Could not decode request: " + ex.Message
                });
            }
        }

        /// <summary>
        /// The cofing challenge logic is implemented in this method.
        /// This method uses LINQ to Object to filter the shows with DRM true and episodeCount > 0
        /// </summary>
        /// <param name="input">A de-serialized object of shows  equivalent to the input JSON.</param>
        /// <returns>An object of type Output with a property of response of image, slug and title fields from the input.</returns>
        private Output ShowsWithDrmAndAtleastOneEpisode(Show input)
        {
            var data = (from p in input.payload
                            where
                                p.drm &&
                                p.episodeCount > 0
                            select new Response
                            {
                                image = p.image.showImage,
                                slug = p.slug,
                                title = p.title
                            }
                        ).ToList();
            return new Output
            {
                response = data
            };
        }
       
    }
```

#### Routing 
The default routing is changed to redirect all requests to the root of the service 

```
public static class WebApiConfig
    {
        /// <summary>
        /// Registers the configuration
        /// Here Cross-origin resource sharing needs to be enabled.
        /// Default routing to the API is also configured here.
        /// </summary>
        /// <param name="config">HttpConfiguration</param>
        public static void Register(HttpConfiguration config)
        {
            //Cross-origin resource sharing 
            config.EnableCors();
            
            //Default Routing
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "service", 
						id = RouteParameter.Optional }
            );
        }
    }
```

#### Installation
 * If you are hosting on AppHarbor.com, just clone this repository and the appharbor will look after everything else.
 * If you building on the cloud services such as Appveyor or Azure, In the setting > build > Before build script,  add the following `nuget restore` so the build service will restore all necessary packages.
 * On local machine, please clone the repository and open the solution with visual studio and build. the solution is configured to automatically download all required packages


#### Technical documentation
 There is a technical documentation is automatically generated from the comments in the project by free tools [http://sandcastle.codeplex.com/](http://sandcastle.codeplex.com/).
 The document is in html format and is available in [https://github.com/mehdiromi/mi9service/tree/master/TechnicalDocument/Help](https://github.com/mehdiromi/mi9service/tree/master/TechnicalDocument/Help)

 
 
 
This application is written by Mehdi Romi (mehdi.romi@gmail.com) and uses free resources for development and hosting.


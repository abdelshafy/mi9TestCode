using System.Collections.Generic;

namespace AwesomeService.Models
{
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
   
}   
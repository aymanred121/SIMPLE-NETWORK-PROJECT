using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
        //    throw new NotImplementedException();

           
            string statueLine = GetStatusLine(code);

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string headLine;
            headerLines.Add($"Content-Type: {contentType}");
            headerLines.Add("\r\n");
            headerLines.Add($"Content-Length: {content.Length}");
            headerLines.Add("\r\n");
            headerLines.Add($"Date: {DateTime.Now}");
            headerLines.Add("\r\n");
            if (redirectoinPath != "")
            {
                headerLines.Add($"Location: {Path.Combine(Configuration.RootPath, redirectoinPath)}");
            headerLines.Add("\r\n");
            }
            headLine = string.Join("", headerLines);

            // TODO: Create the request string
             responseString =statueLine+"\r\n"+ headLine+ content;
            new Request(responseString);
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = HTTPVersion.HTTP11 +" "+ ((int)code) +" "+ code.ToString();
            return statusLine;
        }
    }
}

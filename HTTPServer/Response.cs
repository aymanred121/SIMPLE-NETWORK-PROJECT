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
            throw new NotImplementedException();


            string statueLine = GetStatusLine(code);

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string headerLine = contentType + ',' + content.Length + ',' + DateTime.Now.ToString() + redirectoinPath;

            // TODO: Create the request string
            String requsetString =statueLine+ headerLine + '\n' + content;
            new Request(requsetString);
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = HTTPVersion.HTTP10.ToString() + code + code.ToString();
            return statusLine;
        }
    }
}

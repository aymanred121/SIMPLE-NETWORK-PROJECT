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
                headerLines.Add($"Location: {redirectoinPath}");
                headerLines.Add("\r\n");
            }
            headLine = string.Join("", headerLines);

            responseString = statueLine + "\r\n" + headLine + "\r\n" + content;
            // TODO: Create the request string

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string codeMsg = "";
            switch (code)
            {
                case StatusCode.BadRequest:
                    codeMsg = "Bad Request";
                    break;
                case StatusCode.InternalServerError:
                    codeMsg = "Internal Server Error";
                    break;
                case StatusCode.NotFound:
                    codeMsg = "Not Found";
                    break;
                case StatusCode.OK:
                    codeMsg = "OK";
                    break;
                case StatusCode.Redirect:
                    codeMsg = "Moved Permanently";
                    break;
            }
            string statusLine = Configuration.ServerHTTPVersion + " " + ((int)code) + " " + codeMsg;
            return statusLine;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            // throw new NotImplementedException();
            bool isPerfect = true;
            //TODO: parse the receivedRequest using the \r\n delimeter   
            contentLines = requestString.Split(new char[] { '\r', '\n' });
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
                return false;

            // Parse Request line
            isPerfect = ParseRequestLine();
            if (!isPerfect) return false;
            // Validate blank line exists
            isPerfect = ValidateBlankLine();
            if (!isPerfect) return false;
            // Load header lines into HeaderLines dictionary
            isPerfect = LoadHeaderLines();
            return isPerfect;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            requestLines = contentLines[0].Split(' ');
            if (ValidateIsURI(requestLines[1]))
            {
                //currently only GET is implemented
                try
                {
                switch (requestLines[0])
                {
                    case "GET":
                        method = RequestMethod.GET;
                        break;
                    case "POST":
                     method = RequestMethod.POST;
                     throw new Exception("Only GET is implemented");
                    case "HEAD":
                        method = RequestMethod.HEAD;
                        throw new Exception("Only GET is implemented");
                    default:
                      throw new Exception("Only GET is implemented");
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                    return false;
                }
                relativeURI = requestLines[1];
                //Remove '/' at the start so the path could be corrent
                relativeURI = relativeURI.TrimStart('/');
                switch (requestLines[2])
                {
                    case "HTTP/1.1":
                        httpVersion = HTTPVersion.HTTP11;
                        break;
                    case "HTTP/1.0":
                        httpVersion = HTTPVersion.HTTP10;
                        break;
                    default:
                        httpVersion = HTTPVersion.HTTP09;
                        break;
                }
                return true;
            }
            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < contentLines.Length; i++)
            {
                if (contentLines[i].Count()<2)
                    continue;
                string[] headers = contentLines[i].Split(':');
                headerLines.Add(headers[0], headers[1]);
       
            }
            return headerLines.Count != 0;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            if (contentLines.Last() == string.Empty) return true;
            return false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket client = this.serverSocket.Accept();
                Console.WriteLine($"New client accepted: {client.RemoteEndPoint}");
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(client);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            byte[] data;
            int receivedLength;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine($"clinet {clientSock.RemoteEndPoint} has ended connection");
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request clientRequset = new Request(Encoding.ASCII.GetString(data).TrimEnd('\0'));
                    // TODO: Call HandleRequest Method that returns the response
                   Response response= HandleRequest(clientRequset);

                    // TODO: Send Response back to client
                         byte[] msgResponse = Encoding.ASCII.GetBytes(response.ResponseString);

                    clientSock.Send(msgResponse);
                    //  clientSock.Send();
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    Console.WriteLine(ex.ToString());
                }
               
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
          //  throw new NotImplementedException();
            StatusCode code = StatusCode.OK;
            string content;
            try
            {
                //TODO: check for bad request 
               if(!request.ParseRequest())
                {
                    code = StatusCode.BadRequest;
                    string badRequest = LoadDefaultPage("BadRequest.html");
                  return new Response(StatusCode.BadRequest,"text/html",badRequest,"");
                    //this is a bad request
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath =Path.Combine( Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string redirctedPath=GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirctedPath.Length != 0)
                {
                    code = StatusCode.Redirect;
                    return new Response(code, "text/html","", redirctedPath);
                }

                //TODO: check file exists
                
                if (!File.Exists(physicalPath))
                {
                    code = StatusCode.NotFound;
                 //   redirctedPath = Path.Combine( Configuration.RootPath, "Notfound.html");
                   string notFoundPage= LoadDefaultPage("Notfound.html");
                    return new Response(code, "text/html", notFoundPage, "");
                }
                //TODO: read the physical file
                StreamReader sr;
                sr = new StreamReader(physicalPath);
                string pageContent = sr.ReadToEnd();
                // Create OK response
                return new Response(code, "text/html",pageContent,"");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                return new Response(StatusCode.InternalServerError, "text/html", "","");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string newPath;
           Configuration.RedirectionRules.TryGetValue(relativePath,out newPath);
            if (newPath == null)
                return "";
            return newPath;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try{
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            // else read file and return its content
            return new StreamReader(filePath).ReadToEnd();
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string rules = new StreamReader(filePath).ReadToEnd();
                string[] listOfRules = rules.Split(new char[] {'\r','\n'});
                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach(var rule in listOfRules)
                {
                    if (rule == "")
                        continue;
                   string[] lists = rule.Split(',');
                   Configuration.RedirectionRules.Add(lists[0],lists[1]);
                }
               
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}

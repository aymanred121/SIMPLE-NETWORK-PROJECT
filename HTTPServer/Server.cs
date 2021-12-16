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
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
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
            string welcome = "Welcome to my test server";
            byte[] data = Encoding.ASCII.GetBytes(welcome);
            clientSock.Send(data);
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
                    Request clientRequset = new Request(Encoding.ASCII.GetString(data));
                    Console.WriteLine("Received: {0} from Client: {1}" ,Encoding.ASCII.GetString(data, 0, receivedLength), clientSock.RemoteEndPoint);
                    // TODO: Call HandleRequest Method that returns the response
                   Response response= HandleRequest(clientRequset);
                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.));

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
            throw new NotImplementedException();
            StatusCode code;
            string content;
            try
            {
                //TODO: check for bad request 
               if(!request.ParseRequest())
                {
                    code = StatusCode.BadRequest;
                    return new Response(StatusCode.BadRequest,"text/html","","");
                    //this is a bad request
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Configuration.RootPath + request.relativeURI;
                //TODO: check for redirect
                string redirctedPath=GetRedirectionPagePathIFExist(request.relativeURI);
                string redircationPath = "";
                if (redirctedPath.Length != 0)
                {
                    code = StatusCode.Redirect;
                    redircationPath = Configuration.RootPath + request.relativeURI+redirctedPath;
                  
                }

                //TODO: check file exists
                
                if (!File.Exists(physicalPath))
                {
                    code = StatusCode.NotFound;
                    redircationPath = Configuration.RootPath+ "Notfound.html";
                }
                //TODO: read the physical file
                StreamReader sr;
                if (redircationPath.Length == 0)
                    sr = new StreamReader(physicalPath);
                else
                    sr = new StreamReader(redircationPath);
                string pageContent = sr.ReadToEnd();
                // Create OK response
                return new Response(code, "text/html","",redircationPath);
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
            if(newPath == null)
            return string.Empty;
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
                string[] listOfRules = rules.Split('\n');
                // then fill Configuration.RedirectionRules dictionary
                foreach(string rule in listOfRules)
                {
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

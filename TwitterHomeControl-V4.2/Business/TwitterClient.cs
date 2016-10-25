using System;
using GHIElectronics.NETMF.FEZ;
using TwitterHomeControl.Utilitarios;
using Networking;

namespace TwitterHomeControl.Business
{
    public class TwitterClient
    {
        // Base64 string of username and password
        public string B64AuthenticateString { get; set; }

        /// <summary>
        /// Create new TwitterClient instance
        /// </summary>
        /// <param name="username">Twitter Username</param>
        /// <param name="password">Twitter Password</param>
        public TwitterClient(string username, string password)
        {
            // Convert username and password into Base64 string 
            // Used for basic HTTP authentication
            B64AuthenticateString = Base64Encode(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
        }
        
        // Base64 Encode data
        public static string Base64Encode(byte[] bytesToEncode)
        {
            const String b64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

            var ret = "";
            var b = 0;
            var i = 0;
            var inLen = bytesToEncode.Length;
            var charArray3 = new byte[3];
            var charArray4 = new byte[4];

            while ((inLen--) != 0)
            {
                charArray3[i++] = bytesToEncode[b++];
                if (i != 3) continue;
                charArray4[0] = (byte)((charArray3[0] & 0xfc) >> 2);
                charArray4[1] = (byte)(((charArray3[0] & 0x03) << 4) + ((charArray3[1] & 0xf0) >> 4));
                charArray4[2] = (byte)(((charArray3[1] & 0x0f) << 2) + ((charArray3[2] & 0xc0) >> 6));
                charArray4[3] = (byte)(charArray3[2] & 0x3f);

                for (i = 0; (i < 4); i++)
                {
                    ret += b64Chars[charArray4[i]];
                }

                i = 0;
            }

            if (i != 0)
            {
                int j;
                for (j = i; j < 3; j++)
                    charArray3[j] = 0;

                charArray4[0] = (byte)((charArray3[0] & 0xfc) >> 2);
                charArray4[1] = (byte)(((charArray3[0] & 0x03) << 4) + ((charArray3[1] & 0xf0) >> 4));
                charArray4[2] = (byte)(((charArray3[1] & 0x0f) << 2) + ((charArray3[2] & 0xc0) >> 6));
                charArray4[3] = (byte)(charArray3[2] & 0x3f);

                for (j = 0; (j < i + 1); j++)
                    ret += b64Chars[charArray4[j]];

                while ((i++ < 3))
                    ret += '=';
            }

            return ret;
        }
        
        // Find first position of an byte array in an other byte array
        static int FindArray(byte[] needle, byte[] haystack)
        {
            int needlePos = 0;
            int haystakPos;

            for (haystakPos = 0; haystakPos < haystack.Length; haystakPos++)
            {
                if (haystack[haystakPos] == needle[needlePos])
                    needlePos++;
                else
                    needlePos = 0;

                if (needlePos == needle.Length)
                {
                    return (haystakPos - needlePos + 1);
                }
            }

            return -1;

        }

        private static string BuscaHeader(string url, string referer, 
                                          string extraHeaders, string requestMethod, 
                                          string postContentType, string postBody)
        {
            // Get Server
            var server = url.Substring(0, url.IndexOf("/"));

            // Get Path
            var path = url.Substring(url.IndexOf("/"));

            // Get Port
            if (server.IndexOf(':') != -1)
            {
                var sServer = server.Split(new[] { '|' });
                server = sServer[0];
                //Funcoes.ToInt(sServer[1]);
            }

            var requestHeader = requestMethod + " " + path + " HTTP/1.0\r\n";                          // Request URI           
            requestHeader += "Host: " + server + "\r\n";                                                  // HTTP1.0 Host
            requestHeader += "Connection: Close\r\n";                                                     // HTTP1.0 Close connection after request
            requestHeader += "Pragma:	no-cache\r\n";                                                    // HTTP1.0 No caching support
            requestHeader += "Cache-Control: no-cache\r\n";                                               // HTTP1.0 No caching support
            requestHeader += "User-Agent: GM862 (.NET Micro Framework 4.0)\r\n";                          // HTTP1.0 User Agent

            if (referer != string.Empty)
            {
                requestHeader += "Referer: " + referer + "\r\n";                                          // HTTP1.0 Referer
            }

            if ((requestMethod == "POST") & (postContentType != string.Empty))
            {
                requestHeader += "Content-Type: " + postContentType + "\r\n";
            }

            if (extraHeaders != string.Empty)
            {
                requestHeader += extraHeaders;
            }

            if (postBody != string.Empty)
            {
                requestHeader += "Content-Length: " + postBody.Length + "\r\n";
                requestHeader += "\r\n";
                requestHeader += postBody;
            }
            else
            {
                requestHeader += "\r\n";
            }

            return requestHeader;
        }

        private static string RetiraHeader(string receive)
        {
            byte[] retorno = RetiraHeader(System.Text.Encoding.UTF8.GetBytes(receive));
            return Funcoes.BytesToString(retorno);
        }
                
        private static byte[] RetiraHeader(byte[] receive)
        {
            var endOfHeader = new byte[] {13, 10, 13, 10};
            //System.Text.Encoding.UTF8.GetChars(endOfHeader);
            byte[] responseHeaderRaw;
            
            var headerEnd = FindArray(endOfHeader, receive);
            //while (headerEnd != -1)
            if (headerEnd != -1)
            {
                // Copy data from header
                responseHeaderRaw = new byte[headerEnd];
                Array.Copy(receive, responseHeaderRaw, responseHeaderRaw.Length);

                // Strip header from response
                var responseTemp = new byte[receive.Length - headerEnd - 4];
                Array.Copy(receive, headerEnd + 4, responseTemp, 0, responseTemp.Length);
                receive = responseTemp;

                //headerEnd = FindArray(endOfHeader, receive);
            }

            return receive;
        }

        //private static byte[] RetiraHeaderWebService(byte[] receive)
        //{
        //    //<string
        //    var endOfHeader = new byte[] { 60, 115, 116, 114, 105, 110, 103 };
        //    byte[] responseHeaderRaw;

        //    var headerEnd = FindArray(endOfHeader, receive);
            
        //    if (headerEnd != -1)
        //    {
        //        // Copy data from header
        //        responseHeaderRaw = new byte[headerEnd];
        //        Array.Copy(receive, responseHeaderRaw, responseHeaderRaw.Length);

        //        // Strip header from response
        //        var responseTemp = new byte[receive.Length - headerEnd];
        //        Array.Copy(receive, headerEnd, responseTemp, 0, responseTemp.Length);
        //        receive = responseTemp;
        //    }

        //    return receive;
        //}

        public static void WebPost(string url, string referer,
                                   string extraHeaders, string requestMethod,
                                   string postContentType, string postBody)
        {
            //Trata Url
            url = url.Substring(7);

            string destIp = "184.172.63.50";

            try
            {
                //Realiza conexão com o socket
                EthernetW5100.ConnectTCP(destIp, 80);
                System.Threading.Thread.Sleep(1000); //1 segundos

                //Constroi o Header da mensagem
                var request = BuscaHeader(url, referer, extraHeaders, requestMethod, postContentType, postBody);
                
                //Envia requisição
                EthernetW5100.Send(request);

                //Aguarda os dados
                var timeoutInicial = DateTime.Now.AddSeconds(30);
                while (EthernetW5100.Available <= 0 && DateTime.Now < timeoutInicial)
                {
                    System.Threading.Thread.Sleep(500); //0,5 segundo
                }

            }
            finally
            {
                try
                {
                    //Desconecta o socket
                    EthernetW5100.Disconnect();
                }
                catch (Exception) { }
            }

        }
        
        public static string WebRequest(string url, string referer,
                                        string extraHeaders, string requestMethod,
                                        string postContentType, string postBody)
        {
            string mensagemRetorno = "";

            //Trata Url
            url = url.Substring(7);
            
            System.Threading.Thread.Sleep(1000); //1 segundo

            string response;
            string destIp = "184.172.63.50";
            
            try
            {
                //Realiza conexão com o socket                
                EthernetW5100.ConnectTCP(destIp, 80);
                System.Threading.Thread.Sleep(1000); //1 segundos

                //Constroi o Header da mensagem
                var request = BuscaHeader(url, referer, extraHeaders, requestMethod, postContentType, postBody);

                EthernetW5100.Send(request);

                //Aguarda os dados
                var timeoutInicial = DateTime.Now.AddSeconds(30);
                while (EthernetW5100.Available <= 0 && DateTime.Now < timeoutInicial)
                {
                    System.Threading.Thread.Sleep(500); //0,5 segundo
                }
                                               
                bool finalMensagem = false;
                var timeoutPrincipal = DateTime.Now.AddSeconds(60);

                //Realiza Loop até ler a mensagem completa
                do
                {
                    response = EthernetW5100.Receive();
                    if (response != "")
                        mensagemRetorno = mensagemRetorno + RetiraHeader(response);

                    //Verifica se obteve final da mensagem ou se é método de envio
                    if (response.IndexOf("</string>") > 0)
                        finalMensagem = true;


                } while (DateTime.Now < timeoutPrincipal && finalMensagem == false);


            }
            finally
            {
                try
                {
                    //Desconecta o socket
                    EthernetW5100.Disconnect();
                } catch (Exception){}
            }

            return mensagemRetorno;
        }

                
    }
}

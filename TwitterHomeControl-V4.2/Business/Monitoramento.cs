using System;
using GHIElectronics.NETMF.FEZ;
using TwitterHomeControl.Utilitarios;
using Networking;

namespace TwitterHomeControl_V4._1.Business
{
    class Monitoramento
    {
        public static bool Ping(string ipDestino)
        {
            var retorno = false;
            try
            {
                //Realiza conexão com o socket                
                EthernetW5100.ConnectTCP(ipDestino, 80);
                System.Threading.Thread.Sleep(1000); //1 segundos

                retorno = true;
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

            return retorno;
        }
    }
}

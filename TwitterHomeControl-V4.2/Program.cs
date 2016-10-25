#region "codigo comentario BKP"
//using System;
//using System.IO;
//using System.Threading;
//using Microsoft.SPOT;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using Microsoft.SPOT;
//using Microsoft.SPOT.Cryptography;
//using TwitterHomeControl.Utilitarios;
//using USBizi_Ethernet;
//using System.Threading;
//using Microsoft.SPOT.Hardware;
//using System.Collections;

//Ligar terra no GND (Ground = Terra)

//var executa = 0;
//if (executa > 0)
//{
//    var pino2 = new OutputPort((Cpu.Pin) FEZ_Pin.Digital.Di2, true);
//    pino2.Write(false);
//    pino2.Write(true);

//    var pino4 = new OutputPort((Cpu.Pin) FEZ_Pin.Digital.Di4, true);
//    pino4.Write(false);
//    pino4.Write(true);

//    var pino7 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, true);
//    pino7.Write(false);
//    pino7.Write(true);

//    var pino8 = new OutputPort((Cpu.Pin) FEZ_Pin.Digital.Di8, true);
//    pino8.Write(false);
//    pino8.Write(true);

//    var pino13 = new OutputPort((Cpu.Pin) FEZ_Pin.Digital.Di13, true);
//    pino13.Write(false);
//    pino13.Write(true);

//}

//Funcoes.EscreverLog(wrkPath, "Sistema Inicializado.", 0, true);

//if (wrkPath != string.Empty)
//{
//    string[] wrkArquivos = Directory.GetFiles(wrkPath);
//    foreach (string wrkItem in wrkArquivos)
//    {
//        Debug.Print(wrkItem);
//        Funcoes.EscreverLog(wrkPath, "Arquivo encontrado => '" + Path.GetFileName(wrkItem) + "'");
//    }
//}                              

//Funcoes.EscreverLog(wrkPath, "Sistema Finalizado.", 0, false);

//Testa leitura do arquivo
//StreamReader wrkStream = new StreamReader(Path.Combine(wrkPath, "TwitterHomeControlLOG.txt"));
//string conteudo = wrkStream.ReadToEnd();
//wrkStream.Close();
//========================

//while (true)
//{
//    Debug.Print("Executa código.");
//    Thread.Sleep(1000);
//}

//Thread.Sleep(Timeout.Infinite);

#endregion
using System;
using Microsoft.SPOT.IO;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.IO;
using TwitterHomeControl.Business;
using TwitterHomeControl.Utilitarios;

using Networking;

namespace TwitterHomeControl_V4._1
{
    public class Program
    {
        #region "Threading"        

        public static void IniciaThreads()
        {
            var tarefa1 = new System.Threading.Thread(EscutaRequisicao);
            tarefa1.Start();
        }

        public static void EscutaRequisicao()
        {
            
            var socket = new GHIElectronics.NETMF.Net.Sockets.Socket(GHIElectronics.NETMF.Net.Sockets.AddressFamily.InterNetwork,
                GHIElectronics.NETMF.Net.Sockets.SocketType.Stream, GHIElectronics.NETMF.Net.Sockets.ProtocolType.Tcp);
            try
            {
                while (true)
                {                     
                    GHIElectronics.NETMF.Net.IPEndPoint localEndPoint = new GHIElectronics.NETMF.Net.IPEndPoint(GHIElectronics.NETMF.Net.IPAddress.Any, 8448);
                    socket.Bind(localEndPoint);
                    socket.Listen(1);

                    var listner = socket.Accept();

                    string texto = "";

                    if (listner.Poll(5 * 1000000,
                            GHIElectronics.NETMF.Net.Sockets.SelectMode.SelectRead))
                    {
                        //var conteudo = new byte[socket.Available];
                        var conteudo = new byte[1024];
                        listner.Receive(conteudo);

                        texto = Funcoes.BytesToString(conteudo);
                    }


                    string para = texto;
                }//end while
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                socket.Close();
            }
                        
            
        }

        #endregion

        static private string GetRootFolder()
        {
            var volumes = VolumeInfo.GetVolumes();
            return volumes.Length > 0 ? volumes[0].RootDirectory : string.Empty;
        }
        
        public static void Main()
        {
            //Acende Led principal
            var ledPrincipal = new FEZ_Components.LED(FEZ_Pin.Digital.LED);
            ledPrincipal.TurnOn();
            
            PersistentStorage wrkSd = null;
            try
            {
                //Inicia relés / saidas digitais
                var wrkSaidas = new Saidas();

                //Monta diretórios do Cartão SD
                wrkSd = new PersistentStorage("SD");
                wrkSd.MountFileSystem();
                var wrkPath = GetRootFolder();
                //----------------------------
                
                Funcoes.EscreverLog(wrkPath, "Sistema Inicializado.", 0, true);

                //Busca os parametros do sistema
                var objParametros = Parametros.BuscaParametros(wrkPath);

                var executa = 1;
                if (executa > 1)
                {
                    //string ip = "10.1.1.9";
                    //string subnet = "255.0.0.0";
                    //string gateway = "10.1.1.1";
                    //string mac = "00:19:5B:04:36:20";
                    //string dns = "10.1.1.1";                    

                    // Connect using Static IP address
                    EthernetW5100.Initialize(objParametros.ClasseIp.EnderecoIp, 
                        objParametros.ClasseIp.MascaraIp, 
                        objParametros.ClasseIp.GatewayIp, 
                        objParametros.ClasseIp.EnderecoMacGateway, 
                        objParametros.ClasseIp.Dns);

                    // Connect using new dhcp option
                    //EthernetW5100.Initialize(mac);

                    //Inicia monitoramento da rede interna
                    //IniciaThreads();
                    
                    //Iniciar monitoramento pelo Twitter
                    //MonitoraTwitter(objParametros, wrkPath, wrkSaidas, ledPrincipal)
                    //while (true)
                    //{
                    //    executa = 0;
                    //    if (executa > 0)
                    //        EscutaRequisicao();
                    //}


                    
                }

                //Desliga o led principal
                ledPrincipal.ShutOff();
            }
            catch (Exception ex)
            {
                ledPrincipal.StartBlinking(500, 500);
            }
            finally
            {
                //Libera controle SD
                if (wrkSd != null)
                    wrkSd.UnmountFileSystem();
            }
            
        }

        public static void MonitoraRede(Parametros objParametros, string wrkPath, Saidas wrkSaidas, FEZ_Components.LED ledPrincipal)
        {
            while (true)
            {
                //Defini o tempo de intervalo entre cada execução
                var timeout = DateTime.Now.AddSeconds(5); //5 segundos entre cada ping

                try
                {
                    //Efetua ping e verifica retorno
                    //if ()

                    //Desliga primeira tomada
                    //...

                    //Aguarda 10 segundos e religa a tomada
                    //...

                    //Aguarda 1 minuto para ativar conexão
                    //...

                }
                catch (Exception ex)
                {
                    ledPrincipal.StartBlinking(500, 500);

                    //Grava log:
                    Funcoes.EscreverLog(wrkPath, "Erro de exceção ocorrido: " + ex, 1, false);
                }

                //Espera o tempo de leitura entre cada mensagem
                while (DateTime.Now < timeout)
                {
                    System.Threading.Thread.Sleep(500); //0,5 segundo
                }
            }
        }

        public static void MonitoraTwitter(Parametros objParametros, string wrkPath, Saidas wrkSaidas, FEZ_Components.LED ledPrincipal)
        {
            while (true)
            {
                //Defini o tempo de intervalo entre cada execução
                var timeout = DateTime.Now.AddSeconds(objParametros.Intervalo);

                try
                {
                    //Busca lista de mensagens
                    var mensagens = Mensagem.BuscaMensagensDiretas(objParametros.UltimaMsgId,
                                                                   objParametros.Token, objParametros.TokenSecret);

                    //Instancia comando
                    var wrkComando = new Comando(wrkPath);
                    if (wrkComando.PercorreMensagens(mensagens, wrkSaidas, objParametros.Token,
                                                 objParametros.TokenSecret, objParametros.ClasseIp) == "close")
                    {
                        //Finalizar aplicação
                        objParametros.UltimaMsgId = Parametros.ObtemUltimaMsgId(wrkPath);
                        break;
                    }

                    objParametros.UltimaMsgId = Parametros.ObtemUltimaMsgId(wrkPath);
                }
                catch (Exception ex)
                {
                    ledPrincipal.StartBlinking(500, 500);

                    //Grava log:
                    Funcoes.EscreverLog(wrkPath, "Erro de exceção ocorrido: " + ex, 1, false);
                }

                //Espera o tempo de leitura entre cada mensagem
                while (DateTime.Now < timeout)
                {
                    System.Threading.Thread.Sleep(500); //0,5 segundo
                }
            }
        }
       
    }
}

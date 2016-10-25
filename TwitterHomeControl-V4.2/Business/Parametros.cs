using System.IO;
using System.Xml;
using TwitterHomeControl.Utilitarios;

namespace TwitterHomeControl.Business
{
    public class Ip
    {
        public string EnderecoIp { get; set; }
        public string MascaraIp { get; set; }
        public string GatewayIp { get; set; }
        public string EnderecoMacGateway { get; set; }
        public string Dns { get; set; }
    }

    public class Parametros
    {
        public const string ArquivoXml = "Sistema\\Parametros.xml";
        public const string ArquivoUltimaMsgId = "Sistema\\UltimaMsgId.txt";
        
        public enum TipoLeituraMsg
        {
            Publicas,
            Diretas,
        }

        public TipoLeituraMsg Mensagens { get; set; }
        public int Intervalo { get; set; }        
        public string Senha {get;set;}
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string UltimaMsgId { get; set; }

        public Ip ClasseIp { get; set; }
        

        //public DateTime DataUltimaMensagemLida
        //{
        //    get
        //    {
        //        return wrkDataUltimaMensagemLida;
        //    }
        //    //set
        //    //{
        //    //    // Grava no arquivo XML o valor também
        //    //    XmlDocument wrkXmlDocument = new XmlDocument();
        //    //    wrkXmlDocument.Load((My.Settings.DiretorioSistema + gArquivoXML));
        //    //    wrkXmlDocument.SelectSingleNode("//parametros//DataUltimaMensagemLida").InnerText = Format(value, "dd/MM/yyyy HH:mm:ss");
        //    //    wrkXmlDocument.Save((My.Settings.DiretorioSistema + gArquivoXML));
        //    //    wrkXmlDocument = null;
        //    //    wrkDataUltimaMensagemLida = value;
        //    //}
        //}

        public static Parametros BuscaParametros(string pPathSd)
        {
            //Carrega FileStream
            var filestream = File.OpenRead(Path.Combine(pPathSd, ArquivoXml));
            var filelength = filestream.Length;
            var xmlBytes = new byte[filelength];
            filestream.Read(xmlBytes, 0, (int)filelength);
            filestream.Close(); 
            //==================         
            
            var xmlStream = new MemoryStream(xmlBytes);
            Parametros wrkRetorno;
            using (var xmlreader = XmlReader.Create(xmlStream))
            {
                //Instancia de um novo objeto
                wrkRetorno = new Parametros();                

                //Obtem valores
                xmlreader.Read();

                //Atenção: A ordem de leitura dos parametros necessita estar na ordem correta

                xmlreader.ReadToFollowing("Usuario");
                wrkRetorno.Usuario = xmlreader.ReadString();

                xmlreader.ReadToFollowing("Token");
                wrkRetorno.Token = xmlreader.ReadString();

                xmlreader.ReadToFollowing("TokenSecret");
                wrkRetorno.TokenSecret = xmlreader.ReadString();

                wrkRetorno.UltimaMsgId = ObtemUltimaMsgId(pPathSd);

                xmlreader.ReadToFollowing("Senha");
                wrkRetorno.Senha = xmlreader.ReadString(); //Funcoes.Decripta(xmlreader.ReadString());
                
                xmlreader.ReadToFollowing("Intervalo");
                wrkRetorno.Intervalo = int.Parse(xmlreader.ReadString());

                xmlreader.ReadToFollowing("Mensagens");
                wrkRetorno.Mensagens = ((TipoLeituraMsg)(int.Parse(xmlreader.ReadString())));

                wrkRetorno.ClasseIp = new Ip();
                
                xmlreader.ReadToFollowing("EnderecoIp");
                wrkRetorno.ClasseIp.EnderecoIp = xmlreader.ReadString(); //Funcoes.BytesStringToBytes(xmlreader.ReadString());

                xmlreader.ReadToFollowing("MascaraIp");
                wrkRetorno.ClasseIp.MascaraIp = xmlreader.ReadString(); //Funcoes.BytesStringToBytes(xmlreader.ReadString());

                xmlreader.ReadToFollowing("GatewayIp");
                wrkRetorno.ClasseIp.GatewayIp = xmlreader.ReadString(); //Funcoes.BytesStringToBytes(xmlreader.ReadString());

                xmlreader.ReadToFollowing("Dns");
                wrkRetorno.ClasseIp.Dns = xmlreader.ReadString();

                xmlreader.ReadToFollowing("EnderecoMacGateway");
                wrkRetorno.ClasseIp.EnderecoMacGateway = xmlreader.ReadString(); //Funcoes.BytesStringToBytes(xmlreader.ReadString());
                //==============

            }

            return wrkRetorno;
        }

        public static string ObtemUltimaMsgId(string pPathSd)
        {
            var arrayByte = File.ReadAllBytes(Path.Combine(pPathSd, ArquivoUltimaMsgId));
            return Funcoes.BytesToString(arrayByte);
            
            //Trata retorno
            //return retorno.Substring(0, retorno.Length - 2);
        }

        public static void GravaUltimaMsgId(string pPathSd, string pUltimaMsgId)
        {
            var wrkNomeArquivo = Path.Combine(pPathSd, ArquivoUltimaMsgId);
            File.WriteAllBytes(wrkNomeArquivo, System.Text.Encoding.UTF8.GetBytes(pUltimaMsgId));
            
            //var wrkStream = new StreamWriter(wrkNomeArquivo, true);
            //try
            //{
            //    //Realiza a gravação do ultimo Id
            //    wrkStream.WriteLine(pUltimaMsgId);
            //}
            //finally
            //{
            //    wrkStream.Close();
            //}
        }
        
    }
}

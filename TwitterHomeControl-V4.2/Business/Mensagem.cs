#region "codigo comentario BKP"
//using GHIElectronics.NETMF.FEZ;
//using Microsoft.SPOT.Hardware;
//using TwitterHomeControl.Utilitarios;


//var twitterClient = new TwitterClient(pLogin, pSenha);

//Request documento do Twitter
//var mensagemRetorno = TwitterClient.WebRequest("http://api.twitter.com/1/direct_messages.xml?since_id=" + pUltimaMsgId + "&count=1&",
//                                               String.Empty,
//                                               "Authorization: Basic " + twitterClient.B64AuthenticateString +
//                                               "\r\n",
//                                               "GET",
//                                               "", String.Empty, pClasseIp);

//var mensagemRetorno = TwitterClient.WebRequest(Bibliotecas.OAuthTwitter.Teste(),
//                                               String.Empty,
//                                               "",
//                                               "GET",
//                                               "application/x-www-form-urlencoded", String.Empty, pClasseIp);

////Array de objetos de mensagem
//var resultArray = new ArrayList();

////Cria um stream a partir do conteudo
//var stream = new MemoryStream(mensagemRetorno);

////Cria XML reader
//var reader = XmlReader.Create(stream);

//// Process each status entry
//while (reader.Read())
//{
//    if (reader.NodeType != XmlNodeType.Element)
//        continue;

//    //Debug.Print(reader.Name);
//    if (reader.Name != "direct_message")
//        continue;

//    //Objeto que deve ser adicionado no array
//    Mensagem mensagem = null;

//    if (reader.ReadToFollowing("id"))
//        mensagem = new Mensagem { Id = reader.ReadString() };

//    if (reader.ReadToFollowing("text") && mensagem != null)
//        mensagem.Texto = reader.ReadString();

//    if (reader.ReadToFollowing("sender_screen_name") && mensagem != null)
//    {
//        mensagem.EnviadoPor = reader.ReadString();
//        resultArray.Add(mensagem);
//    }
//}//While
#endregion

using System;
using System.Collections;
using TwitterHomeControl.Utilitarios;

namespace TwitterHomeControl.Business
{
    public class Mensagem
    {
        public string Id { get; set; }
        public string Texto { get; set; }
        public string EnviadoPor { get; set; }

        public static void EnviaMensagem(string pToken, string pTokenScret,
                                         string pPara, string pMensagem)
        {

            var url = "http://gadottisolucoes.com/Gerenciador.asmx/EnviaMensagemDireta?pParaUsuario=" +
                      pPara + "&pMensagem=" + pMensagem + "&pToken=" + pToken + "&pTokenSecret=" + pTokenScret;

            TwitterClient.WebPost(url,
                                  String.Empty,
                                  String.Empty,
                                  "GET",
                                  "application/x-www-form-urlencoded", String.Empty);

            
        }

        public static ArrayList BuscaMensagensDiretas(string pUltimaMsgId,
                                                      string pToken, string pTokenScret)
        {
            
            var url = "http://gadottisolucoes.com/gerenciador.asmx/BuscaMensagensDiretas?pSinceId=" +
                      pUltimaMsgId + "&pCount=20&pToken=" + pToken + "&pTokenSecret=" + pTokenScret;

            var mensagemRetorno = TwitterClient.WebRequest(url,
                                                           String.Empty,
                                                           "",
                                                           "GET",
                                                           "application/x-www-form-urlencoded", String.Empty);

            
            if (mensagemRetorno != "")
            {
                var mensagemString = mensagemRetorno.Substring(76);
                mensagemString = mensagemString.Substring(0, mensagemString.IndexOf("</string>"));

                if (mensagemString.Equals("null"))
                    return null;

                //Verifica ser ocorreu erro dentro do webservice
                if (mensagemString.IndexOf("Erro").Equals(0))
                    return null;

                //Array de objetos de mensagem
                var resultArray = new ArrayList();

                foreach (var mensagem in mensagemString.Split('-'))
                {
                    if (mensagem == string.Empty) continue;

                    var parametros = mensagem.Split(';');
                    var retorno = new Mensagem();
                    retorno.Id = parametros[0];
                    retorno.Texto = parametros[1];
                    retorno.EnviadoPor = parametros[2];

                    resultArray.Add(retorno);
                }

                //Retorna array de objetos com resultos
                return resultArray;
            }

            return null;
        }
    }
}

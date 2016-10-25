using System;
using System.Collections;
using System.IO;
using System.Xml;
using TwitterHomeControl.Utilitarios;

namespace TwitterHomeControl.Business
{
    public class Comando
    {

        public enum TipoComandoEnum
        {
            Simples,
            Encadeada,
        }

        public const string ArquivoXml = "Sistema\\Comandos.xml";
        private string PathSd { get; set; }
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Nome { get; set; }
        public int EquipamentosId { get; set; }
        public TipoComandoEnum TipoComando { get; set; }
        public bool EnviaRetorno { get; set; }
        public string RestricaoUsuarios { get; set; }
        public string DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public string[] ListaComandos { get; set; }
        public bool EstadoEquipamento { get; set; }

        public Comando(string pPath)
        {
            PathSd = pPath;
        }

        private Comando Busca(int pComandoId)
        {
            //Carrega FileStream
            var filestream = File.OpenRead(Path.Combine(PathSd, ArquivoXml));
            var filelength = filestream.Length;
            var xmlBytes = new byte[filelength];
            filestream.Read(xmlBytes, 0, (int)filelength);
            filestream.Close();
            //==================         

            var xmlStream = new MemoryStream(xmlBytes);
            Comando wrkRetorno;
            using (var xmlreader = XmlReader.Create(xmlStream))
            {
                //Instancia de um novo objeto
                wrkRetorno = new Comando(PathSd);

                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType != XmlNodeType.Element)
                        continue;

                    if (xmlreader.Name != "comando")
                        continue;

                    xmlreader.ReadToFollowing("id");

                    //Verifica se é o Id procurado
                    var comandoId = Convert.ToInt32(xmlreader.ReadString());
                    if (comandoId == pComandoId)
                    {
                        wrkRetorno.Id = comandoId;

                        xmlreader.ReadToFollowing("descricao");
                        wrkRetorno.Descricao = xmlreader.ReadString();

                        xmlreader.ReadToFollowing("nome");
                        wrkRetorno.Nome = xmlreader.ReadString();

                        xmlreader.ReadToFollowing("equipamentosid");
                        wrkRetorno.EquipamentosId = Convert.ToInt32(xmlreader.ReadString());

                        xmlreader.ReadToFollowing("tipocomando");
                        wrkRetorno.TipoComando = (TipoComandoEnum) Convert.ToInt32(xmlreader.ReadString());

                        xmlreader.ReadToFollowing("envia_retorno");
                        wrkRetorno.EnviaRetorno = xmlreader.ReadString() == "0" ? false : true;

                        xmlreader.ReadToFollowing("restricao_usuarios");
                        wrkRetorno.RestricaoUsuarios = xmlreader.ReadString();

                        xmlreader.ReadToFollowing("data_cadastro");
                        wrkRetorno.DataCadastro = xmlreader.ReadString();

                        xmlreader.ReadToFollowing("ativo");
                        wrkRetorno.Ativo = xmlreader.ReadString() == "0" ? false : true;

                        xmlreader.ReadToFollowing("listacomandos");
                        wrkRetorno.ListaComandos = xmlreader.ReadString() == string.Empty
                                                       ? null
                                                       : xmlreader.ReadString().Split(';');

                        xmlreader.ReadToFollowing("estadoequipamento");
                        wrkRetorno.EstadoEquipamento = xmlreader.ReadString() == "0" ? false : true;

                        //Adiciona objeto à lista
                        return wrkRetorno;
                    }
                }
            }

            return null;
        }
    
        private ArrayList Busca()
        {
            //Carrega FileStream
            var filestream = File.OpenRead(Path.Combine(PathSd, ArquivoXml));
            var filelength = filestream.Length;
            var xmlBytes = new byte[filelength];
            filestream.Read(xmlBytes, 0, (int)filelength);
            filestream.Close();
            //==================         

            var xmlStream = new MemoryStream(xmlBytes);
            ArrayList wrkRetorno;
            using (var xmlreader = XmlReader.Create(xmlStream))
            {
                //Instancia de um novo objeto
                wrkRetorno = new ArrayList();

                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType != XmlNodeType.Element)
                        continue;

                    if (xmlreader.Name != "comando")
                        continue;

                    //Objeto que deve ser adicionado no array
                    var wrkComando = new Comando(PathSd);

                    xmlreader.ReadToFollowing("id");
                    wrkComando.Id = Convert.ToInt32(xmlreader.ReadString());

                    xmlreader.ReadToFollowing("descricao");
                    wrkComando.Descricao = xmlreader.ReadString();

                    xmlreader.ReadToFollowing("nome");
                    wrkComando.Nome = xmlreader.ReadString();

                    xmlreader.ReadToFollowing("equipamentosid");
                    wrkComando.EquipamentosId = Convert.ToInt32(xmlreader.ReadString());

                    xmlreader.ReadToFollowing("tipocomando");
                    wrkComando.TipoComando = (TipoComandoEnum) Convert.ToInt32(xmlreader.ReadString());

                    xmlreader.ReadToFollowing("envia_retorno");
                    wrkComando.EnviaRetorno = xmlreader.ReadString() == "0" ? false : true;

                    xmlreader.ReadToFollowing("restricao_usuarios");
                    wrkComando.RestricaoUsuarios = xmlreader.ReadString();

                    xmlreader.ReadToFollowing("data_cadastro");
                    wrkComando.DataCadastro = xmlreader.ReadString();

                    xmlreader.ReadToFollowing("ativo");
                    wrkComando.Ativo = xmlreader.ReadString() == "0" ? false : true;

                    xmlreader.ReadToFollowing("listacomandos");
                    var lista = xmlreader.ReadString();
                    wrkComando.ListaComandos = lista == string.Empty
                                                   ? null
                                                   : lista.Split(';');

                    xmlreader.ReadToFollowing("estadoequipamento");
                    wrkComando.EstadoEquipamento = xmlreader.ReadString() == "0" ? false : true;

                    //Adiciona objeto à lista
                    wrkRetorno.Add(wrkComando);
                }
            }

            return wrkRetorno;
        }

        public string PercorreMensagens(ArrayList pMensagens, Saidas pSaidas,
                                      string pToken, string pTokenSecret, Ip pClasseIp)
        {
            try
            {
                //Sai da rotina caso não houver mensagens para tratar
                if (pMensagens == null || pMensagens.Count.Equals(0))
                    return "OK";
                //====================================================

                //Busca comandos cadastrados
                var listaComandos = Busca();
                //==========================

                //Verifica se obteve com sucesso os comandos
                if (listaComandos == null || listaComandos.Count.Equals(0))
                    return "Sem comandos cadastrados";
                //==========================================

                //Passa por todas as mensagens coletadas
                foreach (var mensagem in pMensagens)
                {
                    //Obtem objeto da mensagem
                    var itemMensagem = (Mensagem)mensagem;

                    //Salva id da mensagem executada
                    Parametros.GravaUltimaMsgId(PathSd, itemMensagem.Id);

                    //Verifica se nao deve finalizar a aplicação
                    if (itemMensagem.Texto.IndexOf("finalizar app") >= 0)
                    {
                        Parametros.GravaUltimaMsgId(PathSd, itemMensagem.Id);
                        return "close";
                    }

                    //Passa por cada comando
                    foreach (var listaComando in listaComandos)
                    {
                        var itemcomando = (Comando)listaComando;

                        //Verifica se comando está ativo
                        if (!itemcomando.Ativo)
                            continue;
                        
                        //Verifica se encontra o nome do comando detro da mensagem
                        if (itemMensagem.Texto.IndexOf(itemcomando.Nome) >= 0)
                        {
                            //Verifica se há restrição para usuários
                            if (itemcomando.RestricaoUsuarios != string.Empty &&
                                !(itemcomando.RestricaoUsuarios.IndexOf(itemMensagem.EnviadoPor) >= 0))
                                continue;

                            //Executa o comando
                            var retorno = itemcomando.TipoComando == TipoComandoEnum.Simples ? 
                                Executa(itemcomando, pSaidas) : 
                                ExecutaEncadeado(itemcomando, pSaidas);

                            //Verifica retorno da execução
                            if (retorno)
                            {
                                //Salva id da mensagem executada
                                //Parametros.GravaUltimaMsgId(PathSd, itemMensagem.Id);

                                //Verifica necessidade de envio de retorno
                                if (itemcomando.EnviaRetorno)
                                    Mensagem.EnviaMensagem(pToken, pTokenSecret, itemMensagem.EnviadoPor, "Comando_" + itemcomando.Descricao + "_executado_com_sucesso");
                            }
                            else
                            {
                                //Verifica necessidade de envio de retorno
                                if (itemcomando.EnviaRetorno)
                                    Mensagem.EnviaMensagem(pToken, pTokenSecret, itemMensagem.EnviadoPor, "Comando_" + itemcomando.Descricao + "_nao_executado_Erro_de_execucao");
                                
                            }

                            //Proxima mensagem
                            break;

                        } //if comando

                    }//foreach comando
                }//foreach mensagem

                //Retorno com sucesso
                return "OK";
            }
            catch (Exception ex)
            {
                //Grava log
                Funcoes.EscreverLog(PathSd, "Erro de exceção ocorrido ao percorrer mensagens: " + ex, 1, false);
                return ex.Message;
            }
            
        }

        public bool ExecutaEncadeado(Comando pComando, Saidas pSaidas)
        {
            foreach (var comando in pComando.ListaComandos)
            {
                Executa(Busca(int.Parse(comando)), pSaidas);
            }
            return true;
        }

        public bool Executa(Comando pComando, Saidas pSaidas)
        {
            try
            {
                switch (pComando.EquipamentosId)
                {
                    case 1:
                        {
                            pSaidas.Tomada1.Write(pComando.EstadoEquipamento);
                            break;
                        }
                    case 2:
                        {
                            pSaidas.Tomada2.Write(pComando.EstadoEquipamento);
                            break;
                        }
                    case 3:
                        {
                            pSaidas.Tomada3.Write(pComando.EstadoEquipamento);
                            break;
                        }
                    case 4:
                        {
                            pSaidas.Tomada4.Write(pComando.EstadoEquipamento);
                            break;
                        }
                    case 5:
                        {
                            pSaidas.Tomada5.Write(pComando.EstadoEquipamento);
                            break;
                        }
                }

                return true;
            }
            catch (Exception ex)
            {
                //Grava log
                Funcoes.EscreverLog(PathSd, "Erro de exceção ocorrido ao executar comando '" + pComando.Nome + "': " + ex, 1, false);
                return false;
            }
        }
    }
}

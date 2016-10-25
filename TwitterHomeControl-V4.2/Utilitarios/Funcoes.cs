using System;
using System.IO;
using Microsoft.SPOT.Cryptography;

namespace TwitterHomeControl.Utilitarios
{
    public class Funcoes
    {
        public static string BytesToString(byte[] bytes)
        {
            var s = string.Empty;
            for (var i = 0; i < bytes.Length; ++i)
                s += (char)bytes[i];
            return s;
        }

        public static string BytesToBytesString(byte[] bytes)
        {
            var retorno = string.Empty;
            foreach (var b in bytes)
            {
                string.Concat(retorno, b + ".");
            }

            return retorno.Substring(0, retorno.Length - 1);
        }

        public static byte[] BytesStringToBytes(string stringBytes)
        {
            var caracteres = stringBytes.Split('.');
            var retorno = new byte[caracteres.Length];
            var indice = 0;
            foreach (var caractere in caracteres)
            {
                retorno[indice] = byte.Parse(caractere);
                indice++;
            }

            return retorno;
        }

        public static DateTime FromTwitterDate(string pData)
        {
            //string dayOfWeek = pData.Substring(0, 3);
            var month = pData.Substring(4, 3);
            var dayInMonth = pData.Substring(8, 2);
            var time = pData.Substring(11, 9);
            //string offset = pData.Substring(20, 5);
            var year = pData.Substring(25, 5);

            // Split time
            var splitTime = time.Split(':');
            var hour = int.Parse(splitTime[0]);
            var minutes = int.Parse(splitTime[1]);
            var seconds = int.Parse(splitTime[2]);
            var ret = new DateTime(int.Parse(year), int.Parse(month), int.Parse(dayInMonth), hour, minutes, seconds);
            return ret;
        }

        public static DateTime DateTimeParse(string pDataHora)
        {
            var wrkDataSplit = pDataHora.Substring(0, 11).Split('/');
            var wrkDia = int.Parse(wrkDataSplit[0]);
            var wrkMes = int.Parse(wrkDataSplit[1]);
            var wrkAno = int.Parse(wrkDataSplit[2]);

            var wrkHora = 0;
            var wrkMinuto = 0;
            var wrkSegundo = 0;
            if (pDataHora.IndexOf(':') > -1)
            {
                var wrkHoraSplit = pDataHora.Substring(11, 8).Split(':');
                wrkHora = int.Parse(wrkHoraSplit[0]);
                wrkMinuto = int.Parse(wrkHoraSplit[1]);
                wrkSegundo = int.Parse(wrkHoraSplit[2]);
            }

            var wrkDataHora = new DateTime(wrkAno, wrkMes, wrkDia, wrkHora, wrkMinuto, wrkSegundo);
            return wrkDataHora;
        }

        public static int ToInt(String valor)
        {
            var r = 0;
            var negative = false;
            foreach (Char c in valor)
            {
                if (c == '-')
                {
                    negative = true;
                }
                else if ("0123456789".IndexOf(c) != -1)
                {
                    r *= 10;
                    r += (c - '0');
                }
            }

            if (negative)
                return -r;

            //else
            return r;
        }

        //public static string Decripta(string mensagem)
        //{
        //    // 16-byte 128-bit key (chave)
        //    var xteaKey = new byte[] { 15, 10, 1, 7, 5, 14, 6, 3, 11, 2, 4, 8, 9, 12, 16, 13 };
        //    var xtea = new Key_TinyEncryptionAlgorithm(xteaKey);

        //    var encryptedData = BytesStringToBytes(mensagem);
        //    var decryptedBytes = xtea.Decrypt(encryptedData, 0, encryptedData.Length, null);
        //    return BytesToString(decryptedBytes);
            
        //}

        //public static string SomenteNumeros(string pValor)
        //{
        //    int wrkInd = 0;
        //    int wrkTemp = 0;
        //    string wrkTemp2 = String.Empty;
        //    for (wrkInd = 1; (wrkInd <= pValor.Length); wrkInd++)
        //    {
        //        wrkTemp = Asc(pValor.Substring((wrkInd - 1), 1));
        //        if (((wrkTemp >= 48)
        //                    && (wrkTemp <= 57)))
        //        {
        //            wrkTemp2 = (wrkTemp2 + pValor.Substring((wrkInd - 1), 1));
        //        }
        //    }
        //    return wrkTemp2;
        //}

        //public static string Encripta(string wrkString)
        //{
        //    Seguranca wrkCripto = new Seguranca();
        //    string wrkChave;
        //    string wrkStringEncriptada;
        //    // Encripta a String
        //    wrkStringEncriptada = wrkCripto.Encripta(wrkString);
        //    // Pega a Chave Gerada
        //    wrkChave = wrkCripto.Key;
        //    // Retorna a Senha Criptografada
        //    return (wrkStringEncriptada + wrkChave);
        //}

        //public static string Decripta(string wrkString)
        //{
        //    Utilitarios.Seguranca wrkCripto = new Utilitarios.Seguranca();
        //    // Caso a senha tenha 56 caracteres, significa que pode ter sido criptografada
        //    if ((wrkString.Length >= 56))
        //    {
        //        // Pega a Chave da String criptografada
        //        wrkCripto.Key = wrkString.Substring((wrkString.Length - 32));
        //        // Retorna a String Original
        //        return wrkCripto.Decripta(wrkString.Substring(0, (wrkString.Length - 32)));
        //    }

        //    return wrkString;
        //}

        public static string FormataUsuario(string pUsuario)
        {
            var wrkUsuario = pUsuario;
            if ((wrkUsuario != String.Empty))
            {
                if ((wrkUsuario.Substring(0, 1) != "@"))
                {
                    wrkUsuario = ("@" + wrkUsuario);
                }
            }
            return wrkUsuario;
        }

        public static void EscreverLog(string pNmArqvLog, string pLinha)
        {
            EscreverLog(pNmArqvLog, pLinha, 1, false);
        }

        public static void EscreverLog(string pNmArqvLog, string pLinha, int pNrNivel, bool pPularLinha)
        {
            const string wrkProgramaName = "TwitterHomeControl";
            const string wrkProgramaVersion = "v1.0.0";
            pNmArqvLog = Path.Combine(pNmArqvLog, "TwitterHomeControlLOG.txt");

            if (File.Exists(pNmArqvLog))
            {
                // verifica tamanho do arquivo                
                FileStream fileStream = null;
                long fileLength;
                try
                {
                    fileStream = new FileStream(pNmArqvLog, FileMode.Open, FileAccess.Read);
                    fileLength = fileStream.Length;
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                }
                //=============================

                //Maior que 1Mb, excluir
                if ((fileLength > (1000 * 1024)))
                {
                    File.Delete(pNmArqvLog);
                }
                //=======================
            }

            var wrkStream = new StreamWriter(pNmArqvLog, true);
            try
            {
                wrkStream.WriteLine((pPularLinha ? wrkStream.NewLine : "")
                                    + FormataData(DateTime.Now) + " " + FormataHora(DateTime.Now) + " - "
                                    + wrkProgramaName
                                    + wrkProgramaVersion + " - "
                                    + Espacos(pNrNivel) + pLinha);
            }
            finally
            {
                wrkStream.Close();
            }
        }

        private static string Espacos(int nivel)
        {
            if (nivel == 0)
                return string.Empty;

            string espacos = "";

            for (int wrkInd = 0; wrkInd <= nivel; wrkInd++)
            {
                espacos = espacos + "    ";
            }

            return espacos;
        }

        public static string FormataData(DateTime data)
        {
            return (data.Day < 10 ? "0" : "") + data.Day + "/"
                + (data.Month < 10 ? "0" : "") + data.Month + "/"
                + data.Year;
        }

        public static string FormataHora(DateTime data)
        {
            return (data.Hour < 10 ? "0" : "") + data.Hour + ":"
                + (data.Minute < 10 ? "0" : "") + data.Minute + ":"
                + (data.Second < 10 ? "0" : "") + data.Second;
        }

        public static byte[] EncodingAscii(string caracteres)
        {
            var retorno = new byte[caracteres.Length];
            var indice = 0;
            foreach (var b in caracteres.ToCharArray())
            {
                retorno[indice] = GetAscii(b.ToString());
                indice++;
            }

            return retorno;
        }

        public static byte GetAscii(string caracter)
        {
            switch (caracter)
            {
                case " ":
                    return 32;
                case "!":
                    return 33;
                case "#":
                    return 35;
                case "$":
                    return 36;
                case "%":
                    return 37;
                case "&":
                    return 38;
                case "'":
                    return 39;
                case "(":
                    return 40;
                case ")":
                    return 41;
                case "*":
                    return 42;
                case "+":
                    return 43;
                case ",":
                    return 44;
                case "-":
                    return 45;
                case ".":
                    return 46;
                case "/":
                    return 47;
                case "0":
                    return 48;
                case "1":
                    return 49;
                case "2":
                    return 50;
                case "3":
                    return 51;
                case "4":
                    return 52;
                case "5":
                    return 53;
                case "6":
                    return 54;
                case "7":
                    return 55;
                case "8":
                    return 56;
                case "9":
                    return 57;
                case ":":
                    return 58;
                case ";":
                    return 59;
                case "<":
                    return 60;
                case "=":
                    return 61;
                case ">":
                    return 62;
                case "?":
                    return 63;
                case "@":
                    return 64;
                case "A":
                    return 65;
                case "B":
                    return 66;
                case "C":
                    return 67;
                case "D":
                    return 68;
                case "E":
                    return 69;
                case "F":
                    return 70;
                case "G":
                    return 71;
                case "H":
                    return 72;
                case "I":
                    return 73;
                case "J":
                    return 74;
                case "K":
                    return 75;
                case "L":
                    return 76;
                case "M":
                    return 77;
                case "N":
                    return 78;
                case "O":
                    return 79;
                case "P":
                    return 80;
                case "Q":
                    return 81;
                case "R":
                    return 82;
                case "S":
                    return 83;
                case "T":
                    return 84;
                case "U":
                    return 85;
                case "V":
                    return 86;
                case "W":
                    return 87;
                case "X":
                    return 88;
                case "Y":
                    return 89;
                case "Z":
                    return 90;
                case "[":
                    return 91;
                case "\\":
                    return 92;
                case "]":
                    return 93;
                case "^":
                    return 94;
                case "_":
                    return 95;
                case "`":
                    return 96;
                case "a":
                    return 97;
                case "b":
                    return 98;
                case "c":
                    return 99;
                case "d":
                    return 100;
                case "e":
                    return 101;
                case "f":
                    return 102;
                case "g":
                    return 103;
                case "h":
                    return 104;
                case "i":
                    return 105;
                case "j":
                    return 106;
                case "k":
                    return 107;
                case "l":
                    return 108;
                case "m":
                    return 109;
                case "n":
                    return 110;
                case "o":
                    return 111;
                case "p":
                    return 112;
                case "q":
                    return 113;
                case "r":
                    return 114;
                case "s":
                    return 115;
                case "t":
                    return 116;
                case "u":
                    return 117;
                case "v":
                    return 118;
                case "w":
                    return 119;
                case "x":
                    return 120;
                case "y":
                    return 121;
                case "z":
                    return 122;
                case "{":
                    return 123;
                case "|":
                    return 124;
                case "}":
                    return 125;
                case "~":
                    return 126;
                default:
                    return 0;

            }
        }
    }
}

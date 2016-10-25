using System;
using Microsoft.SPOT;

namespace TwitterHomeControl.Utilitarios
{
    public class Seguranca
    {
        public enum enumCriptoProvider
        {
            Rijndael = 1,
            RC2 = 2,
            DES = 3,
            TripleDES = 4,
        }
        //string wrkKey = String.Empty;
        //enumCriptoProvider wrkCriptoProvider;
        //SymmetricAlgorithm wrkAlgoritmo;


        //public Seguranca()
        //{
        //    // Como não foi passado nenhum Algoritmo
        //    wrkAlgoritmo = new RijndaelManaged();
        //    wrkAlgoritmo.Mode = CipherMode.CBC;
        //    wrkCriptoProvider = enumCriptoProvider.Rijndael;
        //}

        //// Inicializa a Classe passando o Tipo de Provedor da Criptografia
        //public Seguranca(enumCriptoProvider pCriptoProvider)
        //{
        //    switch (pCriptoProvider)
        //    {
        //        case enumCriptoProvider.Rijndael:
        //            wrkAlgoritmo = new RijndaelManaged();
        //            wrkCriptoProvider = enumCriptoProvider.Rijndael;
        //            break;
        //        case enumCriptoProvider.RC2:
        //            wrkAlgoritmo = new RC2CryptoServiceProvider();
        //            wrkCriptoProvider = enumCriptoProvider.RC2;
        //            break;
        //        case enumCriptoProvider.DES:
        //            wrkAlgoritmo = new DESCryptoServiceProvider();
        //            wrkCriptoProvider = enumCriptoProvider.DES;
        //            break;
        //        case enumCriptoProvider.TripleDES:
        //            wrkAlgoritmo = new TripleDESCryptoServiceProvider();
        //            wrkCriptoProvider = enumCriptoProvider.TripleDES;
        //            break;
        //    }
        //    wrkAlgoritmo.Mode = CipherMode.CBC;
        //}

        //// Chave de Criptografia
        //public string Key
        //{
        //    get
        //    {
        //        return wrkKey;
        //    }
        //    set
        //    {
        //        wrkKey = value;
        //    }
        //}

        //// Cria Vetor de Inicialização
        //private void SetIV()
        //{
        //    switch (wrkCriptoProvider)
        //    {
        //        case enumCriptoProvider.Rijndael:
        //            wrkAlgoritmo.IV = new byte[] {
        //                    204,
        //                    115,
        //                    75,
        //                    168,
        //                    234,
        //                    156,
        //                    70,
        //                    5,
        //                    249,
        //                    205,
        //                    194,
        //                    53,
        //                    46,
        //                    19,
        //                    111,
        //                    15};
        //            break;
        //        default:
        //            wrkAlgoritmo.IV = new byte[] {
        //                    249,
        //                    205,
        //                    194,
        //                    53,
        //                    46,
        //                    19,
        //                    111,
        //                    15};
        //            break;
        //    }
        //}

        //// Gera uma Chave Aleatória para ser usada na Criptografia
        //private string GerarKey()
        //{
        //    int wrkIndice;
        //    string wrkRetorno = String.Empty;
        //    int wrkNumero;
        //    System.Random wrkRandom = new System.Random();
        //    for (wrkIndice = 0; (wrkIndice <= 32); wrkIndice++)
        //    {
        //        wrkNumero = wrkRandom.Next(0, 99);
        //        wrkRetorno = (wrkRetorno + wrkNumero.ToString());
        //        if ((wrkRetorno.Length >= 32))
        //        {
        //            if ((wrkRetorno.Length > 32))
        //            {
        //                wrkRetorno = wrkRetorno.Substring(0, 32);
        //            }
        //            break;
        //        }
        //    }
        //    return wrkRetorno;
        //}
        
        //// Pega a Chave gerada
        //public byte[] GetKey() {
        //    string wrkSalt = String.Empty;
        //    if ((wrkAlgoritmo.LegalKeySizes.Length > 0)) {
        //        int wrkKeySize = (wrkKey.Length * 8);
        //        int wrkMinSize = wrkAlgoritmo.LegalKeySizes(0).MinSize;
        //        int wrkMaxSize = wrkAlgoritmo.LegalKeySizes(0).MaxSize;
        //        int wrkSkipSize = wrkAlgoritmo.LegalKeySizes(0).SkipSize;
        //        if ((wrkKeySize > wrkMaxSize)) {
        //            wrkKey = wrkKey.Substring(0, Convert.ToInt32((wrkMaxSize / 8)));
        //        }
        //        else if ((wrkKeySize < wrkMaxSize)) {
        //            int wrkSizeValido;
        //            if ((wrkKeySize <= wrkMinSize)) {
        //                wrkSizeValido = wrkMinSize;
        //            }
        //            else {
        //                wrkSizeValido = ((wrkKeySize 
        //                            - (wrkKeySize % wrkSkipSize)) 
        //                            + wrkSkipSize);
        //                if ((wrkKeySize < wrkSizeValido)) {
        //                    wrkKey = wrkKey.PadRight(Convert.ToInt32((wrkSizeValido / 8)), Convert.ToChar("*"));
        //                }
        //            }
        //        }
        //    }
        //    if ((wrkKey.Length == 0)) {
        //        wrkKey = GerarKey();
        //    }
        //    PasswordDeriveBytes wrkKeyRetorno = new PasswordDeriveBytes(wrkKey, ASCIIEncoding.ASCII.GetBytes(wrkSalt));
        //    return wrkKeyRetorno.GetBytes(wrkKey.Length);
        //}
        
        //// Encripta a String passada por Parâmetro
        //public string Encripta(string pString) {
        //    byte[] wrkByte = ASCIIEncoding.ASCII.GetBytes(pString);
        //    byte[] wrkKeyByte = GetKey();
        //    wrkAlgoritmo.Key = wrkKeyByte;
        //    SetIV();
        //    ICryptoTransform wrkCriptoTransform = wrkAlgoritmo.CreateEncryptor();
        //    MemoryStream wrkMemoryStream = new MemoryStream();
        //    CryptoStream wrkCriptoStream = new CryptoStream(wrkMemoryStream, wrkCriptoTransform, CryptoStreamMode.Write);
        //    wrkCriptoStream.Write(wrkByte, 0, wrkByte.Length);
        //    wrkCriptoStream.FlushFinalBlock();
        //    byte[] wrkCriptoByte = wrkMemoryStream.ToArray();
        //    return Convert.ToBase64String(wrkCriptoByte, 0, wrkCriptoByte.GetLength(0));
        //}
        
        //// Decripta a String passada por Parâmetro
        //public string Decripta(string pString) {
        //    byte[] wrkByte = Convert.FromBase64String(pString);
        //    byte[] wrkKeyByte = GetKey();
        //    wrkAlgoritmo.Key = wrkKeyByte;
        //    SetIV();
        //    ICryptoTransform wrkCriptoTransform = wrkAlgoritmo.CreateDecryptor();
        //    try {
        //        MemoryStream wrkMemoryStream = new MemoryStream(wrkByte, 0, wrkByte.Length);
        //        CryptoStream wrkCriptoStream = new CryptoStream(wrkMemoryStream, wrkCriptoTransform, CryptoStreamMode.Read);
        //        StreamReader wrkStreamReader = new StreamReader(wrkCriptoStream);
        //        return wrkStreamReader.ReadToEnd();
        //    }
        //    catch (System.Exception Return) {
        //        null;
        //    }
        //}
    }
}

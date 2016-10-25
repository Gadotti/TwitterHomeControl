using System;
using System.Text;
using Microsoft.SPOT;
using TwitterHomeControl.Utilitarios;


namespace TwitterHomeControl.Bibliotecas
{
    public class OAuthTwitter
    {

        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            var random = new Random();
            return (random.Next(9999999) + 123400).ToString();
        }

        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.Seconds.ToString();
        }


        public static string GenerateSignature(string url, string consumerKey,
                                               string consumerSecret, string token,
                                               string tokenSecret, string verifier,
                                               string httpMethod, string timeStamp,
                                               string nonce,
                                               out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, verifier, httpMethod, timeStamp, nonce);

            //Generate Key do sha1
            string keySha = consumerSecret + "&" + tokenSecret;
            
            //HMACSHA1 hmacsha1 = new HMACSHA1();
            //hmacsha1.Key =
            //    Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret),
            //                                          string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

            //return GenerateSignatureUsingHash(signatureBase, hmacsha1);
            
            //return HexDisplay.bytesToHex(SHA.computeHMAC_SHA1(
            //    Encoding.UTF8.GetBytes(keySha),
            //    Encoding.UTF8.GetBytes(signatureBase)));

            //%3d
            
            //string retorno = Business.TwitterClient.Base64Encode(Funcoes.EncodingAscii(HexDisplay.bytesToHex(SHA.computeHMAC_SHA1(
            //    Funcoes.EncodingAscii(keySha),
            //    Funcoes.EncodingAscii(signatureBase)))));

            //string retorno = Business.TwitterClient.Base64Encode(Encoding.UTF8.GetBytes(HexDisplay.bytesToHex(SHA.computeHMAC_SHA1(
            //    Funcoes.EncodingAscii(keySha),
            //    Funcoes.EncodingAscii(signatureBase)))));

            string retorno = Business.TwitterClient.Base64Encode(Funcoes.EncodingAscii(HexDisplay.bytesToHex(SHA.computeHMAC_SHA1(
                Encoding.UTF8.GetBytes(keySha),
                Encoding.UTF8.GetBytes(signatureBase)))));

            return retorno;

        }

        public static string GenerateSignatureBase(string url, string consumerKey, 
            string token, string tokenSecret, 
            string verifier, string httpMethod, 
            string timeStamp, string nonce)
        {
            //normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            //if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            //{
            //    normalizedUrl += ":" + url.Port;
            //}

            //TEMPORÁRIO - implementar replace e desfixar este endereço de url
            //GET&http%3A%2F%2Fapi.twitter.com%2F1%2Fdirect_messages.xml&count%3D1%26oauth_consumer_key%3D9VEi7wHNmTSbrN5ho6OQtA%26oauth_nonce%3D3096861%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1283378425%26oauth_token%3D119910496-kk1R7yZ7vhL5DCbDkGiMoomtdwKRSpYCWHOnRmpk%26oauth_version%3D1.0
            const string urlTratada = "http%3A%2F%2Fapi.twitter.com%2F1%2Fdirect_messages.xml&count%3D1%26";
            return httpMethod + "&" + urlTratada + "oauth_consumer_key%3D" + consumerKey + "%26oauth_nonce%3D" + nonce + "%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D" + timeStamp + "%26oauth_token%3D" + token + "%26oauth_version%3D1.0";
            
            
        }

        public static string Teste()
        {
            string url = "http://api.twitter.com/1/direct_messages.xml?count=1&";

            const string consumerKey = "9VEi7wHNmTSbrN5ho6OQtA";
            const string consumerSecret = "qmvRJiPEejgPMS7B6BPUr1clK27zl3JKsQb6vLIEs";
            const string token = "119910496-kk1R7yZ7vhL5DCbDkGiMoomtdwKRSpYCWHOnRmpk";
            const string tokenSecret = "4xglIKfhk4TGnomWVsp9SB6UTq6lwrdtAxwhbxpbo";
            const string verifier = "";

            var nonce = Bibliotecas.OAuthTwitter.GenerateNonce();
            var timeStamp = Bibliotecas.OAuthTwitter.GenerateTimeStamp();

            string outUrl;
            string querystring;

            //Generate Signature
            string sig = Bibliotecas.OAuthTwitter.GenerateSignature(url,
                                                                    consumerKey,
                                                                    consumerSecret,
                                                                    token,
                                                                    tokenSecret,
                                                                    verifier,
                                                                    "GET",
                                                                    timeStamp,
                                                                    nonce,
                                                                    out outUrl,
                                                                    out querystring);

            return string.Concat(url,
                                "oauth_consumer_key=" + consumerKey + "&oauth_nonce=" + nonce +
                                "&oauth_signature_method=HMAC-SHA1&oauth_timestamp=" + timeStamp + "&oauth_token=" +
                                token + "&oauth_version=1.0&oauth_signature=" + sig);
            //http://api.twitter.com/1/direct_messages.xml?count=1&oauth_consumer_key=9VEi7wHNmTSbrN5ho6OQtA&oauth_nonce=3096861&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1283378425&oauth_token=119910496-kk1R7yZ7vhL5DCbDkGiMoomtdwKRSpYCWHOnRmpk&oauth_version=1.0&since_id=0&oauth_signature=yCMwVoDMWP7drQ3TaGFWR8OIZ0M%3d

        }

    }
}


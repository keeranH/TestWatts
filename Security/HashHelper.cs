using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Security
{
    public class HashHelper
    {
        static int nbrDigit = 6;
        private static string watt = "WATT";
        public static string HashPassWord(string password)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
          
        }

        public static Boolean HashPassWordCompare(string Hashpassword,string password)
        {
            var HashInputPassword = "";
            try
            {   if(password != null){
                    HashInputPassword = HashPassWord(password);
                    if (HashInputPassword.ToUpper().Equals(Hashpassword.ToUpper()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    } 
                }
            return false;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Générer Code Interne Watts Green
        /// </summary>
        /// <returns></returns>
        public static string GenererCodeInterne()
        {
            //Random random = new Random();
            //string r = "";
            //for (int i = 0; i < nbrDigit; i++)
            //{
            //    r += random.Next(0, nbrDigit - 1).ToString();
            //}
            //return r;
            return watt+GetUniqueKey(nbrDigit);
        }


        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[36];
            chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
          
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();            
            byte[]  data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

    }
}

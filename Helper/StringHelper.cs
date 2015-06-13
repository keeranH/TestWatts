using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;


namespace Helper
{
    public class StringHelper
    {

        public static string encrypt(string toEncrypt)
        {
            try
            {

                if (toEncrypt != null && toEncrypt.Length != 0)
                {
                     byte[] keyArray;
                     byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                     System.Configuration.AppSettingsReader settingsReader =  new AppSettingsReader();
                     string key = (string)settingsReader.GetValue("SecurityKey",typeof(String));
                  
                     MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                     keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                     hashmd5.Clear();
                        
                     TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                     tdes.Key = keyArray;
                     tdes.Mode = CipherMode.ECB;
                     tdes.Padding = PaddingMode.PKCS7;

                     ICryptoTransform cTransform = tdes.CreateEncryptor();
                     //transform the specified region of bytes array to resultArray
                     byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    //Release resources held by TripleDes Encryptor
                     tdes.Clear();
                    //Return the encrypted data into unreadable string format
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);

                }

                
            }
            catch (Exception)
            {
                
                throw;
            }
            return null;
        }


        public static string Decrypt(string cipherString)
        {
            try
            {
                byte[] keyArray;
                //get the byte code of the string

                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                System.Configuration.AppSettingsReader settingsReader =
                                                    new AppSettingsReader();
                //Get your key from config file to open the lock!
                string key = (string)settingsReader.GetValue("SecurityKey",typeof(String));
                
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();


                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes. 
                //We choose ECB(Electronic code Book)

                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(
                                     toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor                
                tdes.Clear();
                //return the Clear decrypted TEXT
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static string randomStringGenerator(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var randText = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());


            return randText;
        }

        public static string randomStringGenerator(int length,string email)
        {


            
            return null;
        }



    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Econocom.Helper.Models;

namespace Econocom.Helper.File
{
    public class FileHelper
    {
        /// <summary>
        /// e.x SECTEURACTIVITE_20130417
        /// </summary>
        /// <param name="nomFichier"></param>
        /// <returns></returns>
        public static string GetTypeObjet(string nomFichier)
        {
            if (nomFichier != null)
            {
                var contenuNomFichier = nomFichier.Split('_');
                if (contenuNomFichier.Count() <= 1)
                    throw new Exception("ErreurNombreParametre");
                var type = contenuNomFichier.First();
                return type;
            }

            return null;
        }

        /// <summary>
        /// Creer un objet pour representer un fichier telecharger
        /// </summary>
        /// <param name="nomFichier"></param>
        /// <returns></returns>
        public static ModeleFichier GetFileModel(string nomFichier)
        {
            var modeleFichier = new ModeleFichier();
            modeleFichier.NomFichier = nomFichier;

            if (nomFichier != null)
            {
                var contenuNomFichier = nomFichier.Split('_');
                if (contenuNomFichier.Count() != 3)
                    throw new Exception("ErreurNombreParametre");
                modeleFichier.Operation = contenuNomFichier.First();
                modeleFichier.NomTable = contenuNomFichier.ElementAt(1);
                modeleFichier.DateFichier = contenuNomFichier.ElementAt(2);
                return modeleFichier;
            }

            return null;
        }

        public static int GetNumberOfFiles(string path, string extension)
        {
            var numberOfFiles = 0;
            var directory = new DirectoryInfo(path);
            var files = directory.EnumerateFiles().Where(c => c.Extension.ToLower().Equals(extension));           
            if (files.Any())
                numberOfFiles = files.Count();
            return numberOfFiles;
        }

        /// <summary>
        /// We expect to get filename="ImportParc_{ClientId}", ex: ImportParc_22
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool VerifierNomFichierParc(string fileName)
        {
            var fileParams = GetFileParams(fileName);
            if (fileParams.Count == 2)
            {
                var nameOk = fileParams.ElementAt(0).ToLower().Equals("importparc");
                var clientId = -1;
                try
                {
                    clientId=Convert.ToInt32(fileParams.ElementAt(1));
                }
                catch (Exception)
                {
                    throw;
                }
                return clientId > 0 && nameOk;
            }
            return false;
        }

        public static string GetParcSuccessPath(string uploadFolder, int clientId)
        {            
            var year = DateTime.Now.Year;
            var date = DateTime.Now.ToString("yyyyMMdd");
            var range = ((clientId/10000)*10000)+10000;
            uploadFolder = uploadFolder.TrimEnd('/');
           
            var filePath = String.Format("{0}/{1}/{2}/{3}/{4}/OK/", uploadFolder, year, date, range, clientId);
            return filePath;
        }

        public static string GetParcFailPath(string uploadFolder, int clientId)
        {            
            var year = DateTime.Now.Year;
            var date = DateTime.Now.ToString("yyyyMMdd");
            var range = ((clientId / 10000) * 10000) + 10000;
            uploadFolder = uploadFolder.TrimEnd('/');
            
            var filePath = String.Format("{0}/{1}/{2}/{3}/{4}/KO/", uploadFolder, year, date, range, clientId);
            return filePath;
        }

        public static string GetParcFailPathInvalid(string uploadFolder)
        {
            var year = DateTime.Now.Year;
            var date = DateTime.Now.ToString("yyyyMMdd");            
            uploadFolder = uploadFolder.TrimEnd('/');

            var filePath = String.Format("{0}/{1}/{2}/Invalid/", uploadFolder, year, date);
            return filePath;
        }

        public static int GetClientId(string fileName)
        {
            var clientId = -1;

            var fileParams = GetFileParams(fileName);
            if (fileParams.Count == 2)
            {
                clientId = Convert.ToInt32(fileParams.ElementAt(1));
            }

            return clientId;
        }

        public static List<string> GetFileParams(string fileName)
        {
            var fileParams = fileName.Split('_');
            return fileParams.ToList();
        }

    }
}

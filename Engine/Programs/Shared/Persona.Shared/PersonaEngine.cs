using System;
using System.Security.Cryptography;
using System.Text;

namespace Persona
{

    public static class PersonaEngine
    {
        static public DirectoryReference RootDirectory = DirectoryReference.FindRootDirectory();

        static public DirectoryReference EngineDirectroy = RootDirectory.Combine("\\Engine");

        static public DirectoryReference EngineSourceDirectory = EngineDirectroy.Combine("\\Source");

        public static string MakeMD5(string Name)
        {
            string MD5Hash = "";
            MD5 MD5Calculator = MD5.Create();
            byte[] Data = MD5Calculator.ComputeHash(Encoding.UTF8.GetBytes(Name));
            for (int i = 0; i < Data.Length; i++)
            {
                if (i == 4 || i == 6 || i == 8 || i == 10)
                {
                    MD5Hash += "-";
                }
                MD5Hash += Data[i].ToString("x2");
            }
            return MD5Hash.ToUpper();
        }
    }

}
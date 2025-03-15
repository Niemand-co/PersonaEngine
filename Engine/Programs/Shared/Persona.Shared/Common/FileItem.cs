using System;
using System.IO;

namespace Persona
{

    public class FileItem
    {
        public string Content;

        public FileItem()
        {
            Content = "";
        }

        public FileItem(string InContent)
        {
            Content = InContent;
        }

        public FileItem(FileItem OtherFile)
        {
            Content = OtherFile.Content;
        }

        public void Combine(FileItem OtherFile)
        {
            Content += "\r\n" + OtherFile.Content;
        }

        public int Size()
        {
            return Content.Length;
        }

        public void Clear()
        {
            Content = "";
        }

        public static FileItem GetFromFileReference(FileReference Reference)
        {
            Reference.ReadToString(out string Content);
            return new FileItem(Content);
        }

        public static FileItem GetFromFilePath(string FilePath)
        {
            FileStream File = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            StreamReader Reader = new StreamReader(File);
            string Content = Reader.ReadToEnd();

            Reader.Close();
            File.Close();

            return new FileItem(Content);
        }
    }

}
using System;
using System.IO;

namespace Persona
{

    public class FileReference
    {
        public string Name { get; }

        public DirectoryReference Directory { get; }

        public FileReference(string InName, DirectoryReference InDirectory)
        {
            Name = InName;
            Directory = InDirectory;
        }

        public string GetFullName()
        {
            return Directory.GetFullName() + "\\" + Name;
        }

        public bool Exists()
        {
            FileInfo Info = new FileInfo(GetFullName());
            return Info.Exists;
        }

        public void ReadToString(out string Content)
        {
            FileStream File = new FileStream(GetFullName(), FileMode.Open, FileAccess.Read, FileShare.Read);

            StreamReader Reader = new StreamReader(File);
            Content = Reader.ReadToEnd();

            Reader.Close();
            File.Close();
        }

        public bool WriteFileContent(string Content, FileMode Mode)
        {
            DirectoryInfo Dir = new DirectoryInfo(Directory.GetFullName());
            if (!Dir.Exists)
            {
                System.IO.Directory.CreateDirectory(Dir.FullName);
            }

            FileStream File = new FileStream(Directory.GetFullName() + '\\' + Name, Mode, FileAccess.Write, FileShare.None, Content.Length);
            if(File == null)
            {
                return false;
            }

            StreamWriter Writer = new StreamWriter(File);
            Writer.Write(Content);
            Writer.Flush();
            Writer.Close();
            File.Close();
            return true;
        }
    }

}
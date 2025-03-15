using System;
using System.Reflection;
using System.IO;

namespace Persona
{

    [Serializable]
    public class DirectoryReference : IEquatable<DirectoryReference>, IComparable<DirectoryReference>
    {
        
        static public DirectoryReference FindRootDirectory()
        {
            string AssemblyLocation = Assembly.GetEntryAssembly()!.Location;

            DirectoryReference? FoundRootDirectory = DirectoryReference.FromString(AssemblyLocation);

            while(FoundRootDirectory != null)
            {
                int LastSlashIndex = FoundRootDirectory.Directory.LastIndexOf('\\');
                string CurrentFolder = FoundRootDirectory.Directory.Substring(LastSlashIndex + 1);
                if (CurrentFolder.Equals("Engine"))
                {
                    return FoundRootDirectory.GetParentDirectory();
                }
                FoundRootDirectory = FoundRootDirectory.GetParentDirectory();
            }

            return FoundRootDirectory;
        }

        public string Directory { get; }

        public DirectoryReference(string InDirectory)
        {
            Directory = InDirectory;
        }

        static public DirectoryReference FromString(string? Directory)
        {
            if(Directory == null)
            {
                return null;
            }
            else
            {
                return new DirectoryReference(Directory);
            }
        }

        public bool Equals(DirectoryReference? Other)
        {
            if(Other == null)
            {
                return false;
            }
            else
            {
                return Directory == Other.Directory;
            }
        }

        public int CompareTo(DirectoryReference? Other)
        {
            if(Other == null)
            {
                throw new Exception("[DirectoryReference::Error]Cannot be campared with null.");
            }

            return -1;
        }

        public string GetFullName()
        {
            return Directory;
        }

        public string GetFolderName()
        {
            int LastBackSlashIndex = Directory.LastIndexOf('\\');
            int LastSlashIndex = Directory.LastIndexOf("/");
            if(LastBackSlashIndex >= 0)
            {
                return Directory.Substring(LastBackSlashIndex + 1);
            }
            else if(LastSlashIndex >= 0)
            {
                return Directory.Substring(LastBackSlashIndex + 1);
            }
            else
            {
                return Directory;
            }
        }

        public DirectoryReference GetParentDirectory()
        {
            int LastBackSlashIndex = Directory.LastIndexOf('\\');
            int LastSlashIndex = Directory.LastIndexOf("/");
            if (LastBackSlashIndex >= 0)
            {
                return new DirectoryReference(Directory.Substring(0, LastBackSlashIndex));
            }
            else if (LastSlashIndex >= 0)
            {
                return new DirectoryReference(Directory.Substring(0, LastSlashIndex));
            }
            else
            {
                return null;
            }
        }

        public void Iterate(Func<FileReference, bool>? Lambda, Action<DirectoryReference>? DirectoryLambda)
        {
            DirectoryInfo Info = new DirectoryInfo(Directory);

            if(!Info.Exists)
            {
                return;
            }

            if (DirectoryLambda != null)
            {
                DirectoryLambda(this);
            }

            if (Lambda != null)
            {
                foreach (FileInfo fileInfo in Info.GetFiles())
                {
                    FileReference File = new FileReference(fileInfo.Name, this);
                    {
                        if (!Lambda(File))
                        {
                            break;
                        }
                    }
                }
            }

            foreach (DirectoryInfo SubdirectoryInfo in Info.GetDirectories())
            {
                DirectoryReference Subdirectory = new DirectoryReference(SubdirectoryInfo.FullName);
                Subdirectory.Iterate(Lambda, DirectoryLambda);
            }
        }

        public void EnumrateDirectories(Action<DirectoryReference>? Lambda)
        {
            if (Lambda == null)
            {
                return;
            }

            DirectoryInfo Info = new DirectoryInfo(Directory);

            foreach (DirectoryInfo SubdirectoryInfo in Info.GetDirectories())
            {
                DirectoryReference Subdirectory = new DirectoryReference(SubdirectoryInfo.FullName);
                Lambda(Subdirectory);
            }
        }

        public string GetRelativePath(string AbsolutePath)
        {
            string RelativePath = "";

            string[] AbsoluteFolders = AbsolutePath.Split('\\');
            string[] CurrentFolders = Directory.Split('\\');
            int ShorterFolderCount = AbsoluteFolders.Length < CurrentFolders.Length ? AbsoluteFolders.Length : CurrentFolders.Length;
            int SamePathIndex = 0;
            for (; SamePathIndex < ShorterFolderCount; ++SamePathIndex)
            {
                if (AbsoluteFolders[SamePathIndex] != CurrentFolders[SamePathIndex])
                {
                    break;
                }
            }

            for(int FolderIndex = SamePathIndex; FolderIndex < CurrentFolders.Length; ++FolderIndex)
            {
                RelativePath += "..\\";
            }

            for(int FolderIndex = SamePathIndex; FolderIndex < AbsoluteFolders.Length; ++FolderIndex)
            {
                if(FolderIndex == AbsoluteFolders.Length - 1)
                {
                    RelativePath += (AbsoluteFolders[FolderIndex]);
                }
                else
                {
                    RelativePath += (AbsoluteFolders[FolderIndex] + "\\");
                }
            }

            return RelativePath;
        }

        public string GetRelativePath(DirectoryReference Directory)
        {
            return GetRelativePath(Directory.Directory);
        }

        public DirectoryReference Combine(string ChildDirectory)
        {
            return new DirectoryReference(Directory + ChildDirectory);
        }

        public static DirectoryReference Combine(string A, string B)
        {
            return new DirectoryReference(A + B);
        }

        public static DirectoryReference operator+(DirectoryReference Parent, string Folder)
        {
            return Parent.Combine(Folder);
        }
    }

}
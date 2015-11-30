using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDL
{
    public class File
    {
        public File(string fullpath)
        {
            Path = new List<string>();

            foreach (string folder in System.IO.Path.GetDirectoryName(fullpath).Split('\\'))
            {
                Path.Add(folder);
            }

            Name = System.IO.Path.GetFileNameWithoutExtension(fullpath);
            Ext = System.IO.Path.GetExtension(fullpath);

            //Console.Out.WriteLine(GetFullPath());
        }

        public string GetFullPath()
        {
            string full = "";

            foreach (string folder in Path)
            {
                full += folder + "\\";
            }

            return full + Name + Ext;          
        }

        public bool IsWithinDirectory(string directory)
        {
            directory = directory.ToLower();

            foreach (string folder in Path)
            {
                if (folder.ToLower() == directory)
                    return true;
            }

            return false;
        }
        public List<string> Path {get;set;}
        public string Name { get; set; }
        public string Ext { get; set; }
    }
    public class FileManager
    {
        private List<File> _files;

        public List<File> Files { get { return _files; } }

        public FileManager(string path)
        {
            _files = new List<File>();
            DoFolder(path);
        }

        private void DoFolder(string path)
        {
            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                _files.Add(new File(file));
            }

            foreach (string file in System.IO.Directory.GetDirectories(path))
            {
                DoFolder(file);
            }
        }

        public void Copy(string dst, string src)
        {
            string folder = System.IO.Path.GetDirectoryName(dst);
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            if (System.IO.File.Exists(dst))
            {
                System.IO.File.Delete(dst);
            }

            System.IO.File.Copy(src, dst);
        }

        public void Compress(string target)
        {
            FileInfo fileToBeZipped = new FileInfo(target);
            FileInfo zipFileName = new FileInfo(string.Concat(fileToBeZipped.FullName, ".bz2"));

            if (System.IO.File.Exists(zipFileName.FullName))
            {
                System.IO.File.Delete(zipFileName.FullName);
            }

            using (FileStream fileToBeZippedAsStream = fileToBeZipped.OpenRead())
            {
                using (FileStream zipTargetAsStream = zipFileName.Create())
                {
                    try
                    {
                        BZip2.Compress(fileToBeZippedAsStream, zipTargetAsStream, true, 4096);
                        Console.Out.WriteLine("Compressed: " + zipFileName.FullName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

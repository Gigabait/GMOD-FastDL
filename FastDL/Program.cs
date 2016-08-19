using System;
using System.IO;

namespace FastDL
{
    class Program
    {
        static bool IsValidFile(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return false;

            switch (ext.ToLower())
            {
                case ".bsp":
                case ".ain":
                case ".vmt":
                case ".vtf":
                case ".png":
                case ".vtx":
                case ".mdl":
                case ".phy":
                case ".vvd":
                case ".mp3":
                case ".wav":
                case ".pcf":
                case ".ttf":
                    return true;
            }

            return false;
        }

        static bool IsValidDir(string dir)
        {
            switch (dir.ToLower())
            {
                case "maps":
                case "materials":
                case "models":
                case "particles":
                case "resource":
                case "sound":
                    return true;
            }

            return false;
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: <addons path>");
                Console.ReadLine();
                return;
            }

            string path = args[0].Replace("\"", "");
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid Directory: " + path);
                Console.ReadLine();
                return;
            }

            string target = Directory.GetCurrentDirectory() + @"\" + (new DirectoryInfo(path).Name) + "_out";

            if (Directory.Exists(target))
                Directory.Delete(target, true);

            Directory.CreateDirectory(target + @"\lua\autorun\server\");

            StreamWriter rf = new StreamWriter(target + @"\lua\autorun\server\resource_" + Math.Floor((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds) + ".lua");
            FileManager fm = new FileManager(path);
            FileInfo fi;
            int total      = 0;
            int useless    = 0;
            long ignorelen = 0;
            long beforelen = 0;
            long afterlen  = 0;

            foreach (File file in fm.Files)
            {
                if (!IsValidFile(file.Ext))
                    continue;

                string relitivepath = "";
                foreach (string folder in file.Path)
                {
                    if (IsValidDir(folder))
                        break;

                    relitivepath = relitivepath + @"\" + folder;
                }

                string fullpath = file.GetFullPath();
                relitivepath = fullpath.Replace(relitivepath.Substring(1), "");
                string nicepath = relitivepath.Substring(1).Replace(@"\", "/");

                if ((fullpath.Substring(fullpath.Length - 9) == ".xbox.vtx") || (fullpath.Substring(fullpath.Length - 7) == ".sw.vtx"))
                {
                    Console.WriteLine("Removed -> " + nicepath);
                    fi = new FileInfo(fullpath);
                    ignorelen += fi.Length;

                    System.IO.File.Delete(fullpath);
                    useless++;
                    continue;
                }

                if (file.Ext != ".bsp")
                    rf.WriteLine("resource.AddSingleFile'" + nicepath + "'");
                
                fm.Copy(target + relitivepath, fullpath);

                fi = new FileInfo(target + relitivepath);
                beforelen += fi.Length;

                Console.WriteLine("Copied -> " + nicepath);

                fm.Compress(target + relitivepath);

                fi = new FileInfo(target + relitivepath + ".bz2");
                afterlen += fi.Length;

                Console.WriteLine("Compressed -> " + nicepath + ".bz2");

                total++;
            }

            rf.Close();

            Console.WriteLine("\n\nComplete:");
            Console.WriteLine("   Files Compressed: " + total);
            Console.WriteLine("   Files Removed: " + useless);
            Console.WriteLine("   Uncompressed Size: " + beforelen/1024/1024 + "mb");
            Console.WriteLine("   Compressed Size: " + afterlen/1024/1024 + "mb");
            Console.WriteLine("   Total Saved: " + ((ignorelen + beforelen) - afterlen)/1024/1024 + "mb");
            Console.ReadLine();
        }
    }
}
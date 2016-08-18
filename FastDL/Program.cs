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

            Directory.CreateDirectory(target);

            StreamWriter rf = new StreamWriter(target + @"\resource.lua");
            FileManager fm = new FileManager(path);
            int copied = 0;

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

                if (file.Ext != ".bsp")
                    rf.WriteLine("resource.AddSingleFile'" + nicepath + "'");

                fm.Copy(target + relitivepath, fullpath);
                fm.Compress(target + relitivepath);

                Console.WriteLine("Compressed -> " + nicepath);

                copied++;
            }

            rf.Close();

            Console.WriteLine("\nComplete!\n   Files Compressed: " + copied);
            Console.ReadLine();
        }
    }
}
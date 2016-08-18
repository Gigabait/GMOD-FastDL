using System;
using System.IO;

namespace FastDL
{
    class Program
    {
        static public bool IsValidFile(string ext)
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

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: <addons path>");
                Console.ReadLine();
                return;
            }

            if (Directory.Exists(args[0]))
            {
                Console.WriteLine("Invalid Directory: " + args[0]);
                Console.ReadLine();
                return;
            }

            string path = args[0].Replace("\"", "");
            string target = Directory.GetCurrentDirectory() + @"\FastDL";

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

                bool breaknext = false;
                string relitivepath = "";
                foreach (string folder in file.Path)
                {
                    relitivepath = relitivepath + @"\" + folder;

                    if (breaknext)
                        break;

                    if (relitivepath.Substring(1) == path)
                        breaknext = true;
                }

                string fullpath = file.GetFullPath();
                relitivepath = fullpath.Replace(relitivepath.Substring(1), "");
        
                string nicepath = relitivepath.Substring(1).Replace(@"\", "/");

                rf.WriteLine("resource.AddSingleFile'" + nicepath + "'");
                fm.Copy(target + relitivepath, fullpath);
                fm.Compress(target + relitivepath);

                Console.WriteLine("Added -> " + nicepath);

                copied++;
            }

            rf.Close();

            Console.WriteLine("\nComplete!\n   Files Compressed: " + copied);
            Console.ReadLine();
        }
    }
}
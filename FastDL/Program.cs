using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDL
{
    class Program
    {

        static public bool Ignore(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return true;

            ext = ext.ToLower();

            switch(ext)
            {
                case ".lua":
                case ".txt":
                case ".zip":
                case ".rar":
                case ".gitattributes":
                case ".gitignore":
                case ".md":
                case ".ds_store":
                case ".bz2":
                case ".dll":
                case ".exe":
                    return true;
            }

            return false;
        }
        static void Main(string[] args)
        {
            string Path = System.IO.Directory.GetCurrentDirectory();
            string Target = System.IO.Directory.GetCurrentDirectory() + "\\FastDL";

            if (System.IO.Directory.Exists(Target))
                System.IO.Directory.Delete(Target, true);

            FileManager FM = new FileManager(Path);
            string NewFile;
            int Depth;
            int cnt = 0;

            foreach(File file in FM.Files)
            {
                if (Ignore(file.Ext) || !file.IsWithinDirectory("addons"))
                    continue;

                NewFile = "";
                Depth = 0;

                foreach (string folder in file.Path)
                {
                    if (Depth == 0 && folder.ToLower() == "addons")
                    {
                        Depth++;
                        continue;
                    }
                    else if (Depth == 1)
                    {
                        Depth++;
                    }
                    else if (Depth==2)
                    {
                        NewFile += folder+"/";
                    }
                }

                Console.Out.WriteLine(Target + NewFile + file.Name + file.Ext + " -> " + file.GetFullPath());
                FM.Copy(Target + "\\" + NewFile + file.Name + file.Ext, file.GetFullPath());
                FM.Compress(Target + "\\" + NewFile + file.Name + file.Ext);
                cnt++;
            }

            System.Console.Out.WriteLine("\nDONE! "+cnt+" files copied and compressed!");
            System.Console.In.ReadLine();
        }
    }
}

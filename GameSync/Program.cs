using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            var delay = args.Contains("/delay") ? TimeSpan.FromMinutes(10) : new TimeSpan(0);

            var timer = new Timer(SpiderFolders, null, delay, TimeSpan.FromMinutes(10));

            Console.WriteLine("Running");
            Console.ReadLine();
        }

        private static void SpiderFolders(object state)
        {
            var fileService = new FileService();

            var gameRepository = new XmlConfigurationService(@".\config.xml");

            var backupRoot = ConfigurationManager.AppSettings["Destination Folder"] ?? Directory.GetCurrentDirectory();

            var fileRepository = new FileRepository(fileService, backupRoot);

            var folders = gameRepository.GetFolders();

            foreach (var game in folders)
            {
                try
                {
                    fileRepository.EnsureFolderLink(game);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error syncing {0}: {1}", game.Name, ex.Message);
                }
            }
        }
    }
}

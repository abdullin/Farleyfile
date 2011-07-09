using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Lokad.Cqrs;

namespace FarleyFile
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var cache = GetDataFolder();
            using (var source = new CancellationTokenSource())
            {
                var builder = Sys.Configure(cache);
                var fastSubject = new Subject<ISystemEvent>();
                builder.Advanced.Observers.Clear();
                builder.Advanced.RegisterObserver(fastSubject);
                using (var host = builder.Build())
                {
                    Console.WriteLine("Starting host");
                    var task = host.Start(source.Token);
                    var form = new Form1(host, fastSubject);
                    form.FormClosing += (sender, args) => source.Cancel();


                    Application.Run(form);

                    task.Wait(5000);
                }
            }
        }

        static FileStorageConfig GetDataFolder()
        {
            var current = Directory.GetCurrentDirectory();
            var cache = Path.Combine(current, "data");
            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }

            return FileStorage.CreateConfig(cache, "files");
        }
    }
}
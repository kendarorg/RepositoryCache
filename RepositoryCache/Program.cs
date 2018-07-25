using MultiRepositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using MultiRepositories.Repositories;
using System.Windows.Forms;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Reflection;
using Ioc;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace RepositoryCache
{
    class Program
    {
        static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = true;



        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }

        static void Main(string[] args)
        {
            

            var options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            
        }

        static public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                var assemblyDir = Path.GetDirectoryName(path);
                return assemblyDir;
            }
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            var settingsFile = opts.Settings;

            if (!string.IsNullOrWhiteSpace(settingsFile) && File.Exists(settingsFile))
            {
                Console.WriteLine("Reading settings from " + settingsFile);
                opts = JsonConvert.DeserializeObject<Options>(File.ReadAllText(settingsFile));
            }
            if (string.IsNullOrWhiteSpace(opts.Path))
            {
                opts.Path = Directory.GetCurrentDirectory();
            }
            opts.Settings = settingsFile;

            var settings = JsonConvert.SerializeObject(opts);
            Console.WriteLine(settings);

            if (opts.ShowInTray)
            {
                notifyIcon.DoubleClick += (s, e) =>
                {
                    Visible = !Visible;
                    SetConsoleWindowVisibility(Visible);
                };
                //notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                Bitmap bmp = new Bitmap(16, 16, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.FillEllipse(Brushes.Red, 0, 0, 16, 16);
                    g.FillRectangle(Brushes.White, 4, 6, 8, 4);
                }
                notifyIcon.Icon = FlimFlan.IconEncoder.Converter.BitmapToIcon(bmp);
                notifyIcon.Visible = true;
                notifyIcon.Text = "RepositoryCache";

                var contextMenu = new ContextMenuStrip();

                contextMenu.Items.Add("Exit", null, (s, e) => { System.Windows.Forms.Application.Exit(); });
                notifyIcon.ContextMenuStrip = contextMenu;

                SetConsoleWindowVisibility(false);
            }
            if (string.IsNullOrWhiteSpace(opts.Host))
            {
                opts.Host = "http://localhost:"+opts.Port;
            }


            var container = new WindsorContainer();

            RegisterApps(container, new AppProperties(opts.Host,"db"));
            var shttp= container.Resolve<ISimpleHTTPServer>();

            shttp.Start(opts.Path, opts.Port, opts.LogRequests, opts.Urls, opts.Ignores);

            if (opts.ShowInTray)
            {
                System.Windows.Forms.Application.Run();
            }
            else
            {
                while (Console.ReadKey().KeyChar != 'q')
                {
                    Console.WriteLine("");
                    continue;
                }
            }
            shttp.Stop();
        }

        private static void RegisterApps(WindsorContainer container, AppProperties appProperties)
        {
            //container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
            container.Kernel.Resolver.AddSubResolver(new ListResolver(container.Kernel,true));
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel,true));

            var filter = new AssemblyFilter(AssemblyDirectory);
            container.Register(
                Component.For<AppProperties>().Instance(appProperties));
            /*container.Register(
                Classes.FromAssemblyInDirectory(filter)
                    .BasedOn<AppProperties>());*/
            container.Register(
                Classes.FromAssemblyInDirectory(filter)
                    .BasedOn<ISingleton>()
                    .LifestyleSingleton()
                    .WithServiceAllInterfaces());
            container.Register(
                Classes.FromAssemblyInDirectory(filter)
                    .BasedOn<ITransient>()
                    .LifestyleTransient()
                    .WithServiceAllInterfaces());

        }
    }
}

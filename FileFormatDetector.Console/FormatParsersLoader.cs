using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    /// <summary>
    /// Detector plugins loading class
    /// </summary>
    internal class FormatPluginsLoader
    {
        /// <summary>
        /// Directory to search for pluging
        /// </summary>
        public string PluginsDirectory { get; }

        /// <summary>
        /// Loaded general format parsers
        /// </summary>
        public IFormatDetector[] GeneralFormatDetectors { get; private set; } = new IFormatDetector[0];

        /// <summary>
        /// Loaded text-based format parsers
        /// </summary>
        public ITextBasedFormatDetector[] TextBasedFormatDetectors { get; private set; } = new ITextBasedFormatDetector[0];

        /// <summary>
        /// Indicates that any general format parsers were loaded
        /// </summary>
        public bool AnyGeneralFormatDetectorsLoaded => GeneralFormatDetectors.Any();

        /// <summary>
        /// Constructs plugin loader
        /// </summary>
        /// <param name="directory">Directory to look for plugins</param>
        public FormatPluginsLoader(string directory)
        {
            PluginsDirectory = directory;
        }

        /// <summary>
        /// Load plugins from specified directory
        /// </summary>
        public void LoadPlugins()
        {
            List<IFormatDetector> generalFormatDetectors = new List<IFormatDetector>();
            List<ITextBasedFormatDetector> textBasedFormatDetectors = new List<ITextBasedFormatDetector>();

            var plugins = GetLibraries();

            foreach (var plugin in plugins)
            {
                try
                {
                    PluginLoadContext context = new PluginLoadContext(plugin, false);

                    var pluginAssembly = context.LoadFromAssemblyPath(plugin);

                    generalFormatDetectors.AddRange(TryCreateFormatDetector<IFormatDetector>(pluginAssembly));
                    textBasedFormatDetectors.AddRange(TryCreateFormatDetector<ITextBasedFormatDetector>(pluginAssembly));
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error loading plugin {plugin}: {ex.Message}");
                }
            }

            GeneralFormatDetectors = generalFormatDetectors.ToArray();
            TextBasedFormatDetectors = textBasedFormatDetectors.ToArray();
        }

        private IEnumerable<T> TryCreateFormatDetector<T>(Assembly assembly)
        {
            Type[] pluginTypes = assembly.ExportedTypes.Where(t => typeof(T).IsAssignableFrom(t)).ToArray();

            List<T> detectors = new List<T>(pluginTypes.Length);

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var detector = (T?)Activator.CreateInstance(pluginType);

                    if (detector != null)
                        detectors.Add(detector);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error creating detector {pluginType.Name}: {ex.Message}");
                }
            }

            return detectors;
        }

        private IEnumerable<string> GetLibraries()
        {
            if (!Directory.Exists(PluginsDirectory))
                return Array.Empty<string>();

            var subdirs = Directory.GetDirectories(PluginsDirectory);

            List<string> plugins = new List<string>();

            foreach (var subdir in subdirs)
            {
                plugins.AddRange(Directory.GetFiles(subdir, "*Format.dll").Select(Path.GetFullPath));
            }

            return plugins;
        }
    }
}

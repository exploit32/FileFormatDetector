using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    internal class FormatPluginsLoader
    {
        public string PluginsDirectory { get; }

        public List<IBinaryFormatDetector> BinaryFormatDetectors { get; private set; } = new List<IBinaryFormatDetector>();

        public List<ITextFormatDetector> TextFormatDetectors { get; private set; } = new List<ITextFormatDetector>();

        public List<ITextBasedFormatDetector> TextBasedFormatDetectors { get; private set; } = new List<ITextBasedFormatDetector>();

        public bool AnyPluginsLoaded => BinaryFormatDetectors.Count + TextFormatDetectors.Count + TextBasedFormatDetectors.Count > 0;

        public FormatPluginsLoader(string directory)
        {
            PluginsDirectory = directory;
        }

        public void LoadPlugins()
        {
            var plugins = GetLibraries();

            foreach (var plugin in plugins)
            {
                try
                {
                    PluginLoadContext context = new PluginLoadContext(plugin, false);

                    var pluginAssembly = context.LoadFromAssemblyPath(plugin);

                    BinaryFormatDetectors.AddRange(TryCreateFormatDetector<IBinaryFormatDetector>(pluginAssembly));
                    TextFormatDetectors.AddRange(TryCreateFormatDetector<ITextFormatDetector>(pluginAssembly));
                    TextBasedFormatDetectors.AddRange(TryCreateFormatDetector<ITextBasedFormatDetector>(pluginAssembly));
                } catch (Exception ex)
                {
                    System.Console.WriteLine($"Error loading plugin {plugin}: {ex.Message}");
                }
            }
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

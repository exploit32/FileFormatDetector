using FormatApi;
using System.Reflection;
using System.Runtime.Loader;

namespace FileFormatDetector.Console
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath, bool collectible)
         : base(name: Path.GetFileName(pluginPath), collectible)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // Skipping duplicates of FormatApi assembly
            if (assemblyName.Name == typeof(IFormatDetector).Assembly.GetName().Name)
                return null;

            string? target = _resolver.ResolveAssemblyToPath(assemblyName);

            if (target != null)
                return LoadFromAssemblyPath(target);

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}

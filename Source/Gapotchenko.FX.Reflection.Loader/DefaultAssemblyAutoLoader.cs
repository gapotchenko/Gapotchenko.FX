namespace Gapotchenko.FX.Reflection.Loader
{
    sealed class DefaultAssemblyAutoLoader : AssemblyAutoLoader
    {
#if TFF_ASSEMBLYLOADCONTEXT
        private DefaultAssemblyAutoLoader() :
            base(AssemblyLoadContexts.Local)
        {
        }
#else
        private DefaultAssemblyAutoLoader()
        {
        }
#endif

        public static DefaultAssemblyAutoLoader Instance { get; } = new DefaultAssemblyAutoLoader();

        protected override void Dispose(bool disposing)
        {
            // Default instance is not disposable.
        }
    }
}

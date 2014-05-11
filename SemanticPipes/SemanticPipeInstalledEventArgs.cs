using System;

namespace SemanticPipes
{
    public sealed class SemanticPipeInstalledEventArgs : EventArgs
    {
        private readonly Action<PipeOutputPackage> _appendPackageCallback;

        internal SemanticPipeInstalledEventArgs(
            Action<PipeOutputPackage> appendPackageCallback,
            PipeExtension pipeExtension)
        {
            _appendPackageCallback = appendPackageCallback;
            PipeExtension = pipeExtension;
        }

        public PipeExtension PipeExtension { get; private set; }

        public void AppendPackage(PipeOutputPackage package)
        {
            if (package == null) throw new ArgumentNullException("package");

            if (_appendPackageCallback != null)
            {
                _appendPackageCallback(package);
            }
        }
    }
}
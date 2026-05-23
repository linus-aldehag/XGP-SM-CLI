using XgpSaveTools.Records;

namespace XgpSaveTools.SaveHandlers.Impl.Generic
{
    public class GenericHandler : ISaveHandler
    {
        public bool CanHandle(string handlerName) => true;

        public IEnumerable<SaveFile> GetSaveEntries(List<ContainerMetaFile> containers, HandlerArgs? args)
        {
            foreach (var container in containers)
            {
                foreach (var file in container.Files)
                {
                    yield return new(file.Name + (args?.Suffix ?? string.Empty), file);
                }
            }
        }
    }
}
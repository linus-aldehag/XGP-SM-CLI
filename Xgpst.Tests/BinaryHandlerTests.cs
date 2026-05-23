using System;
using System.IO;
using System.Linq;
using Xunit;
using XgpSaveTools.SaveHandlers;
using XgpSaveTools.Records;
using System.Collections.Generic;

namespace Xgpst.Tests
{
    public class BinaryHandlerTests
    {
        [Fact]
        public void GenericHandler_OutputsBaseFilename()
        {
            var handler = SaveHandlerFactory.Get("generic");
            
            // Mock a container and file entry
            var mockFile = new ContainerEntry("GeneralSave", "dummy_path");
            var mockContainer = new ContainerMetaFile("Container1", 1, new List<ContainerEntry> { mockFile });
            
            var entries = handler.GetSaveEntries(new List<ContainerMetaFile> { mockContainer }, null).ToList();
            
            Assert.Single(entries);
            Assert.Equal("GeneralSave", entries[0].OutputName);
        }

        [Fact]
        public void RestoredSaveFiles_ExistAndCanBeRead()
        {
            // Verify the legacy python-extracted saves were restored correctly via project MSBuild copy
            var targetDir = Path.Combine(AppContext.BaseDirectory, "FilesToCheck", "reference");
            Assert.True(Directory.Exists(targetDir), "FilesToCheck/reference directory was not copied to output");

            var files = Directory.GetFiles(targetDir);
            Assert.NotEmpty(files);

            var save00 = files.FirstOrDefault(f => Path.GetFileName(f) == "Save00.sav");
            Assert.NotNull(save00);
            
            // Prove we can read the binary payload
            var bytes = File.ReadAllBytes(save00);
            Assert.True(bytes.Length > 0);
        }
    }
}

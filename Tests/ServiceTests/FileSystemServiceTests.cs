using VueExample.Services;
using Xunit;

namespace VueExample.Tests.ServiceTests
{
    public class FileSystemServiceTests
    {
        [Fact]
        public void FindFolderInTemporaryFolder_Folder_ShouldFindAndReturnFolder()
        {
            var folderName = FileSystemService.FindFolderInTemporaryFolder("VS");
            Assert.Equal("VS", folderName);
        }
    }
}

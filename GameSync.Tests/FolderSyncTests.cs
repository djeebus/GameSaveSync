using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSync.Tests
{
    [TestClass]
    public class FolderSyncTests
    {
        [TestMethod]
        public void EnvironmentVariablesExpandProperly()
        {
            var o = new Folder("mock game", @"%LOCALAPPDATA%");

            var actual = o.Path;

            var expected = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Assert.AreEqual(expected, actual, "Variables not expanded properly");
        }
    }
}

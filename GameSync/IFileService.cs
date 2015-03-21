using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameSync
{
    // info on hard links: http://www.flexhex.com/docs/articles/hard-links.phtml
    public interface IFileService
    {
        void EnsureFolder(string folder);

        void CreateHardLink(string existingPath, string newPath);

        bool DoFoldersPointToSameJunctionPoint(string path1, string path2);

        bool DoFilesPointToSamePoint(string backupPath, string livePath);

        void MoveFile(string livePath, string backupPath);

        void CreateHardLinkForFiles(string backupPath, string livePath);

        void MoveFolder(string livePath, string backupPath);
    }

    class FileService : IFileService
    {
        public bool DoFoldersPointToSameJunctionPoint(string path1, string path2)
        {
            var current = Directory.GetCurrentDirectory();

            path1 = Path.Combine(current, path1);
            path2 = Path.Combine(current, path2);

            var realPath1 = JunctionPoint.GetTarget(path1);
            var realPath2 = JunctionPoint.GetTarget(path2);

            if (realPath1 == null && realPath2 == null)
                return false;

            return path1 == realPath2 ||
                   path2 == realPath1;
        }

        public void CreateHardLink(string existingPath, string newPath)
        {
            if (Directory.Exists(newPath))
            {
                if (Directory.GetFiles(newPath, "*.*").Length > 0)
                    throw new ArgumentOutOfRangeException("newPath", "Folder still has files in it!");

                Directory.Delete(newPath, false);
            }

            JunctionPoint.Create(newPath, existingPath, false);
        }

        public void EnsureFolder(string folder)
        {
            if (Directory.Exists(folder))
                return;

            var parent = Path.GetDirectoryName(folder);
            if (!Directory.Exists(parent))
                EnsureFolder(parent);

            Directory.CreateDirectory(folder);
        }

        public bool DoFilesPointToSamePoint(string backupPath, string livePath)
        {
            throw new NotImplementedException();
        }

        public void CreateHardLinkForFiles(string backupPath, string livePath)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string livePath, string backupPath)
        {
            File.Move(livePath, backupPath);
        }

        public void MoveFolder(string livePath, string backupPath)
        {
            Directory.Move(livePath, backupPath);
        }
    }
}

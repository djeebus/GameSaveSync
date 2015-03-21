using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameSync
{
    public class FileRepository
    {
        public string RootFolder { get; private set; }

        private IFileService _fileService;

        public FileRepository(IFileService fileService, string rootFolder)
        {
            this._fileService = fileService;

            this.RootFolder = EnsureTrailingSlash(rootFolder);

            this._fileService.EnsureFolder(this.RootFolder);
        }

        const string slash = @"\";
        private string EnsureTrailingSlash(string root)
        {
            if (root.EndsWith(slash))
                return root;

            return root + slash;
        }

        private string GetRelativePath(Folder folder, string fullPath)
        {
            var root = EnsureTrailingSlash(folder.Path);

            var u1 = new Uri(root);
            var u2 = new Uri(fullPath);

            return u1.MakeRelativeUri(u2).ToString();
        }

        private string GetBackupFilename(Folder info, string fullPath)
        {
            var relativePath = GetRelativePath(info, fullPath);

            var gameBackupFolder = GetGameBackupFolder(info);

            return Path.Combine(gameBackupFolder, relativePath);
        }

        private string GetGameBackupFolder(Folder info)
        {
            return Path.Combine(this.RootFolder, info.Name);
        }

        public void SyncFolders(Folder folderInfo)
        {
            var gameBackupFolder = GetGameBackupFolder(folderInfo);
            var gameBackupFiles = Directory.GetFiles(gameBackupFolder, "*.*", SearchOption.AllDirectories);

            var liveFolder = folderInfo.Path;
            var liveFiles = Directory.GetFiles(liveFolder, "*.*", SearchOption.AllDirectories);
        }

        internal void EnsureFolderLink(Folder game)
        {
            var backupPath = GetGameBackupFolder(game);

            var livePath = game.Path;

            if (AttemptBackupAsDirectory(game, backupPath, livePath))
                return;
            else if (AttemptBackupAsFile(game, backupPath, livePath))
                return;
        }

        private bool AttemptBackupAsFile(Folder game, string backupPath, string livePath)
        {
            var backupExistsAsFile = File.Exists(backupPath);
            var liveExistsAsFile = File.Exists(livePath);

            if (!backupExistsAsFile && !liveExistsAsFile)
                return false;

            if (backupExistsAsFile && liveExistsAsFile)
            {
                if (this._fileService.DoFilesPointToSamePoint(backupPath, livePath))
                    return true; // already set up

                throw new ArgumentOutOfRangeException("livePath", livePath, "Please delete either the backup file or the live file");
            }
            else if (!liveExistsAsFile)
            {
                Console.Write("Linking '{0}' to backup path", game.Name);
                this._fileService.CreateHardLink(backupPath, livePath);
            }
            else if (!backupExistsAsFile)
            {
                Console.WriteLine("Moving '{0}' to backup path", game.Name);
                var path = Path.GetDirectoryName(backupPath);
                _fileService.EnsureFolder(path);
                _fileService.MoveFile(livePath, backupPath);

                Console.WriteLine("Linking '{0}' to backup path", game.Name);
                _fileService.CreateHardLinkForFiles(backupPath, livePath);
            }
        }

        private bool AttemptBackupAsDirectory(Folder game, string backupPath, string livePath)
        {
            var backupExistsAsDirectory = Directory.Exists(backupPath);
            var liveExistsAsDirectory = Directory.Exists(livePath);

            if (!backupExistsAsDirectory && !liveExistsAsDirectory)
                return false;

            if (backupExistsAsDirectory && liveExistsAsDirectory)
            {
                if (this._fileService.DoFoldersPointToSameJunctionPoint(livePath, backupPath))
                    return true; // already set up!

                throw new ArgumentOutOfRangeException("livePath", livePath, "Please empty either the backup folder or the live folder");
            }
            else if (!liveExistsAsDirectory)
            {
                Console.WriteLine("Linking '{0}' to backup path", game.Name);
                this._fileService.CreateHardLink(backupPath, livePath);
            }
            else if (!backupExistsAsDirectory)
            {
                Console.WriteLine("Moving '{0}' files to backup path", game.Name);
                this._fileService.MoveFolder(livePath, backupPath);

                Console.WriteLine("Linking '{0}' to backup path", game.Name);
                this._fileService.CreateHardLink(backupPath, livePath);
            }

            return true;
        }
    }
}

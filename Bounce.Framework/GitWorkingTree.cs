using System.IO;

namespace Bounce.Framework {
    public class GitWorkingTree : Task {
        [Dependency]
        public Val<string> Repository;
        [Dependency]
        public Val<string> Directory;

        private IGitRepoParser GitRepoParser;
        private IDirectoryUtils DirectoryUtils;
        private readonly IGitCommand GitCommand;

        public GitWorkingTree() : this(new GitRepoParser(), new DirectoryUtils(), new GitCommand()) {}

        public GitWorkingTree(IGitRepoParser gitRepoParser, IDirectoryUtils directoryUtils, IGitCommand gitCommand) {
            GitRepoParser = gitRepoParser;
            DirectoryUtils = directoryUtils;
            GitCommand = gitCommand;
        }

        public override void Build() {
            if (DirectoryUtils.DirectoryExists(WorkingDirectory)) {
                GitCommand.Pull();
            } else {
                GitCommand.Clone(Repository.Value, WorkingDirectory);
            }
        }

        private string WorkingDirectory {
            get {
                if (Directory != null && Directory.Value != null) {
                    return Directory.Value;
                } else {
                    return GitRepoParser.ParseCloneDirectoryFromRepoUri(Repository.Value);
                }
            }
        }

        public override void Clean() {
            DirectoryUtils.DeleteDirectory(WorkingDirectory);
        }

        public Val<string> this[Val<string> filename] {
            get {
                return this.WhenBuilt(() => Path.Combine(WorkingDirectory, filename.Value));
            }
        }
    }
}
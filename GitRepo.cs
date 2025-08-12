
using System;
using System.Diagnostics;
using System.IO;


namespace TrainerManager;

public class GitRepo
{
  public string Location { get; }

  public GitRepo(string dir)
  {
    this.Location = Directory.Exists(dir + "\\.git") ? dir : throw new ArgumentException($"Git: Directory \"{dir}\" is not a git repository.");
  }

  public static GitRepo Clone(string repo, string directory, string branch = "master")
  {
    Directory.CreateDirectory(directory);
    GitRepo.Execute($"clone -b {branch} --single-branch --recursive {repo} \"{directory}\"", directory);
    return new GitRepo(directory);
  }

  public void Pull() => this.Execute("pull");

  public void Execute(string args) => GitRepo.Execute(args, this.Location);

  private static void Execute(string args, string workingDir)
  {
    Process process = new Process()
    {
      StartInfo = {
        FileName = "git.exe",
        Arguments = args,
        UseShellExecute = false,
        WorkingDirectory = workingDir
      }
    };
    if (!process.Start())
      throw new Exception("Git: Failed to start git process.");
    process.WaitForExit();
    if (process.ExitCode != 0)
      throw new Exception($"Git: Command failed with exit code 0x{process.ExitCode:x8}.");
  }
}

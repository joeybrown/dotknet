namespace Dotknet.Cli;

using System;

class Program
{
  /// <summary>
  /// Build a dotnet image
  /// </summary>
  /// <param name="projectPath">Path to dotnet project to compile</param>
  static int Main(string projectPath)
  {
    if (string.IsNullOrWhiteSpace(projectPath))
    {
      Console.WriteLine($"Error: {nameof(projectPath)} was not provided");
      return 1;
    }

    var projectDir = new DirectoryInfo(projectPath);
    if (!projectDir.Exists)
    {
      Console.WriteLine($"Cannot find path {projectDir}");
      return 1;
    }

    Console.WriteLine($"Here's your file path {projectDir}");
    return 0;
  }
}

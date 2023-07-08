using System.Diagnostics;

// ----------------------------------------------------------------------------
//     Get the working directory
// ----------------------------------------------------------------------------
var workingDirectory = Environment.CurrentDirectory;
Console.WriteLine("Working directory: " + workingDirectory);

// ----------------------------------------------------------------------------
//     Check if we are actually in a git repo
// ----------------------------------------------------------------------------
var checkIfGitRepo = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c git rev-parse --is-inside-work-tree",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    }
};

checkIfGitRepo.Start();
var isGitRepo = checkIfGitRepo.StandardOutput.ReadToEnd().Trim();
checkIfGitRepo.WaitForExit();

if (isGitRepo != "true")
{
    Console.WriteLine("Not a git repo, exiting");
    return;
}

// ----------------------------------------------------------------------------
//     Check that the current branch is actually dev
// ----------------------------------------------------------------------------
var checkIfDevBranch = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c git branch --show-current",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    }
};

checkIfDevBranch.Start();
var currentBranch = checkIfDevBranch.StandardOutput.ReadToEnd().Trim();
checkIfDevBranch.WaitForExit();

if (currentBranch != "dev")
{
    Console.WriteLine("Not on dev branch, exiting");
    return;
}

// ----------------------------------------------------------------------------
//     Get the latest tag (version)
// ----------------------------------------------------------------------------
var getLatestTag = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c git describe --abbrev=0 --tags",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    }
};

getLatestTag.Start();
var latestTag = getLatestTag.StandardOutput.ReadToEnd().Trim();
getLatestTag.WaitForExit();

Console.WriteLine("Latest tag: " + latestTag);

// ----------------------------------------------------------------------------
//     Generate the next tag (version)
// ----------------------------------------------------------------------------
var oldVersion = new Version(latestTag);
var newVersion = new Version(oldVersion.Major, oldVersion.Minor + 1, oldVersion.Build);

Console.WriteLine("New tag: " + newVersion);

// ----------------------------------------------------------------------------
//     Now run the Git commands, to:
//     1. Checkout main
//     2. Pull latest   
//     3. Merge dev into main
//     4. Tag the new version
//     5. Push the new tag
//     6. Checkout dev
// ----------------------------------------------------------------------------
var commandList = new List<string>
{
    "git checkout main",
    "git pull",
    "git merge -X theirs dev",
    $"git tag -a {newVersion} -m \"new release\"",
    "git push --follow-tags",
    "git checkout dev"
};

foreach (var cmd in commandList)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {cmd}",
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    
    process.Start();
    process.WaitForExit();
}

// ----------------------------------------------------------------------------
//     We are done
// ----------------------------------------------------------------------------
Console.WriteLine("Published new release");
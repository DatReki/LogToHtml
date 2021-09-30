# Log2Html

<a href="https://github.com/DatReki/LogToHtml/actions/workflows/dotnet.yml">
    <img src="https://github.com/DatReki/LogToHtml/actions/workflows/dotnet.yml/badge.svg" />
</a>
<a href="https://www.nuget.org/packages/LogToHtml/">
    <img src="https://img.shields.io/nuget/v/LogToHtml?style=flat-square" />
</a>

A small library to write (error) logs to a .html file.
The html file's structure is based of an embedded .cshtml file.

Currently tested working on Linux & Windows.

<img src="https://cdn.discordapp.com/attachments/406006274661154818/892449912980131920/unknown.png" alt="ExampleImage.png">

## Usage
Write a log
```cs
using Log2Html;

class Program
{
    public static Logging.Options options = new()
    {
        Projects = new List<string>()
        {
            $"{Assembly.GetCallingAssembly().GetName().Name}"
        },
        Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
        LogToConsole = true
    };

    static void Main(string[] args)
    {
        Logging.Log(options, Logging.LogType.Warn, "Testing");
    }
}
```

Retrieve written logs
```cs
//Returns lists with errors all of different log levels written
var logs = Logging.GetLogs();
```
Options
```cs
public static Logging.Options options = new()
{
    Projects = new List<string>()
    {
        $"{Assembly.GetCallingAssembly().GetName().Name}"
    },
    Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
    Date = DateTime.UtcNow,
    FilePath = Path.Combine(Environment.CurrentDirectory, "logging", "loggin.html"),
    LogToConsole = true
};
```
1. <strong>Projects</strong>: A list of projects within the solution (so you can write different logs for each project).
2. <strong>Project</strong>: name of current project that is being used for logging (change this option for each solution obviously).
3. <strong>Date</strong>: DateTime format you want to be used in the logs (default is `DateTime.UtcNow`).
4. <strong>FilePath</strong>: The path you want the file to be written to and it's name.
5. <strong>LogToConsole</strong>: Boolean indicating wether or not you want the library to also write to the console.

## Performance
Currently if you're just writing a log every so often performance is fine but if you write a massive amount to it (300-1000+) it will slow down significantly. I do know ways to make the library faster but currently do not have time to implement these.

## To Do
- [ ] Make sure the library has read access to the file it's logging to if not wait for it to become accessible. 
- [ ] Make the logging process happen in threads this way it won't cause delays for the program that's writing the logs.
- [ ] Store the edited HTML as a string inside the library and edit this instead of reading the file first (if the HTML gets updated it will obviously still be written to the log file. However keeping the file as a variable will allow for faster access).
- [ ] Create a queue system for the logs to reduce the amount of IO calls the library needs to make.

## Libraries

Log2Html utilizes: 
1. <a href="https://github.com/toddams/RazorLight">RazorLight</a> to read embedded .cshtml and convert it to string
2. <a href="https://html-agility-pack.net/">Html Agility Pack</a> to read and write to the .html file
3. <a href="https://anglesharp.github.io/">ΛngleSharp</a> to format the HTML
4. <a href="https://github.com/silkfire/Pastel">Pastel</a> to write colors to the console

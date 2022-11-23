# Log2Html
A small library to write (error) logs to a .html file. The html file's structure is based of an embedded .cshtml file.

Currently tested working on Linux & Windows.

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
1. **Projects**: A list of projects within the solution (so you can write different logs for each project).
2. **Project**: name of current project that is being used for logging (change this option for each solution obviously).
3. **Date**: DateTime format you want to be used in the logs (default is DateTime.UtcNow).
4. **FilePath**: The path you want the file to be written to and it's name.
5. **LogToConsole**: Boolean indicating wether or not you want the library to also write to the console.

## Performance
Currently if you're just writing a log every so often performance is fine but if you write a massive amount to it (300-1000+) it will slow down significantly. I do know ways to make the library faster but currently do not have time to implement these.

## Libraries
Log2Html utilizes:

1. [RazorLight](https://github.com/toddams/RazorLight) to read embedded .cshtml and convert it to string
2. [Html Agility Pack](https://html-agility-pack.net/) to read and write to the .html file
3. [ΛngleSharp](https://anglesharp.github.io/) to format the HTML
4. [Pastel](https://github.com/silkfire/Pastel) to write colors to the console
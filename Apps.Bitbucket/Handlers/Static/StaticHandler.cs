using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Handlers.Static;

public class LanguageDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("abap", "ABAP"),
            new("actionscript", "ActionScript"),
            new("ada", "Ada"),
            new("android", "Android"),
            new("apex", "Apex"),
            new("arc", "Arc"),
            new("asciidoc", "AsciiDoc"),
            new("asp", "ASP"),
            new("assembly", "Assembly"),
            new("bash", "Bash"),
            new("c", "C"),
            new("c#", "C#"),
            new("c++", "C++"),
            new("clojure", "Clojure"),
            new("coffeescript", "CoffeeScript"),
            new("css", "CSS"),
            new("dart", "Dart"),
            new("elixir", "Elixir"),
            new("erlang", "Erlang"),
            new("f#", "F#"),
            new("go", "Go"),
            new("graphql", "GraphQL"),
            new("groovy", "Groovy"),
            new("haskell", "Haskell"),
            new("html", "HTML"),
            new("java", "Java"),
            new("javascript", "JavaScript"),
            new("json", "JSON"),
            new("julia", "Julia"),
            new("kotlin", "Kotlin"),
            new("lua", "Lua"),
            new("matlab", "MATLAB"),
            new("objective-c", "Objective-C"),
            new("other", "Other"),
            new("perl", "Perl"),
            new("php", "PHP"),
            new("powershell", "PowerShell"),
            new("python", "Python"),
            new("r", "R"),
            new("ruby", "Ruby"),
            new("rust", "Rust"),
            new("scala", "Scala"),
            new("shell", "Shell"),
            new("sql", "SQL"),
            new("swift", "Swift"),
            new("typescript", "TypeScript"),
            new("vue", "Vue"),
            new("xml", "XML"),
            new("yaml", "YAML")
        };
    }
}

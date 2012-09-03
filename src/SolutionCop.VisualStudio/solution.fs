namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common

[<RequireQualifiedAccess>]
module Solution = 

    type Configuration = {
        Name : String;
        Platform : String;
        OutputPath : String;
    }

    type Project = {
        FileName : String;
        Configurations : Configuration list;
        ReferencePaths : String list;
    }

    type Solution = {
        Directory : String;
        FileName : String;
        Projects : Project list;
    }

    let parse (settings : Args.Settings) = 
        { Directory = ""; FileName = ""; Projects = []; }
        
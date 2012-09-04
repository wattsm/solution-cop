namespace SolutionCop.VisualStudio

open System

[<RequireQualifiedAccess>]
module Project = 

    type Data = {
        FileName : String;
        Configurations : Configuration.Data list;
        ReferencePaths : String list;
    }
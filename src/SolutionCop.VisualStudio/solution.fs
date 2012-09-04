namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common

[<RequireQualifiedAccess>]
module Solution = 

    type Data = {
        Directory : String;
        FileName : String;
        Projects : Project.Data list;
    }

    let load settings = 
        { Directory = ""; FileName = ""; Projects = []; }
        
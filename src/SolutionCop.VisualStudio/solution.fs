namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common
open SolutionCop.Common.IO

[<RequireQualifiedAccess>]
module Solution = 

    ///Exception describing an invalid solution file
    type InvalidSolutionException () =
        inherit Exception ()

    ///A record containing relevant information about a VS solution
    type Data = {
        Directory : String;
        FileName : String;
        Projects : Project.Data list;
    }

    ///Reads a VS solution from the given solution file contents
    let read str = 
        { Directory = ""; FileName = ""; Projects = []; }

    ///Loads the VS solution identified in the given settings
    let load (settings : Args.Settings) = 
        contentsOf settings.Path
        |> read

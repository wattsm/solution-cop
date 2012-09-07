namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common
open SolutionCop.Common.IO
open SolutionCop.Common.Regex

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

    ///Reads project paths from the given solution file contents
    let getProjectPaths = 
        (matches ",(\\s?)\"((.+?)\\.(fsproj|csproj))\"")  
        >> List.map (groupAt 1)   

    ///Loads projects referenced by the given solution file contents
    let private projects solution = 
        getProjectPaths solution
        |> List.map Project.load

    ///Loads the VS solution identified in the given settings
    let load (settings : Args.Settings) = 
        {
            Directory = (directoryOf settings.Path);
            FileName = (nameOf settings.Path);
            Projects = (contentsOf settings.Path |> projects);
        }
        
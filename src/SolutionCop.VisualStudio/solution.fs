namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common
open SolutionCop.Common.IO
open SolutionCop.Common.Regex

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
    let private projects directory solution = 
        getProjectPaths solution
        |> List.map ((combine directory) >> Project.load)

    ///Loads the VS solution identified in the given settings
    let load path = 

        let directory = directoryOf path
        let filename = nameOf path
        let contents = contentsOf path

        {
            Directory = directory;
            FileName = filename;
            Projects = (projects directory contents);
        }
        
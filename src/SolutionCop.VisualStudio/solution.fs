namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common.IO
open SolutionCop.Common.Regex
open SolutionCop.VisualStudio.Project

module Solution = 

    [<Literal>]
    let SolutionExtension = ".sln"

    type SolutionDescription = {
        Directory : String;
        FileName : String;
        Projects : ProjectDescription list;
    }

    //TODO Support other project file types

    let extractRelativeProjectPaths = 
        (matches ",(\\s?)\"((.+?)\\.(fsproj|csproj))\"")  
        >> List.map (group 1)        

    let extractProjectPaths path = 

        if (extension path) <> SolutionExtension then
            raise (InvalidOperationException ())

        let directory = 
            folder path

        path
        |> fileContents
        |> extractRelativeProjectPaths
        |> List.map (combine directory)

    let describeSolutionAt path = 

        let name = 
            fileName path

        let directory = 
            folder path

        let projects = 
            extractProjectPaths path
            |> List.choose parseProjectAt

        { Directory = directory; FileName = name; Projects = projects; }
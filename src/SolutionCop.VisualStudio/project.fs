namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common.Xml
open SolutionCop.Common.IO

module Project = 

    ///Exception used to describe an invalid project file
    type InvalidProjectException (inner : Exception) =
        inherit Exception ("The project file was not formatted as expected.", inner)

        new () = InvalidProjectException (null)

    ///Record modelling the basic data relevant data in a VS project
    type Data = {
        Directory : String;
        FileName : String;        
        Configurations : Configuration.Data list;
        ReferencePaths : String list;
    }

    ///Reads project configurations from an XML node
    let private configurations context = 
        context
        |> selectMany (Schema.prefix "PropertyGroup[@Condition]")
        |> List.map Configuration.read

    ///Reads project reference hint paths from an XML node
    let private references context = 
        context
        |> selectMany (Schema.prefixMany "{0}:ItemGroup/{0}:Reference/{0}:HintPath")
        |> List.map toValue

    ///Reads the output file name from an XML node
    let private filename context = 

        let toExtension (s : String) = 
            match (s.ToLower ()) with
            | "library" -> "dll"
            | "exe" | "winexe" -> "exe"
            | _ -> raise (InvalidProjectException ())
        
        try 

            let group = 
                context
                |> selectSingle (Schema.prefix "PropertyGroup[not(@Condition)]")

            let name = 
                group
                |> selectSingle (Schema.prefix "AssemblyName")
                |> toValue

            let extension = 
                group
                |> selectSingle (Schema.prefix "OutputType")
                |> toValue
                |> toExtension

            String.Format ("{0}.{1}", name, extension)

        with
            | :? XPathQueryException as e -> raise (InvalidProjectException (e))
        
    ///Reads a project from an XML node
    let read directory context = 
        {
            Directory = directory;
            FileName = (filename context);
            Configurations = (configurations context);
            ReferencePaths = (references context);
        }

    ///Reads a project from a path on disk
    let load path = 

        let directory = 
            directoryOf path

        contentsOf path
        |> parse
        |> Schema.register
        |> (read directory)
        

namespace SolutionCop.FxCop

open System
open SolutionCop.Common
open SolutionCop.Common.IO
open SolutionCop.VisualStudio.Solution

module Settings = 

    type AnalysisSettings = {
        Directory : String;
        FileName : String;
        Targets : String list;
        ReferencePaths : String list;
    }

    let private getTargets solution buildConfig platform filter = 
        solution.Projects
        |> List.filter (fun project -> filter project.AssemblyName)
        |> List.choose (fun project -> 

                let config = 
                    project.Configurations
                    |> List.tryFind (fun config ->                             
                            String.equalIgnoreCase config.BuildConfig buildConfig
                                && String.equalIgnoreCase config.Platform platform
                        )

                match config with
                | Some config' -> Some (combine config'.OutputPath project.AssemblyName)
                | _ -> None
            )

    let noFilter = 
        fun _ -> true

    let settingsFor buildConfig platform filter solution =

        let targets = 
            getTargets solution buildConfig platform filter

        let name = 
            solution.FileName.Replace (".sln", ".fxcop")

        let referencePaths = 
            solution.Projects
            |> List.collect (fun project -> project.ReferencePaths)
            |> List.toSeq
            |> Seq.distinct
            |> Seq.toList   //TODO Fix this

        { Directory = solution.Directory; FileName = name; Targets = targets; ReferencePaths = referencePaths; }
        
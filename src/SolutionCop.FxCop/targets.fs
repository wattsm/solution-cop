namespace SolutionCop.FxCop

open System
open SolutionCop.Common
open SolutionCop.Common.Args
open SolutionCop.Common.Regex
open SolutionCop.Common.IO
open SolutionCop.VisualStudio.Solution
open SolutionCop.VisualStudio.Project
open SolutionCop.VisualStudio.Configuration

module Targets =

    ///Records describing basic target data for an FxCop project
    type Data = {
        Binaries : String list;
        ReferencePaths : String list;
    }

    ///True if the given filename matches the inclusion filter(s)
    let private incl filename settings =
        match settings.Include with 
        | [] -> true
        | patterns -> matchesAny patterns filename

    ///True if the given filename matches the exclusion filter(s)
    let private excl filename settings = 
        match settings.Exclude with
        | [] -> false
        | patterns -> matchesAny patterns filename

    ///Applies configuration name and platform filters to the given solution
    let private filterConfigurations settings solution = 

        let projects' = 
            solution.Projects
            |> List.choose (fun project -> 
                
                    let config = 
                        project.Configurations
                        |> List.tryFind (fun c -> 

                                String.equalIgnoreCase c.Name settings.Configuration
                                    && String.equalIgnoreCase c.Platform settings.Platform

                            )

                    match config with
                    | Some config' -> Some { project with Configurations = [ config'; ]; }
                    | _ -> None
                )

        { solution with Projects = projects'; }

    ///Applies inclusion and exclusion filters to the given solution
    let private filterProjects settings solution = 
        
        let projects' = 
            solution.Projects
            |> List.choose (fun project ->
                    if (incl project.FileName settings) && (not (excl project.FileName settings)) then
                        Some project
                    else
                        None
                )

        { solution with Projects = projects'; }

    ///Extracts target binaries from the given solution
    let private binaries solution = 
        solution.Projects
        |> List.map (fun project -> 

                let configuration = 
                    List.head project.Configurations

                let directory = 
                    combine project.Directory configuration.OutputPath
            
                combine directory project.FileName
            )

    ///Extracts target reference paths from the given solution
    let private references solution = 
        solution.Projects
        |> List.collect (fun project -> 
                project.ReferencePaths
                |> List.map (fun path -> combine project.Directory path)
            )

    ///Extracts target binaries and reference paths from the given solution
    let extract settings solution = 

        let solution' = 
            solution
            |> filterConfigurations settings
            |> filterProjects settings

        { 
            Binaries = (binaries solution'); 
            ReferencePaths = (references solution'); 
        }
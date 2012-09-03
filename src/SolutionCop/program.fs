namespace SolutionCop

open System
open System.IO
open System.Text.RegularExpressions
open SolutionCop.VisualStudio.Solution
open SolutionCop.VisualStudio.Project
open SolutionCop.FxCop.Settings
open SolutionCop.FxCop.Project
open SolutionCop.FxCop
open SolutionCop.Common.IO
open SolutionCop.Common.Regex

module Program = 

    (**
        Usage:
            solutioncop.exe -sln:"C:\MySolution\MySolution.sln" platform:AnyCPU configuration:Debug -include:"^MySolution\." - exclude:"\.(Tests)\."
    **)

    type ProgramSettingsException (message) =
        inherit Exception (message)

    type ProgramSettings = {
        SolutionPath : String;
        ProjectName : String;
        Configuration : String;
        Platform : String;
        Includes : String list;
        Excludes : String list;
    } 

    let findSetting (key : string) (arg : string) =

        let pattern = 
            String.Format ("^-{0}:(\\\"?)(.+?)(\\\"?)$", key)

        matchAt pattern 1 arg
        
    let getSetting (key : string) (required : bool) (args : string []) = 

        let arg = 
            args
            |> Array.tryPick (findSetting key)

        match arg with
        | Some str -> str
        | _ -> 
            if required then
                raise (ProgramSettingsException (String.Format ("The {0} argument is required.", key)))
            else
                String.Empty

    let getPathSetting = 
        (getSetting "sln" true) 
        
    let getConfigurationSetting = 
        (getSetting "configuration" true)
        
    let getPlatformSetting = 
        (getSetting "platform" true)       

    let getProjectName = 
        (getSetting "name" false)

    let getIncludes args = 
        args
        |> Array.Parallel.choose (findSetting "include")
        |> Array.toList

    let getExcludes args = 
        args
        |> Array.Parallel.choose (findSetting "exclude")
        |> Array.toList

    let parseProgramSettings (args : string []) = 
        {
            SolutionPath = getPathSetting args;
            ProjectName = getProjectName args;
            Configuration = getConfigurationSetting args;
            Platform = getPlatformSetting args;
            Includes = getIncludes args;
            Excludes = getExcludes args;
        }
        
    let getFilter settings = 
        match (settings.Includes, settings.Excludes) with
        | ([], []) -> noFilter
        | (includes, []) -> Filters.matchesAny includes
        | ([], excludes) -> Filters.matchesNone excludes
        | (includes, excludes) -> 
            (fun str -> 
                 
                    (Filters.matchesAny includes str)
                        && (Filters.matchesNone excludes str)
                    
                )            

    [<EntryPoint>]
    let main args = 

        let programSettings = 
            parseProgramSettings args

        let filter = 
            getFilter programSettings

        let analysisSettings  =
            describeSolutionAt programSettings.SolutionPath
            |> settingsFor programSettings.Configuration programSettings.Platform filter

        let projectXml = 
            projectFor analysisSettings            

        let path = 
            
            let filename = 
                match programSettings.ProjectName with
                | "" -> analysisSettings.FileName
                | str -> String.Format ("{0}.fxcop", str)

            Path.Combine ((folder programSettings.SolutionPath), filename)

        if (File.Exists path) then
            File.Delete path

        projectXml.Save (path)

        0

namespace SolutionCop

open System
open SolutionCop.Common.Regex
open SolutionCop.FxCop

module Settings = 

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
        | ([], []) -> Settings.noFilter
        | (includes, []) -> Filters.matchesAny includes
        | ([], excludes) -> Filters.matchesNone excludes
        | (includes, excludes) -> 
            (fun str -> 
                 
                    (Filters.matchesAny includes str)
                        && (Filters.matchesNone excludes str)
                    
                )  
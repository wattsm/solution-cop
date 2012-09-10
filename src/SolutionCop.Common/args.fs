namespace SolutionCop.Common

open System
open SolutionCop.Common.Regex
open SolutionCop.Common.IO

module Args = 

    ///Exception raised when a mandatory setting is not specified
    type MissingSettingException (key : string) =
        inherit Exception (String.Format ("The setting {0} is required.", key))

    ///Extract a setting value from an argument
    let private findSetting (key : string) (arg : string) =

        let pattern = 
            String.Format ("^-{0}:(\\\"|'?)(.+?)(\\\"|'?)$", key)

        matchAt pattern 1 arg
        
    ///Gets the setting with a given key from an argument list
    let getSetting (key : string) (required : bool) (args : string []) = 
    
        let arg = 
            args
            |> Array.tryPick (findSetting key)

        match arg with
        | Some str -> str
        | _ -> 
            if required then
                raise (MissingSettingException (key))
            else
                String.Empty

    ///Gets one or more settings with the given key from an argument list
    let getSettings (key : string) (required : bool) (args : string[]) = 

        let settings = 
            args
            |> Array.choose (findSetting key)
            |> Array.toList

        match settings with
        | [] -> 
            if required then
                raise (MissingSettingException (key))
            else
                []
        | _ -> settings
            

    ///Record for target filters and related settings
    type TargetSettings = {
        Configuration : String;
        Platform : String;
        Include : String list;
        Exclude : String list;
    }

    ///Record for FxCop output settings
    type OutputSettings = {
        Directory : String;
        FileName : String;
        BasedOn : String;
    }

    ///Gets the solution file name from the given argument list
    let getFileName = 
        getSetting "sln" true

    ///Gets the target settings from the givern argument list
    let getTargetSettings (args : string []) =
        { 
            Configuration = (getSetting "configuration" true args); 
            Platform = (getSetting "platform" true args); 
            Include = (getSettings "include" false args); 
            Exclude = (getSettings "exclude" false args); 
        }

    ///Gets the output settings from the given argument list
    let getOutputSettings (args : string []) =   
        {
            Directory = (directoryOf (getSetting "sln" true args));
            FileName = (getSetting "name" false args);  
            BasedOn = (getSetting "based-on" false args); 
        }
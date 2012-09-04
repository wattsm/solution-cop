namespace SolutionCop.Common

open System
open SolutionCop.Common.Regex

[<RequireQualifiedAccess>]
module Args = 

    ///Exception raised when a mandatory setting is not specified
    type MissingSettingException (key : string) =
        inherit Exception (String.Format ("The setting {0} is required.", key))
        
    ///Gets the setting with a given key from an argument list
    let getSetting (key : string) (required : bool) (args : string []) = 

        let findSetting (key : string) (arg : string) =

            let pattern = 
                String.Format ("^-{0}:(\\\"|'?)(.+?)(\\\"|'?)$", key)

            matchAt pattern 1 arg

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

    ///Record describing basic program settings
    type Settings = {
        Path : String;
        Configuration : String;
        Platform : String;        
    }
    with

        static member Empty = 
            {
                Path = "";
                Configuration = "";
                Platform = "";
            }

    ///Parses program settings from the given argument list
    let parse (args : string []) = 

        let get key = 
            getSetting key true args

        {
            Path = (get "sln");
            Configuration = (get "configuration");
            Platform = (get "platform");
        }
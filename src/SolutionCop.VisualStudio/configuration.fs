namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common.Xml
open SolutionCop.Common.Regex

module Configuration = 

    ///Exception describing an invalid project configuration
    type InvalidConfigurationException (message) =
        inherit Exception (message)

    ///A record describing a project configuration
    type Data = {
        Name : String;
        Platform : String;
        OutputPath : String;
    }

    ///Try to get a value or throw an exception
    let private tryGet xpath (desc : string) = 
        try 
            selectSingleValue xpath
        with
        | :? XPathQueryException -> raise (InvalidConfigurationException (String.Format ("Missing or invalid {0}.", desc)))

    ///Gets the output path for a configuration
    let private path = 
        tryGet (Schema.prefix "OutputPath") "output path"

    ///Gets the platform for a configuration
    let private platformTarget =
        tryGet (Schema.prefix "PlatformTarget") "platform"

    ///Gets the name for a configuration
    let private configurationName = 
        tryGet (Schema.prefix "ConfigurationName") "configuration name"

    ///Active pattern matching an XML node with a condition attribute containing both configuration name and platform
    let private (|NameAndPlatform|_|) (condition : string) = 

        let values = 
            condition
            |> matchesAt "\\s+'\\$\\(Configuration\\)\\|\\$\\(Platform\\)' == '(.+?)\\|(.+?)'\\s+" [ 0; 1; ]

        match values with
        | Some (config::platform::_) -> Some (config, platform)
        | _ -> None

    ///Active pattern matching an XML node with a condition attribute containing only the configuration name
    let private (|NameOnly|_|) (condition : string) = 
        condition
        |> matchAt "\\s+'\\$\\(Configuration\\)' == '(.+?)'\\s+" 0

    ///Active pattern matching an XML node with a condition attribute containing only the platform
    let private (|PlatformOnly|_|) (condition : string) = 
        condition
        |> matchAt "\\s+'\\$\\(Platform\\)' == '(.+?)'\\s+" 0

    ///Reads a configuration from an XML node
    let read context = 

        let condition =
            context
            |> getAttribute "Condition"

        let name, platform = 
            match condition with
            | NameAndPlatform (name', platform') -> name', platform'
            | NameOnly name' ->  (name', (platformTarget context))
            | PlatformOnly platform' -> ((configurationName context), platform')
            | _ -> raise (InvalidConfigurationException "Could not parse Condition attribute.")        

        { 
            Name = name; 
            Platform = platform; 
            OutputPath = (path context);
        }

namespace SolutionCop.VisualStudio

open System
open System.Xml
open System.Xml.XPath
open System.IO
open SolutionCop.Common
open SolutionCop.Common.IO
open SolutionCop.Common.XPath
open SolutionCop.Common.Regex

module Project = 

    [<Literal>]
    let ProjectSchemaNamespace = "http://schemas.microsoft.com/developer/msbuild/2003"

    let private projectExtensions = 
        [ ".csproj"; ".fsproj"; ]   //TODO Configuration

    type ProjectParseException () =
        inherit Exception ()

    type ProjectConfiguration = {
        BuildConfig : string;
        Platform : string;
        OutputPath : string;
    }

    type ProjectDescription = {
        AssemblyName : String;
        Configurations : ProjectConfiguration list;
        ReferencePaths : String list;
    }

    let private (|BuildConfigAndPlatform|_|) (condition : string) = 

        let values = 
            condition
            |> matchesAt "\\s+'\\$\\(Configuration\\)\\|\\$\\(Platform\\)' == '(.+?)\\|(.+?)'\\s+" [ 0; 1; ]

        match values with
        | Some (config::platform::_) -> Some (config, platform)
        | _ -> None

    let private (|BuildConfigOnly|_|) (condition : string) = 
        condition
        |> matchAt "\\s+'\\$\\(Configuration\\)' == '(.+?)'\\s+" 0

    let private (|PlatformOnly|_|) (condition : string) = 
        condition
        |> matchAt "\\s+'\\$\\(Platform\\)' == '(.+?)'\\s+" 0

    let private (|ValidConfiguration|_|) (context : XmlContext) = 
        
        let condition = 
            context
            |> attr "Condition"

        match condition with
        | BuildConfigAndPlatform (config, platform) -> Some (config, platform)
        | BuildConfigOnly config -> 

            match (valueOf "p:PlatformTarget" context) with
            | Some platform -> Some (config, platform)
            | _ -> None

        | PlatformOnly platform -> 

            match (valueOf "p:ConfigurationName" context) with
            | Some config -> Some (config, platform)
            | _ -> None

        | _ -> None

    let private parseProjectConfiguration (context : XmlContext) =
        match context with
        | ValidConfiguration (config, platform) -> 

            match (valueOf "p:OutputPath" context) with
            | Some path -> Some { BuildConfig = config; Platform = platform; OutputPath = path; }
            | _ -> None

        | _ -> None

    let private extractRelativeReferencePaths context = 
        context
        |> nodes "//p:Project/p:ItemGroup/p:Reference/p:HintPath"
        |> List.map (fun xml -> xml.Navigator.Value)

    let private extractRelativeConfigurations context = 
        context        
        |> nodes "//p:Project/p:PropertyGroup[@Condition]" 
        |> List.choose parseProjectConfiguration

    let private extractReferencePaths path context = 
        
        let directory = 
            folder path

        context
        |> extractRelativeReferencePaths
        |> List.map ((combine directory) >> folder)
        |> List.toSeq
        |> Seq.distinct
        |> Seq.toList //TODO this is horrible, fix

    let private extractConfigurations path context = 

        let folder' = 
            folder path

        context
        |> extractRelativeConfigurations
        |> List.map (fun config -> { config with OutputPath = (combine folder' config.OutputPath); })

    let parseProject path project = 

        let ext (outputType : string) = 
            match (outputType.ToLower ()) with
            | "exe" | "winexe" -> "exe"
            | _ -> "dll"

        let context = 
            project
            |> parse
            |> navigate
            |> prefix "p" ProjectSchemaNamespace

        let assemblyName = 
            valueOf "//p:Project/p:PropertyGroup[not(@Condition)]/p:AssemblyName" context

        let outputType = 
            valueOf "//p:Project/p:PropertyGroup[not(@Condition)]/p:OutputType" context

        match (assemblyName, outputType) with
        | (Some n, Some t) -> 

            let assemblyName' = 
                String.Format ("{0}.{1}", n, ext t)

            let configurations = 
                extractConfigurations path context        

            let referencePaths = 
                extractReferencePaths path context
        
            Some { AssemblyName = assemblyName'; Configurations = configurations; ReferencePaths = referencePaths; }

        | _ -> None

    let parseProjectAt path = 

        if not (List.contains (extension path) projectExtensions) then
            raise (InvalidOperationException ())

        try 
            path
            |> fileContents
            |> (parseProject path)
        with
            | :? FileNotFoundException -> None  //Solution files can point to projects which do not exist on disk

﻿namespace SolutionCop.FxCop

open System
open System.Reflection
open SolutionCop.Common
open SolutionCop.Common.IO
open SolutionCop.Common.Xml
open SolutionCop.VisualStudio
open SolutionCop.FxCop.Targets

[<RequireQualifiedAccess>]
module Project = 

    ///The name of the default template
    [<Literal>]
    let private TemplateName = "default.fxcop"

    ///Gets the default template 
    let private getDefaultTemplate () = 

        let assembly = Assembly.GetExecutingAssembly ()
        
        assembly.GetManifestResourceStream TemplateName
        |> readAll
        |> parse

    ///Parses an FxCop project to create a template
    let private getTemplateBasedOn path = 
        path
        |> contentsOf
        |> parse
        |> removeChildren "//FxCopProject/Targets" "Target"
        |> removeChildren "//FxCopProject/Targets" "AssemblyReferenceDirectories"
        |> removeChildren "//FxCopProject" "FxCopReport"
        |> removeAttribute "//FxCopProject" "Name"

    //TODO Move above into constants

    ///Gets the XML template to be used for the project 
    let private template (settings : Args.OutputSettings) = 

        let context = 
            if (String.IsNullOrEmpty settings.BasedOn) then
                getDefaultTemplate ()
            else
                getTemplateBasedOn settings.BasedOn

        context
        |> appendChild "//FxCopProject/Targets" "AssemblyReferenceDirectories" []

    ///Gets the project name
    let private name (settings : Args.OutputSettings) = 
        settings.FileName.Substring (0, settings.FileName.IndexOf (".fxcop"))
        
    ///Condenses the given path by using the $(ProjectDir) placeholder
    let private condense (settings : Args.OutputSettings) (path : String) = 
        path.Replace (settings.Directory, "$(ProjectDir)")
    
    ///Generates an FxCop project file based on the given settings and targets
    let generate (settings : Args.OutputSettings) targets =

        let appendTargets context = 

            let appendTarget context' path = 

                let attrs =
                    [
                        ("Name", condense settings path); 
                        ("Analyze", "True"); 
                        ("AnalyzeAllChildren", "True");
                    ]

                context'
                    |> appendChild "//FxCopProject/Targets" "Target" attrs

            targets.Binaries
            |> List.fold appendTarget context

        let appendReferencePaths context = 

            let appendReferencePath context' path = 
                context'
                |> appendContent "//FxCopProject/Targets/AssemblyReferenceDirectories" "Directory" (condense settings path)

            targets.ReferencePaths
            |> Seq.map directoryOf
            |> Seq.distinct
            |> Seq.fold appendReferencePath context

        let appendName context = 
            context
            |> appendAttribute "//FxCopProject" "Name" (name settings)        
        
        template settings
        |> appendName
        |> appendTargets
        |> appendReferencePaths      

    ///Creates an FxCop project file based on the given settings and targets
    let create (settings : Args.OutputSettings) targets =

        let filename = 
            combine settings.Directory settings.FileName

        generate settings targets
        |> save filename


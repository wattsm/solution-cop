namespace SolutionCop.FxCop

open System
open System.Xml
open System.Xml.XPath
open System.Reflection
open SolutionCop.FxCop.Settings
open SolutionCop.Common.Xml
open SolutionCop.Common.IO

module Project = 

    let private getTemplate () = 

        let assembly = Assembly.GetExecutingAssembly ()
        use stream = assembly.GetManifestResourceStream "template-default.fxcop"
        
        let xml = XmlDocument ()
        xml.Load (stream)

        xml

    let projectNameFor settings = 
        settings.FileName.Substring (0, settings.FileName.LastIndexOf ("."))
        
    let projectFor (settings : AnalysisSettings) = 

        let appendReferenceDirectories targetsNode = 

            let directories = 
                settings.ReferencePaths                
                |> Seq.distinct

            let referencesNode =
                targetsNode
                |> addNode "AssemblyReferenceDirectories" []
                |> node "AssemblyReferenceDirectories"

            let appendDirectory referencesNode' (directory : string) = 
                referencesNode'
                |> addContentNode "Directory" [] (directory.Replace (settings.Directory, "$(ProjectDir)"))

            directories
            |> Seq.fold appendDirectory referencesNode
            |> ignore

            targetsNode

        let appendTargets targetsNode = 

            let appendTarget targetsNode' (target : string) =
                if (exists target) then

                    let path = 
                        target.Replace (settings.Directory, "$(ProjectDir)")

                    let attrs = 
                        [
                            ("Name", path);
                            ("Analyze", "True");
                            ("AnalyzeAllChildren", "True");
                        ]

                    targetsNode'
                    |> addNode "Target" attrs
                else
                    targetsNode'

            settings.Targets
            |> List.fold appendTarget targetsNode

        getTemplate ()
        |> node "//FxCopProject"
        |> addAttr "Name" (projectNameFor settings)
        |> owner
        |> node "//FxCopProject/Targets"
        |> appendReferenceDirectories
        |> appendTargets        
        |> owner
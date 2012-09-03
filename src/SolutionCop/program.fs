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

    [<EntryPoint>]
    let main args = 

        let programSettings = 
            Settings.parseProgramSettings args

        let filter = 
            Settings.getFilter programSettings

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

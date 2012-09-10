namespace SolutionCop

open System
open SolutionCop.Common
open SolutionCop.VisualStudio
open SolutionCop.FxCop

module Program = 

    (**
        Usage:
            solutioncop.exe -sln:"C:\MySolution\MySolution.sln" -platform:AnyCPU -configuration:Debug -include:"^MySolution\." - exclude:"\.(Tests)\."
    **)    

    [<EntryPoint>]
    let main args = 

        let targetSettings = 
            Args.getTargetSettings args

        let outputSettings = 
            Args.getOutputSettings args

        args 
        |> Args.getFileName
        |> Solution.load
        |> Targets.extract targetSettings
        |> Project.create outputSettings

        0
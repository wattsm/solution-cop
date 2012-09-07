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

        let filters = 
            Filters.parse args

        let properties = 
            Properties.parse args

        args 
        |> Args.parse
        |> Solution.load
        |> Targets.extract filters
        |> Project.generate properties

        0
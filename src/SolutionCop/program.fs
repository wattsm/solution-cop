namespace SolutionCop

open System
open SolutionCop.Common
open SolutionCop.VisualStudio
open SolutionCop.FxCop

module Program = 

    (**
        Usage:
            solutioncop.exe -sln:"C:\MySolution\MySolution.sln" platform:AnyCPU configuration:Debug -include:"^MySolution\." - exclude:"\.(Tests)\."
    **)    

    [<EntryPoint>]
    let main args = 

        args 
        |> Args.parse
        |> Solution.parse
        |> Filters.apply (Filters.parse args)
        |> Project.generate args

        0
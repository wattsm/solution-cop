namespace SolutionCop.FxCop

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Filters = 

    let endsWith str (target : String) = 
        target.EndsWith str

    let contains str (target : String) = 
        target.Contains str

    let matches pattern (target : String) = 
        Regex.IsMatch (target, pattern, RegexOptions.IgnoreCase)

    let matchesAll patterns (target : String) = 
        patterns
        |> List.exists (fun pattern -> not (matches pattern target))
        |> not

    let matchesAny patterns (target : String) = 
        patterns
        |> List.exists (fun pattern -> matches pattern target)
    
    let matchesNone patterns (target : String) = 
        not (matchesAny patterns target)
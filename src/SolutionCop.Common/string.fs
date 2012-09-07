namespace SolutionCop.Common

open System

[<RequireQualifiedAccess>]
module String = 

    ///True if two strings are equal ignoring case
    let equalIgnoreCase (str1 : string) (str2 : string) = 
        match (str1, str2) with
        | (null, null) -> true
        | (_, null) | (null, _) -> false
        | _ -> 
            str1.Equals (str2, StringComparison.OrdinalIgnoreCase)
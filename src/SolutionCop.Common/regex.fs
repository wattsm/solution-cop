namespace SolutionCop.Common

open System
open System.Text.RegularExpressions

module Regex = 

    let matches pattern input = 
        if (String.IsNullOrEmpty input) then
            []
        else
            Regex.Matches (input, pattern, RegexOptions.IgnoreCase)
            |> List.ofEnumerable<Match>

    let group index (match' : Match) = 
        match'.Groups.[index + 1].Value
            
    let matchesAt pattern indices input = 
        if (String.IsNullOrWhiteSpace input) then
            None
        else

            let match' =
                Regex.Match (input, pattern, RegexOptions.IgnoreCase)

            if match'.Success then            
                indices
                |> List.map (fun i -> match'.Groups.[i + 1].Value)
                |> Some
            else
                None

    let matchAt pattern index input = 
        match (matchesAt pattern [index] input) with
        | Some values -> Some (List.head values)
        | _ -> None

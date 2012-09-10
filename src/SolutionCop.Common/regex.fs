namespace SolutionCop.Common

open System
open System.Text.RegularExpressions

module Regex = 

    ///Gets the matches of regular expression against an input
    let matches pattern input = 
        if (String.IsNullOrEmpty input) then
            []
        else
            Regex.Matches (input, pattern, RegexOptions.IgnoreCase)
            |> List.ofEnumerable<Match>

    ///Gets the value of the group at the given index
    let groupAt index (match' : Match) = 
        match'.Groups.[index + 1].Value
            
    ///Applies a regular expression, returning the values of groups at the given indices
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

    ///Applies a regular expression, returning the value of a group at a given index
    let matchAt pattern index input = 
        match (matchesAt pattern [index] input) with
        | Some values -> Some (List.head values)
        | _ -> None

    ///True if the given input matches at least one of the provided patterns. Returns true when no patterns provided.
    let matchesAny patterns input = 
        patterns
        |> List.exists (fun pattern -> Regex.IsMatch (input, pattern, RegexOptions.IgnoreCase))
namespace SolutionCop.Common.Tests

open System
open System.Text.RegularExpressions
open SolutionCop.Common.Regex
open Xunit
open FsUnit.Xunit

module Regex = 

    let text =
        "The rain in Spain falls mainly on the plains"

    module ``Given a regular expression`` = 
        module ``And a piece of text containing no matches`` =
            type ``When matches is called`` () = 

                [<Fact>] member test.
                    ``Then an empty list is returned`` () = 
                        matches "\\d+" text |> should equal List.empty<Match>

        module ``And a piece of text contains multiple matches`` =
            type ``When matches is called`` () =

                let containsCorrectMatches (items : Match list) = 
                    
                    let items' = 
                        items
                        |> List.map (fun m -> m.Groups.[0].Value)

                    (List.length items') = 4
                        && not (List.exists (fun str -> str <> "ain") items')

                [<Fact>] member test.
                    ``Then the correct matches are returned`` () = 
                        matches "ain" text |> containsCorrectMatches |> should be True
            
        
namespace SolutionCop.Common.Tests

open System
open System.Collections
open Xunit
open FsUnit.Xunit
open SolutionCop.Common

module List = 

    let strings length = 
        Array.init length (fun i -> String.Format ("String #{0}", i))

    module ``Given an empty enumerable collection`` =
        type ``When it is passed to ofEnumerable`` () = 
            
            [<Fact>] member test.
                ``Then an empty list is returned`` () = 
                    strings 0 |> List.ofEnumerable |> should equal []

    module ``Given an enumerable collection of strings`` = 
        type ``When it is passed to ofEnumerable`` () =

            let containsCorrectStrings strings = 
                List.exists ((=) "String #0") strings
                    && List.exists ((=) "String #1") strings
                    && List.exists ((=) "String #2") strings

            [<Fact>] member test.
                ``Then a list containing the same strings is returned`` () =
                    strings 3 |> List.ofEnumerable |> containsCorrectStrings |> should be True

    module ``Given two lists of strings`` =

        type ``When they contain the same items`` () = 

            [<Fact>] member test.
                ``Then same returns true`` () = 
                    List.same [ 1; 3; 5; 7; ] [ 1; 3; 5; 7; ] |> should be True

        type ``When they contain different items`` () = 

            [<Fact>] member test.
                ``Then same returns false`` () =
                    List.same [ 1; 3; 5; 7; ] [ 1; 2; 3; 4;] |> should be False

        type ``When they are of different lengths`` () =

            [<Fact>] member test.
                ``Then same returns false`` () = 
                    List.same [ 1; 2; 3; 4; ] [ 1; 2; ] |> should be False
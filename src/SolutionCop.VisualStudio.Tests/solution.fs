namespace SolutionCop.VisualStudio.Tests

open System
open System.IO
open System.Reflection
open SolutionCop.Common
open SolutionCop.VisualStudio.Solution
open Xunit
open FsUnit.Xunit

module Solution = 

    module ``Given a Visual Studio 2012 solution file containing no projects`` =
        type ``When extractRelativeProjectPaths is called`` () = 

            [<Fact>] member test.
                ``Then an empty list is returned`` () = 
                    extractRelativeProjectPaths "" |> should equal List.empty<string>

    module ``Given a Visual Studio 2012 solution file containing multiple projects`` = 
        type ``When extractRelativePaths is called`` () = 

            let containsCorrectProjectPaths items = 

                let contains str = 
                    List.contains str items

                (List.length items) = 4
                    && contains "SolutionCop.VisualStudio\SolutionCop.VisualStudio.fsproj"
                    && contains "SolutionCop\SolutionCop.csproj"
                    && contains "SolutionCop.Common\SolutionCop.Common.fsproj"
                    && contains "SolutionCop.Common.Tests\SolutionCop.Common.Tests.fsproj"

            [<Fact>] member test.
                ``Then the correct project paths are returned`` () =
                    getFileContents "2012-solution.txt" |> extractRelativeProjectPaths |> containsCorrectProjectPaths |> should be True
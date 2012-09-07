namespace SolutionCop.VisualStudio.Tests

open System
open System.Xml
open SolutionCop.Common.Xml
open SolutionCop.Common
open SolutionCop.VisualStudio
open FsUnit.Xunit
open Xunit

module ``Given the contents of the Visual Studio solution`` =

    type ``When project paths are extracted`` () = 

        //TODO Example only contains .fsproj extensions

        let expectedPaths = 
            [
                "SolutionCop.VisualStudio\SolutionCop.VisualStudio.fsproj";
                "SolutionCop\SolutionCop.fsproj";
                "SolutionCop.Common\SolutionCop.Common.fsproj";
                "SolutionCop.FxCop\SolutionCop.FxCop.fsproj";
                "SolutionCop.Common.Tests\SolutionCop.Common.Tests.fsproj";
                "SolutionCop.VisualStudio.Tests\SolutionCop.VisualStudio.Tests.fsproj";
            ]

        let solutionContents  =             
            Embedded.xml "solution-data.xml"
            |> toContext
            |> selectSingle "//examples/example[@name = 'standard']"
            |> toValue
        
        [<Fact>] member test.
            ``Then the correct paths are returned`` () =
                solutionContents |> Solution.getProjectPaths |> List.same expectedPaths |> should be True
namespace SolutionCop.FxCop.Tests

open System
open FsUnit.Xunit
open Xunit
open SolutionCop.VisualStudio.Solution
open SolutionCop.VisualStudio.Project
open SolutionCop.VisualStudio.Configuration

module ``Given a parsed Visual Studio solution`` = 

    let solution = 
        {
            Directory = @"C:\MySolution";
            FileName = "MySolution.sln";
            Projects = 
                [                    
                ];
        }

    module ``And the target settings contain inclusion filters`` = 

        type ``When targets are extracted`` () = 

            [<Fact>] member test.
                ``Then only targets matching one or more of the filters are included`` () =
                    ()

    module ``And the target settings contain exclusion filters`` = 

        type ``When targets are extracted`` () = 

            [<Fact>] member test.
                ``Then only targets not matching any of the filters are included`` () =
                    ()

    type ``When targets are extracted`` () = 

        [<Fact>] member test.
            ``Then only targets with the correct configuration are included`` () =
                ()

        [<Fact>] member test.
            ``Then only targets with the correct platform are included`` () =
                ()
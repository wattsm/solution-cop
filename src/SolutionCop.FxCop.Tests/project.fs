namespace SolutionCop.FxCop.Tests

open System
open FsUnit.Xunit
open Xunit
open SolutionCop.Common
open SolutionCop.Common.Xml
open SolutionCop.Common.Args
open SolutionCop.FxCop
open SolutionCop.FxCop.Targets

(**
Given some a set of analysis targets
    When an FxCop report is generated
        Then all targets are included
        Then targets paths use the ProjectDir placeholder
        Then all reference paths are included
        Then reference paths use the ProjectDir placeholder
        Then name attribute is correctly set
**)
module ``Given some a set of analysis targets`` =

    let settings =
        {
            Directory = @"C:\MySolution";
            FileName = "MySolution.fxcop";
            BasedOn = String.Empty;
        }

    let targets =
        {
            Binaries = 
                [
                    @"C:\Directory\Assembly.dll";
                    @"C:\Directory\Executable.exe";
                ];
            ReferencePaths = 
                [
                    @"C:\References-1\R1.dll";
                    @"C:\References-2\R2.dll";
                ];
        }

    type ``When an FxCop report is generated`` () =

        [<Fact>] member test.
            ``Then all targets are included`` () =
                Project.generate settings targets
                |> selectMany "//FxCopProject/Targets/Target"
                |> List.map (getAttribute "Name")
                |> List.same [ @"C:\Directory\Assembly.dll"; @"C:\Directory\Executable.exe"; ]
                |> should be True

        [<Fact>] member test.
            ``Then target paths use the ProjectDir placeholder`` () =
                let 
                    settings' = { settings with Directory = @"C:\Directory" }
                in
                    Project.generate settings' targets
                    |> selectMany "//FxCopProject/Targets/Target"
                    |> List.map (getAttribute "Name")
                    |> List.same [ @"$(ProjectDir)\Assembly.dll"; @"$(ProjectDir)\Executable.exe"; ]
                    |> should be True

        [<Fact>] member test.
            ``Then all reference paths are included`` () =
                Project.generate settings targets
                |> selectMany "//FxCopProject/Targets/AssemblyReferenceDirectories/Directory"
                |> List.map toValue
                |> List.same [ @"C:\References-1"; @"C:\References-2"; ] 
                |> should be True

        [<Fact>] member test.
            ``Then reference paths use the ProjectDir placeholder`` () =
                let
                    targets' = 
                        { 
                            targets 
                            with 
                                ReferencePaths = 
                                    [
                                        @"C:\MySolution\References-1\R1.dll";
                                        @"C:\MySolution\References-2\R2.dll";
                                    ]; 
                        }
                in
                    Project.generate settings targets'
                    |> selectMany "//FxCopProject/Targets/AssemblyReferenceDirectories/Directory"
                    |> List.map toValue
                    |> List.same [ @"$(ProjectDir)\References-1"; @"$(ProjectDir)\References-2"; ]
                    |> should be True

        [<Fact>] member test.
            ``Then the name attribute is correctly set`` () =
                Project.generate settings targets
                |> selectSingle "//FxCopProject"
                |> getAttribute "Name"
                |> should equal "MySolution"

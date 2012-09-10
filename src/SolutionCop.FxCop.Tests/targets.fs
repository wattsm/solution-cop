namespace SolutionCop.FxCop.Tests

open System
open FsUnit.Xunit
open Xunit
open SolutionCop.VisualStudio.Solution
open SolutionCop.VisualStudio.Project
open SolutionCop.VisualStudio.Configuration
open SolutionCop.FxCop
open SolutionCop.Common
open SolutionCop.Common.Args

module ``Given a parsed Visual Studio solution`` = 

    let solution = 
        {
            Directory = @"C:\MySolution";
            FileName = "MySolution.sln";
            Projects = 
                [ 
                    {
                        Directory = @"C:\MySolution\MyProject-1";
                        FileName = "MyProject-1.exe";
                        Configurations = 
                            [
                                {
                                    Name = "Debug";
                                    Platform = "AnyCPU";
                                    OutputPath = @"bin\Debug\";
                                }
                            ];
                        ReferencePaths = 
                            [
                                @"..\Refs\Reference-1.dll";
                                @"..\Refs\Reference-2.dll";
                            ];
                    };
                    {
                        Directory = @"C:\MySolution\MyProject-2";
                        FileName = "MyProject-2.dll";
                        Configurations = 
                            [
                                {
                                    Name = "Debug";
                                    Platform = "x86";
                                    OutputPath = @"bin\Debug\";
                                };
                                {
                                    Name = "Debug";
                                    Platform = "AnyCPU";
                                    OutputPath = @"bin\Debug\";
                                }
                            ];
                        ReferencePaths = 
                            [
                                @"..\Refs\Reference-3.dll";
                            ];
                    }
                ];
        }

    let binaries (data : Targets.Data) = data.Binaries
    let references (data : Targets.Data) = data.ReferencePaths        

    module ``And the target settings contain inclusion filters`` = 

        type ``When targets are extracted`` () = 

            let settings = 
                {
                    Configuration = "Debug";
                    Platform = "AnyCPU";
                    Include = [ "([2-9]+)"; ];
                    Exclude = [];
                }

            [<Fact>] member test.
                ``Then only targets matching one or more of the filters are included`` () =
                    Targets.extract settings solution |> binaries |> List.same [ @"C:\MySolution\MyProject-2\bin\Debug\MyProject-2.dll"; ] |> should be True

            [<Fact>] member test.
                ``Then only references associated with valid targets are included`` () =
                    Targets.extract settings solution |> references |> List.same [ @"C:\MySolution\Refs\Reference-3.dll"; ] |> should be True

    module ``And the target settings contain exclusion filters`` = 

        type ``When targets are extracted`` () = 

            let settings = 
                {
                    Configuration = "Debug";
                    Platform = "AnyCPU";
                    Include = [];
                    Exclude = [ "([2-9]+)"; ];
                }

            [<Fact>] member test.
                ``Then only targets not matching any of the filters are included`` () =
                    Targets.extract settings solution |> binaries |> List.same [ @"C:\MySolution\MyProject-1\bin\Debug\MyProject-1.exe"; ] |> should be True

            [<Fact>] member test.
                ``Then only references associated with valid targets are included`` () =
                    Targets.extract settings solution |> references |> List.same [ @"C:\MySolution\Refs\Reference-1.dll"; @"C:\MySolution\Refs\Reference-2.dll"; ] |> should be True

    type ``When targets are extracted`` () = 

        [<Fact>] member test.
            ``Then only targets with the correct configuration are included`` () =
                let
                    settings =
                        { 
                            Configuration = "Debug"; 
                            Platform = "AnyCPU"; 
                            Include = []; 
                            Exclude = []; 
                        }                             
                in
                    Targets.extract settings solution |> binaries |> List.same [ @"C:\MySolution\MyProject-2\bin\Debug\MyProject-2.dll"; @"C:\MySolution\MyProject-1\bin\Debug\MyProject-1.exe"; ] |> should be True

        [<Fact>] member test.
            ``Then only targets with the correct platform are included`` () =
                let
                    settings    = 
                        {
                            Configuration = "Debug";
                            Platform = "x86";
                            Include = [];
                            Exclude = [];
                        }
                in
                    Targets.extract settings solution |> binaries |> List.same [ @"C:\MySolution\MyProject-2\bin\Debug\MyProject-2.dll"; ] |> should be True
namespace SolutionCop.FxCop.Tests

open System
open FsUnit.Xunit
open Xunit
open SolutionCop.Common
open SolutionCop.VisualStudio.Solution
open SolutionCop.FxCop.Settings

module Settings = 

    module ``Given a SolutionDescription`` =
        
        let solution = 
            {
                Directory = @"C:\MyProject";
                FileName = "MySolution.sln";
                Projects = 
                    [
                        {
                            AssemblyName = "MySolution.Common.dll";
                            Configurations = 
                                [
                                    {
                                        BuildConfig = "Debug";
                                        Platform = "AnyCPU";
                                        OutputPath = @"C:\MySolution\MySolution.Common\bin\Debug\";
                                    };
                                    {
                                        BuildConfig = "Release";
                                        Platform = "AnyCPU";
                                        OutputPath = @"C:\MySolution\MySolution.Common\bin\Release\";
                                    }
                                ];
                            ReferencePaths = 
                                [
                                    @"C:\MySolution\References-1";
                                    @"C:\MySolution\References-2";
                                ];
                        };
                        {
                            AssemblyName = "MySolution.Model.dll";
                            Configurations = 
                                [
                                    {
                                        BuildConfig = "Debug";
                                        Platform = "x86";
                                        OutputPath = @"C:\MySolution\MySolution.Model\bin\Debug\";
                                    };
                                    {
                                        BuildConfig = "Release";
                                        Platform = "AnyCPU";
                                        OutputPath = @"C:\MySolution\MySolution.Model\bin\Release\";
                                    }
                                ];
                            ReferencePaths = 
                                [
                                    @"C:\MySolution\References-1";
                                    @"C:\MySolution\References-3";
                                ];
                        }
                    ];
            }

        let paths settings = 
            settings.Targets

        let references (settings : AnalysisSettings) = 
            settings.ReferencePaths

        type ``When settingsFor is called`` () = 

            let fxCopFileName (settings : AnalysisSettings) = 
                settings.FileName
        
            [<Fact>] member test.
                ``Then the resulting AnalysisSettings contains the correct FxCop file name`` () = 
                    settingsFor "Debug" "AnyCPU" noFilter solution |> fxCopFileName |> should equal "MySolution.fxcop"

            [<Fact>] member test.
                ``Then the resulting AnalysisSettings contain the correct reference paths`` () = 
                    settingsFor "Debug" "AnyCPU" noFilter solution |> references |> List.same [ @"C:\MySolution\References-1"; @"C:\MySolution\References-2"; @"C:\MySolution\References-3"; ] |> should be True

        type ``When settingsFor is called for Debug AnyCPU with no filter`` () =

            [<Fact>] member test.
                ``Then the resulting AnalysisSettings contains the correct target paths`` () =
                    settingsFor "Debug" "AnyCPU" noFilter solution |> paths |> List.same [ @"C:\MySolution\MySolution.Common\bin\Debug\MySolution.Common.dll"; ] |> should be True

        type ``When settingsFor is called for Release AnyCPU with a filter`` () =

            let filter (path : string) = 
                not (path.Contains ("MySolution.Common.dll"))

            [<Fact>] member test.
                ``Then the resulting AnalysisSettings contains the correct target paths`` () = 
                    settingsFor "Release" "AnyCPU" filter solution |> paths |> List.same [ @"C:\MySolution\MySolution.Model\bin\Release\MySolution.Model.dll"; ] |> should be True
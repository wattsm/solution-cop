namespace SolutionCop.VisualStudio.Tests

open System
open FsUnit.Xunit
open Xunit
open SolutionCop.Common
open SolutionCop.VisualStudio.Project

module Project = 

    let path = @"C:\Projects\SolutionCop.Common\SolutionCop.Common.fsproj"

    let assemblyName project = 
        match project with
        | Some p -> p.AssemblyName
        | _ -> String.Empty

    let configCount project = 
        match project with
        | Some p -> List.length p.Configurations
        | _ -> -1

    let references project = 
        match project with
        | Some p -> p.ReferencePaths
        | _ -> []

    let getConfiguration project = 
        match project with
        | Some p -> List.head p.Configurations
        | _ -> { BuildConfig = ""; Platform = ""; OutputPath = ""; }

    module ``Given a Visual Studio 2012 project file`` = 
        
        type ``When parseProject is called`` () =
            
            [<Fact>] member test.
                ``Then the resulting ProjectDescription has the correct AssemblyName`` () = 
                    getFileContents "2012-project-full.xml" |> parseProject path |> assemblyName |> should equal "SolutionCop.Common.dll"

            [<Fact>] member test.
                ``Then the resulting ProjectDescription has the correct AssemblyName (executable)`` () = 
                    getFileContents "2012-project-full-exe.xml" |> parseProject path |> assemblyName |> should equal "SolutionCop.Common.exe"

            [<Fact>] member test.
                ``Then the resulting ProjectDescription has the correct number of configurations`` () = 
                    getFileContents "2012-project-full.xml" |> parseProject path |> configCount |> should equal 2

            [<Fact>] member test.
                ``Then the resulting ProjectDescription has the correct reference paths`` () =
                    getFileContents "2012-project-full.xml" |> parseProject path |> references |> List.same [ @"C:\Projects\References\log4net"; @"C:\Projects\References\Microsoft"; ] |> should be True

    module ``Given an invalid Visual Studio 2012 project file`` =

        type ``When parseProject is called`` () =

            [<Fact>] member test.
                ``Then None is returned`` () =
                    "<not-a-project />" |> parseProject path |> should equal None

    module ``Given a Visual Studio 2012 project file with a property group with a build config and platform condition``  =

        type ``When parseProject is called`` () = 

            [<Fact>] member test.
                ``Then the resulting ProjectDescription contains the correct build configuration, platform and output path`` () = 
                    getFileContents "2012-project-both.xml" |> parseProject path |> getConfiguration |> should equal { BuildConfig = "Debug"; Platform = "AnyCPU"; OutputPath = @"C:\Projects\SolutionCop.Common\bin\Debug\"; }

    module ``Given a Visual Studio 2012 project file with a property group with a build config condition and target platform element`` = 
        
        type ``When parseProject is called`` () =

            [<Fact>] member test.
                ``Then the resulting ProjectDescription contains the correct build configuration, platform and output path`` () = 
                    getFileContents "2012-project-config.xml" |> parseProject path |> getConfiguration |> should equal { BuildConfig = "Debug"; Platform = "AnyCPU"; OutputPath = @"C:\Projects\SolutionCop.Common\bin\Debug\"; }

    module ``Given a Visual Studio 2012 project file with a property group with a platform condition and a configuration name element`` = 

        type ``When parseProject is called`` () =

            [<Fact>] member test.
                ``Then the resulting ProjectDescription contains the correct build configuration, platform and output path`` () = 
                    getFileContents "2012-project-platform.xml" |> parseProject path |> getConfiguration |> should equal { BuildConfig = "Debug"; Platform = "AnyCPU"; OutputPath = @"C:\Projects\SolutionCop.Common\bin\Debug\"; }
                    
        
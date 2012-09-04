namespace SolutionCop.VisualStudio.Tests

open System
open System.Xml
open SolutionCop.Common.Xml
open SolutionCop.VisualStudio
open FsUnit.Xunit
open Xunit

module ``Given an XML node representing a Visual Studio project property group`` =

    let name (data : Configuration.Data) = data.Name
    let platform (data : Configuration.Data) = data.Platform
    let path (data : Configuration.Data) = data.OutputPath 

    let read (name : string) = 
        Embedded.xml "configuration-data.xml"
        |> toContext
        |> selectSingle (String.Format ("//examples/example[@name='{0}']/PropertyGroup", name))
        |> Configuration.read

    module ``And the configuration name and platform are present in the Condition attribute`` =

        type ``When configuration data is read`` () = 

            let config = 
                read "configuration-and-platform"
        
            [<Fact>] member test.
                ``Then the configuration name is correctly set`` () =
                    config |> name |> should equal "Debug"

            [<Fact>] member test.
                ``Then the platform is correctly set`` () = 
                    config |> platform |> should equal "AnyCPU"

            [<Fact>] member test.
                ``Then the output path is correctly set`` () = 
                    config |> path |> should equal @"bin\Debug\"

    module ``And only the configuration name is present in the Condition attribute`` = 

        type ``When the configuration data is read`` () =

            let config = 
                read "configuration-only"

            [<Fact>] member test. 
                ``Then the configuration name is correctly set`` () =
                    config |> name |> should equal "Debug"

            [<Fact>] member test.
                ``Then platform is correctly set`` () = 
                    config |> platform |> should equal "AnyCPU"

            [<Fact>] member test.
                ``Then the output path is correctly set`` () = 
                    config |> path |> should equal @"bin\Debug\"

    module ``And only the platform is present in the Condition attribute`` =

        type ``When the configuration data is read`` () =

            let config = 
                read "platform-only"

            [<Fact>] member test. 
                ``Then the configuration name is correctly set`` () =
                    config |> name |> should equal "Debug"

            [<Fact>] member test.
                ``Then platform is correctly set`` () = 
                    config |> platform |> should equal "AnyCPU"

            [<Fact>] member test.
                ``Then the output path is correctly set`` () = 
                    config |> path |> should equal @"bin\Debug\"
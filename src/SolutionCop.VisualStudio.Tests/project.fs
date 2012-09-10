namespace SolutionCop.VisualStudio.Tests

open System
open System.Xml
open SolutionCop.Common.Xml
open SolutionCop.Common
open SolutionCop.VisualStudio
open FsUnit.Xunit
open Xunit

module ``Given an XML node representing a Visual Studio project`` =

    let directory (data : Project.Data) = data.Directory
    let filename (data : Project.Data) = data.FileName
    let configurations (data : Project.Data) = data.Configurations
    let references (data : Project.Data) = data.ReferencePaths

    let read (name : string) = 
        Embedded.xml "project-data.xml"
        |> toContext
        |> Schema.register
        |> selectSingle (String.Format ("//examples/example[@name='{0}']/{1}", name, (Schema.prefix "Project")))
        |> (Project.read "C:\MySolution")

    type ``When the project is read`` () =

        [<Fact>] member test.
            ``Then the directory is set correctly`` () = 
                read "library" |> directory |> should equal @"C:\MySolution"

        [<Fact>] member test.
            ``Then the configurations are parsed correctly`` () = 
                read "library" |> configurations |> List.length |> should equal 2

        [<Fact>] member test.
            ``Then the reference paths are parsed correctly`` () =
                read "exe" |> references |> List.same [ "..\Refs\MyOtherProject.dll"; ] |> should be True

    module ``And the project is a console application`` = 

        type ``When the project is read`` () =

            [<Fact>] member test.
                ``Then the filename is parsed correctly`` () =
                    read "exe" |> filename |> should equal "MySolution.MyProject.exe"

    module ``And the project is a Windows application`` = 

        type ``When the project is read`` () =

            [<Fact>] member test.
                ``Then the filename is parsed correctly`` () =
                    read "win-exe" |> filename |> should equal "MySolution.MyProject.exe"

    module ``And the project is a class library`` = 

        type ``When the project is read`` () =

            [<Fact>] member test.
                ``Then the filename is parsed correctly`` () =
                    read "library" |> filename |> should equal "MySolution.MyProject.dll"


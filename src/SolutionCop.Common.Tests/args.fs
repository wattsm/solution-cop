﻿namespace SolutionCop.Common.Tests

open System
open SolutionCop.Common
open SolutionCop.Common.Args
open FsUnit.Xunit
open Xunit

module ``Given a string of command line arguments`` = 

    let args = [| "-sln:C:\MySolution\MySolution.sln"; "-configuration:Debug"; "-platform:AnyCPU"; "-include:^Start"; "-exclude:End$"; "-name:New.fxcop"; "-based-on:C:\MySolution\Old.fxcop"; |]
    let targetSettings = { Configuration = "Debug"; Platform = "AnyCPU"; Include = [ "^Start"; ]; Exclude = [ "End$"; ]; }
    let outputSettings = { Directory = @"C:\MySolution"; FileName = "New.fxcop"; BasedOn = @"C:\MySolution\Old.fxcop"; }

    let argsWithout index = 
        args
        |> Array.mapi (fun i arg -> if i = index then None else Some arg)
        |> Array.choose id

    let replaceArg index arg = 
        args
        |> Array.mapi (fun i arg' -> if i = index then arg else arg')

    module ``And the solution argument is missing`` =
        
        type ``When getFileName is called`` () = 

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown`` () =
                    (fun () -> argsWithout 0 |> getFileName |> ignore) |> should throw typedefof<MissingSettingException>

    module ``And the configuration argument is missing`` =

        type ``When getTargetSettings is called`` () = 

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown`` () = 
                    (fun () -> argsWithout 1 |> getTargetSettings |> ignore) |> should throw typedefof<MissingSettingException>

    module ``And the platform argument is missing`` =

        type ``When getTargetSettings is called`` () = 

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown`` () =
                    (fun () -> argsWithout 2 |> getTargetSettings |> ignore) |> should throw typedefof<MissingSettingException>

    module ``And all arguments are supplied`` =

        type ``When getFileName is called`` () = 
            
            [<Fact>] member test.
                ``Then the correct value is returned`` () =
                    args |> getFileName |> should equal @"C:\MySolution\MySolution.sln"

            [<Fact>] member test.
                ``Then double quotation marks are parsed correctly`` () = 
                    replaceArg 0 "-sln:\"C:\MySolution\MySolution.sln\"" |> getFileName |> should equal @"C:\MySolution\MySolution.sln"

            [<Fact>] member test.
                ``The single quotation marks are parsed correctly`` () = 
                    replaceArg 0 "-sln:'C:\MySolution\MySolution.sln'" |> getFileName |> should equal @"C:\MySolution\MySolution.sln"

        type ``When getTargetSettings is called`` () = 
            
            [<Fact>] member test.
                ``Then the correct value is returned`` () = 
                    args |> getTargetSettings |> should equal targetSettings

            [<Fact>] member test.
                ``Then double quotation marks on includes are parsed correctly`` () = 
                    replaceArg 3 "-include:\"^Start\"" |> getTargetSettings |> should equal targetSettings

            [<Fact>] member test.
                ``The single quotation marks on includes are parsed correctly`` () = 
                    replaceArg 3 "-include:'^Start'" |> getTargetSettings |> should equal targetSettings

            [<Fact>] member test.
                ``Then double quotation marks on excludes are parsed correctly`` () = 
                    replaceArg 4 "-exclude:\"End$\"" |> getTargetSettings |> should equal targetSettings

            [<Fact>] member test.
                ``The single quotation marks on excludes are parsed correctly`` () = 
                    replaceArg 4 "-exclude:'End$'" |> getTargetSettings |> should equal targetSettings

        type ``When getOutputSettings is called`` () =

            [<Fact>] member test.
                ``Then the correct value is returned`` () = 
                    args |> getOutputSettings |> should equal outputSettings

            [<Fact>] member test.
                ``Then the solution file name is used if no name is specified`` () =
                    argsWithout 5 |> getOutputSettings |> should equal { outputSettings with FileName = "MySolution.fxcop"; }

            [<Fact>] member test.
                ``Then single quotation marks on based-on are parsed correctly`` () = 
                    replaceArg 6 "-based-on:'C:\MySolution\Old.fxcop'" |> getOutputSettings |> should equal outputSettings

            [<Fact>] member test.
                ``Then double quotation marks on based-on are parsed correctly`` () =
                    replaceArg 6 "-based-on:\"C:\MySolution\Old.fxcop\"" |> getOutputSettings |> should equal outputSettings
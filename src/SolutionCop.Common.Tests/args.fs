namespace SolutionCop.Common.Tests

open System
open SolutionCop.Common
open FsUnit.Xunit
open Xunit

module ``Given a string of command line arguments`` = 

    let args = [| "-sln:'C:\MySolution\MySolution.sln'"; "-configuration:Debug"; "-platform:AnyCPU"; |]

    module ``And one or more more mandatory settings are not included`` =

        type ``When the argument list is parsed`` () = 

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown if the path is not included`` () =
                    let
                        args' = args.[1..2]
                    in
                        (fun () -> Args.parse args' |> ignore) |> should throw typedefof<Args.MissingSettingException>

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown if the configuration is not included`` () = 
                    let 
                        args' = [| args.[0]; args.[2] |]
                    in
                        (fun () -> Args.parse args' |> ignore) |> should throw typedefof<Args.MissingSettingException>

            [<Fact>] member test.
                ``Then a MissingSettingException is thrown if the platform is not included`` () = 
                    let
                        args' = args.[0..1]
                    in
                        (fun () -> Args.parse args' |> ignore) |> should throw typedefof<Args.MissingSettingException>


    module ``And all mandatory settings are included`` = 

        type ``When the argument list is parsed`` () =            

            let path (settings : Args.Settings) = settings.Path
            let configuration (settings : Args.Settings) = settings.Configuration
            let platform (settings : Args.Settings) = settings.Platform            

            [<Fact>] member test.
                ``Then the path is parsed correctly (single quotes)`` () = 
                    Args.parse args |> path |> should equal "C:\MySolution\MySolution.sln"

            [<Fact>] member test.
                ``Then the path is parsed correctly (double quotes)`` () =
                    let
                        args' = 
                            [| "-sln:\"C:\MySolution\MySolution.sln\""; args.[1]; args.[2]; |]
                    in
                        Args.parse args |> path |> should equal "C:\MySolution\MySolution.sln"

            [<Fact>] member test.
                ``Then the path is parsed correctly (no quotes)`` () = 
                    let 
                        args' =
                            [| "-sln:C:\MySolution\MySolution.sln"; args.[1]; args.[2]; |]
                    in
                        Args.parse args |> path |> should equal "C:\MySolution\MySolution.sln"

            [<Fact>] member test.
                ``Then the configuration is parsed correctly`` () =
                    Args.parse args |> configuration |> should equal "Debug"

            [<Fact>] member test.
                ``Then the platform is parsed correctly`` () =
                    Args.parse args |> platform |> should equal "AnyCPU"        
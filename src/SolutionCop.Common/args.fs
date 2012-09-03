namespace SolutionCop.Common

open System

[<RequireQualifiedAccess>]
module Args = 

    type Settings = {
        Path : String;
        Configuration : String;
        Platform : String;        
    }

    let parse (args : string []) = 
        { Path = ""; Configuration = ""; Platform = ""; }
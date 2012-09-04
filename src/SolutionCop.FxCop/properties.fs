namespace SolutionCop.FxCop

open System

[<RequireQualifiedAccess>]
module Properties = 

    type ProjectProperties = {
        Name : String;
    }

    let parse (args : string []) = 
        { Name = ""; }
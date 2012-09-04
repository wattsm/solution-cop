namespace SolutionCop.FxCop

open System
open SolutionCop.VisualStudio

[<RequireQualifiedAccess>]
module Filters = 

    type Settings = {
        Includes : String list;
        Excludes : String list;
    }  

    let parse (args : string []) = 
        { Includes = []; Excludes = []; }

    let apply settings solution = 
        solution
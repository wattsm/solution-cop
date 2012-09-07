namespace SolutionCop.FxCop

open System

[<RequireQualifiedAccess>]
module Targets =

    type Data = {
        Binaries : String list;
        ReferencePaths : String list;
    }

    let extract filters solution = 
        { Binaries = []; ReferencePaths = []; }
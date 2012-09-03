namespace SolutionCop.Common

open System
open System.Collections

[<AutoOpen>]
module List = 

    let contains value list = 
        list
        |> List.exists ((=) value) 

    let same list1 list2 = 
        if (List.length list1) <> (List.length list2) then 
            false
        else
            list1
            |> List.filter (fun item -> not (contains item list2))
            |> List.length
            |> ((=) 0)

    let ofEnumerable<'a> (enumerable : IEnumerable) =
        
        let rec collect (enumerator : IEnumerator) = 
            match (enumerator.MoveNext ()) with
            | false -> []
            | _ -> 

                (enumerator.Current :?> 'a) :: (collect enumerator)

        enumerable.GetEnumerator ()
        |> collect
        
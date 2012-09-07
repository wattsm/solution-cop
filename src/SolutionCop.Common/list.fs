namespace SolutionCop.Common

open System
open System.Collections

[<AutoOpen>]
module List = 

    ///True if a list contains a given value
    let contains value list = 
        list
        |> List.exists ((=) value) 

    ///True if two lists contain the same values
    let same list1 list2 = 
        if (List.length list1) <> (List.length list2) then 
            false
        else
            list1
            |> List.filter (fun item -> not (contains item list2))
            |> List.length
            |> ((=) 0)

    ///Converts a vanilla IEnumerable to a list of a given type
    let ofEnumerable<'a> (enumerable : IEnumerable) =
        
        let rec collect (enumerator : IEnumerator) = 
            match (enumerator.MoveNext ()) with
            | false -> []
            | _ -> 

                (enumerator.Current :?> 'a) :: (collect enumerator)

        enumerable.GetEnumerator ()
        |> collect
        
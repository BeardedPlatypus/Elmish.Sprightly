namespace Sprightly.Domain

module public Sprite =
    type public Id = | Id of string * uint

    type public Data = 
        { Name : string
        }

    type public T =
        { Id : Id 
          Data : Data
        }


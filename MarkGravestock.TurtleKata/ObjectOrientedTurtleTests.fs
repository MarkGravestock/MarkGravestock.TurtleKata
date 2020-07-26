module ObjectOrientedTurtle

open Xunit
open System
open System.IO

type PenColour = Red = 0 | Black = 1 | Blue = 2

type Turtle() =
    
    let mutable lines = List.empty
    
    let mutable x = 0.0
    let mutable y = 0.0
    
    let mutable penColour = PenColour.Black
    
    let mutable directionInDegrees = 0.0
    
    let mutable isPenDown = true
    
    member this.Move distance =
        let startX = x
        let startY = y
        let radians = (Math.PI / 180.0) * directionInDegrees
        x <- x + (distance * Math.Cos radians)
        y <- y + (distance * Math.Sin radians)
        
        if isPenDown then
            let rgb =
                match penColour with
                    | PenColour.Black -> "(0,0,0)"
                    | PenColour.Red -> "(255,0,0)"
                    | PenColour.Blue -> "(0,0,255)"
                    | _ -> failwith "Invalid Colour"
                    
            let svg = sprintf "<line x1='%f' y1='%f' x2='%f' y2='%f' style='stroke:rgb%s;stroke-width:2' />" startX startY x y rgb
            let newLine = [svg]
            lines <- List.append lines newLine

    member this.TurnClockwise degreesToTurn =
        directionInDegrees <- (directionInDegrees + degreesToTurn) % 360.0

    member this.TurnAntiClockwise degreesToTurn =
        this.TurnClockwise -degreesToTurn
            
    member this.IsAtLocation(expectedX, expectedY) =
        Math.Abs(x - expectedX) < 0.00001 && Math.Abs(y - expectedY) < 0.00001
    
    member this.PenUp =
        isPenDown <- false
        
    member this.PenDown =
        isPenDown <- true
        
    member this.DrewLine =
        isPenDown
    
    member this.SetPenColour colour =
        penColour <- colour
        
    member this.HasPenColour colour =
        penColour = colour
    
    member this.ToHtmlFile name =
        let lineText = lines |> List.fold (+) ""
        let fileContents = "<html><body><svg height='210' width='500'>" + lineText + "</svg></body></html>"
        File.WriteAllText (name, fileContents) 
        
[<Fact>]
let ``It can move some distance in the current direction`` () =
    
    let turtle = Turtle()
    
    turtle.Move 100.0
    
    Assert.True(turtle.IsAtLocation(100.0, 0.0))

[<Fact>]
let ``It can Turn a certain number of degrees clockwise`` () =
    
    let turtle = Turtle()
    
    turtle.TurnClockwise 90.0

    turtle.Move 100.0
    
    Assert.True(turtle.IsAtLocation(0.0, 100.0))

[<Fact>]
let ``It can Turn a certain number of degrees anticlockwise`` () =
    
    let turtle = Turtle()
    
    turtle.TurnAntiClockwise 90.0

    turtle.Move 100.0
    
    Assert.True(turtle.IsAtLocation(0.0, -100.0))

[<Fact>]
let ``It can Put the pen up and draws no line`` () =
    
    let turtle = Turtle()
    
    turtle.PenUp
    turtle.Move 100.0
    
    Assert.False(turtle.DrewLine)

[<Fact>]
let ``It can Put the pen down and draws a line`` () =
    
    let turtle = Turtle()
    
    turtle.PenDown
    turtle.Move 100.0
    
    Assert.True(turtle.DrewLine)

[<Fact>]
let ``It can draw line in Red`` () =
    
    let turtle = Turtle()
    
    turtle.SetPenColour PenColour.Red
    
    Assert.True(turtle.HasPenColour PenColour.Red)

[<Fact>]
let ``It can draw lines in SVG`` () =
    
    let turtle = Turtle()
    
    turtle.Move 100.0
    turtle.SetPenColour PenColour.Red
    turtle.TurnClockwise 90.0
    turtle.Move 100.0
    turtle.PenUp
    turtle.TurnClockwise 90.0
    turtle.Move 100.0
    turtle.PenDown
    turtle.SetPenColour PenColour.Blue
    turtle.TurnClockwise 90.0
    turtle.Move 100.0    
    
    turtle.ToHtmlFile ".\Test.html"

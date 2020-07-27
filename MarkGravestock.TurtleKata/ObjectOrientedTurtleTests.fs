module ObjectOrientedTurtle

open Xunit
open System
open System.IO

type PenColour = Red = 0 | Black = 1 | Blue = 2

[<Measure>]
type Degrees

type Position = double

type Distance = double

type Line = {
    FromX: Position
    FromY: Position
    ToX: Position
    ToY: Position
    PenColour: PenColour
    }    

type LineWriter =
    abstract member WriteTo: List<Line> -> Unit

type HtmlSvgWriter(fileName) =
    
    interface LineWriter with
        member this.WriteTo list =
            
            let lineToSvg line = 
                let rgb =
                    match line.PenColour with
                        | PenColour.Black -> "(0,0,0)"
                        | PenColour.Red -> "(255,0,0)"
                        | PenColour.Blue -> "(0,0,255)"
                        | _ -> failwith "Invalid Colour"
                        
                let svg = sprintf "<line x1='%f' y1='%f' x2='%f' y2='%f' style='stroke:rgb%s;stroke-width:2' />" line.FromX line.FromY line.ToX line.ToY rgb
                svg

            let lineText = list |> List.map lineToSvg |> List.fold (+) ""
            let fileContents = "<html><body><svg height='210' width='500'>" + lineText + "</svg></body></html>"
            File.WriteAllText (fileName, fileContents) 


type Turtle() =
    
    let mutable lines = List.empty
    
    let mutable x = 0.0
    let mutable y = 0.0
    
    let mutable penColour = PenColour.Black
    
    let mutable angle = 0.0<Degrees>
    
    let mutable isPenDown = true
    
    member this.Move (distance:Distance) =
        let startX = x
        let startY = y
        let radians = (Math.PI / 180.0<Degrees>) * angle
        x <- x + (distance * Math.Cos radians)
        y <- y + (distance * Math.Sin radians)
        
        if isPenDown then
            let line = { FromX = startX; FromY = startY; ToX = x; ToY = y; PenColour = penColour }
            lines <- List.append lines [line]

    member this.TurnClockwise degreesToTurn =
        angle <- (angle + degreesToTurn) % 360.0<Degrees>

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
    
    member this.Write (writer:LineWriter) =
        writer.WriteTo lines
        
[<Fact>]
let ``It can move some distance in the current direction`` () =
    
    let turtle = Turtle()
    
    turtle.Move 100.0
    
    Assert.True(turtle.IsAtLocation(100.0, 0.0))

[<Fact>]
let ``It can Turn a certain number of degrees clockwise`` () =
    
    let turtle = Turtle()
    
    turtle.TurnClockwise 90.0<Degrees>

    turtle.Move 100.0
    
    Assert.True(turtle.IsAtLocation(0.0, 100.0))

[<Fact>]
let ``It can Turn a certain number of degrees anticlockwise`` () =
    
    let turtle = Turtle()
    
    turtle.TurnAntiClockwise 90.0<Degrees>

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
    turtle.TurnClockwise 90.0<Degrees>
    turtle.Move 100.0
    turtle.PenUp
    turtle.TurnClockwise 90.0<Degrees>
    turtle.Move 100.0
    turtle.PenDown
    turtle.SetPenColour PenColour.Blue
    turtle.TurnClockwise 90.0<Degrees>
    turtle.Move 100.0    
    
    turtle.Write (HtmlSvgWriter(".\Test.html"))


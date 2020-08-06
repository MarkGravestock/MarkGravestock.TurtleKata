module TurtleKata

open System.IO

module Turtle =

    open System

    type PenColour = Red | Black| Blue

    type PenState = Up | Down

    [<Measure>]
    type Degrees

    [<Measure>]
    type Radians

    type Angle = double

    let convertToRadiansFromDegree angle = (Math.PI / 180.0<Degrees>) * angle

    type Distance = double

    type Position = {
        x: Distance
        y: Distance
    }

    type Line = {
        From: Position
        To: Position
        PenColour: PenColour
        PenState: PenState
        }    

module ObjectOrientedTurtle =
    open Turtle
    open System
    open System.IO

    let writeHtmlSvg list fileName =
        let lineToSvg line = 
            let rgb =
                match line.PenColour with
                    | PenColour.Black -> "(0,0,0)"
                    | PenColour.Red -> "(255,0,0)"
                    | PenColour.Blue -> "(0,0,255)"
                    
            let svg = sprintf "<line x1='%f' y1='%f' x2='%f' y2='%f' style='stroke:rgb%s;stroke-width:2' />" line.From.x line.From.y line.To.x line.To.y rgb
            svg
            
        let drawLine line = line.PenState = PenState.Down    

        let lineText = list |> List.filter drawLine |> List.map lineToSvg |> List.fold (+) ""
        let fileContents = "<html><body><svg height='210' width='500'>" + lineText + "</svg></body></html>"
        File.WriteAllText (fileName, fileContents) 
        
        
    type LineWriter =
        abstract member WriteTo: List<Line> -> Unit

    type HtmlSvgWriter(fileName) =
        
        interface LineWriter with
            member this.WriteTo list =
                
                writeHtmlSvg list fileName

    type Turtle() =
        
        let mutable lines = List.empty
        
        let mutable x = 0.0
        let mutable y = 0.0
        
        let mutable penColour = PenColour.Black
        
        let mutable angle = 0.0<Degrees>
        
        let mutable penState = PenState.Down
        
        member this.Move (distance:Distance) =
            let startX = x
            let startY = y
            let radians = convertToRadiansFromDegree angle
            x <- x + (distance * Math.Cos radians)
            y <- y + (distance * Math.Sin radians)
            
            let line = { From = {x = startX; y = startY}; To = {x = x; y = y} ; PenColour = penColour; PenState = penState }
            lines <- List.append lines [line]

        member this.TurnClockwise degreesToTurn =
            angle <- (angle + degreesToTurn) % 360.0<Degrees>

        member this.TurnAntiClockwise degreesToTurn =
            this.TurnClockwise -degreesToTurn
                
        member this.IsAtLocation(expectedX, expectedY) =
            Math.Abs(x - expectedX) < 0.00001 && Math.Abs(y - expectedY) < 0.00001
        
        member this.PenUp =
            penState <- PenState.Up
            
        member this.PenDown =
            penState <- PenState.Down
            
        member this.DrewLine =
            penState = PenState.Down
        
        member this.SetPenColour colour =
            penColour <- colour
            
        member this.HasPenColour colour =
            penColour = colour
        
        member this.Write (writer:LineWriter) =
            writer.WriteTo lines

module ObjectOrientedTurtleTests =

    open Xunit
    open Turtle
    open ObjectOrientedTurtle
    
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
        
        let fileName = ".\Test.html"
        
        turtle.Write (HtmlSvgWriter(fileName))

        Assert.True(File.Exists(fileName))
        
    let drawPolygon (turtle: Turtle) sides =

        let turnAngle = 360.0<Degrees> / float sides
        
        for i in [1..sides] do
            turtle.Move 100.0
            turtle.TurnClockwise turnAngle
        

    [<Fact>]
    let ``It can draw a heptagon in SVG`` () =
        
        let turtle = Turtle()
        
        drawPolygon turtle 7
        
        let fileName = ".\Heptagon.html"

        turtle.Write (HtmlSvgWriter(fileName))

        Assert.True(File.Exists(fileName))

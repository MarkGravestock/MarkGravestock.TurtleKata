module FunctionalTurtle

open TurtleKata
open TurtleKata.Turtle
open System
open System.IO
open ObjectOrientedTurtle

type Move =
    { angle: float<Degrees>
      distance: Distance }
            
let moveTurtle startPosition penState penColour angle distance =
    let radians = convertToRadiansFromDegree angle
    { From = startPosition
      To =
          { x = startPosition.x + (distance * Math.Cos radians)
            y = startPosition.y + (distance * Math.Sin radians) }
      PenColour = penColour
      PenState = penState }

let moveTurtleFromEndOfLine (line: Line) penState penColour angle distance =
    moveTurtle line.To penState penColour angle distance

let addTurtleMove (lines: Line list) penState penColour angle distance =
    let lastLine = List.last lines
    let nextLine = moveTurtleFromEndOfLine lastLine penState penColour angle distance
    List.append lines [ nextLine ]

let isAtPosition actual expected =
    Math.Abs(actual.x - expected.x) < 0.00001 && Math.Abs(actual.y - expected.y) < 0.00001

type State = {
        Position: Position
        PenColour: PenColour
        PenState: PenState
    }

let move (move:Move) (currentState:State) =
    let radians = convertToRadiansFromDegree move.angle
        
    let newPosition : Position = {
            x = currentState.Position.x + (move.distance * Math.Cos radians)
            y = currentState.Position.y + (move.distance * Math.Sin radians) }
                                                          
    let newState = {
        Position = newPosition 
        PenColour = currentState.PenColour
        PenState = currentState.PenState
    }
    
    newState

let penUp currentState =
    
    let newState = {
        Position = currentState.Position 
        PenColour = currentState.PenColour
        PenState = PenState.Up
    }
    
    newState
    
    
module TurtleTests =
   
    open Xunit

    [<Fact>]
    let ``It can create a line which represents a move of some distance in a given direction`` () =

        let startPosition =
            { x = 0.0
              y = 0.0 }

        let distance = 100.0
        let angle = 0.0<Degrees>
        let penState = PenState.Down
        let penColour = PenColour.Black

        let line = moveTurtle startPosition penState penColour angle distance

        Assert.True
            (isAtPosition line.To
                 { x = 100.0
                   y = 0.0 })
        Assert.True
            (isAtPosition line.From
                 { x = 0.0
                   y = 0.0 })
        Assert.Equal(line.PenState, PenState.Down)
        Assert.Equal(line.PenColour, PenColour.Black)

    [<Fact>]
    let ``It can move from the last position to another position`` () =

        let startPosition =
            { x = 0.0
              y = 0.0 }

        let angle = 0.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Black

        let line = moveTurtle startPosition penState penColour angle distance
        let lines = [ line ]

        let angle = 90.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Blue

        let lines = addTurtleMove lines penState penColour angle distance

        let finalLine = List.last lines

        Assert.True
            (isAtPosition finalLine.To
                 { x = 100.0
                   y = 100.0 })

    [<Fact>]
    let ``It can render the lines as svg`` () =

        let startPosition =
            { x = 0.0
              y = 0.0 }

        let angle = 0.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Black

        let line = moveTurtle startPosition penState penColour angle distance
        let lines = [ line ]

        let angle = 90.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Blue

        let lines = addTurtleMove lines penState penColour angle distance
        
        let fileName = ".\FunctionalTest.html"

        writeHtmlSvg lines fileName 
        
        Assert.True(File.Exists(fileName))
         
    [<Fact>]
    let ``It can move to another position`` () =

        let initialState = {
            Position = {
                x = 0.0
                y = 0.0
            }
            PenColour = PenColour.Black
            PenState = PenState.Down
        }

        let moveOneHundredAtZeroDegrees = {
            angle = 0.0<Degrees>
            distance = 100.0
        }
        
        let finalState = initialState |> move moveOneHundredAtZeroDegrees |> move moveOneHundredAtZeroDegrees 
        
        Assert.True (isAtPosition finalState.Position {
           x = 200.0
           y = 0.0
         })
        
    [<Fact>]
    let ``It can put the pen up`` () =

        let initialState = {
            Position = {
                x = 0.0
                y = 0.0
            }
            PenColour = PenColour.Black
            PenState = PenState.Down
        }
        
        let finalState = initialState |> penUp 
        
        Assert.Equal (finalState.PenState, PenState.Up)
        
        
        


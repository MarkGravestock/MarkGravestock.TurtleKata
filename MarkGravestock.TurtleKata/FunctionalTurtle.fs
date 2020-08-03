module FunctionalTurtle

open TurtleKata.Turtle
open System

type Move = {
    angle: float<Degrees>
    distance: Distance
}

let moveTurtle startPosition penState penColour angle distance =
    let radians = convertToRadiansFromDegree angle
    { From = startPosition; To = {x = startPosition.x + (distance * Math.Cos radians); y = startPosition.y + (distance * Math.Sin radians)} ; PenColour = penColour; PenState = penState }

let moveTurtleFromEndOfLine (line: Line) penState penColour angle distance =
    moveTurtle line.To penState penColour angle distance

let addTurtleMove (lines: Line list) penState penColour angle distance =    
    let lastLine = List.last lines
    let nextLine = moveTurtleFromEndOfLine lastLine penState penColour angle distance
    List.append lines [nextLine]
    
let isAtPosition actual expected =
    Math.Abs(actual.x - expected.x) < 0.00001 && Math.Abs(actual.y - expected.y) < 0.00001
    
module TurtleTests =

    open Xunit
    
    [<Fact>]
    let ``It can create a line which represents a move of some distance in a given direction`` () =
        
        let startPosition = { x = 0.0; y = 0.0 }
        let distance = 100.0
        let angle = 0.0<Degrees>
        let penState = PenState.Down
        let penColour = PenColour.Black
        
        let line = moveTurtle startPosition penState penColour angle distance 
        
        Assert.True(isAtPosition line.To {x = 100.0; y = 0.0})
        Assert.True(isAtPosition line.From {x = 0.0; y = 0.0})
        Assert.Equal(line.PenState, PenState.Down)
        Assert.Equal(line.PenColour, PenColour.Black)        
        
    [<Fact>]
       let ``It can move from the last position to another position`` () =
        
        let startPosition = { x = 0.0; y = 0.0 }
        let angle = 0.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Black
        
        let line = moveTurtle startPosition penState penColour angle distance    
        let lines = [line]
        
        let angle = 90.0<Degrees>
        let distance = 100.0
        let penState = PenState.Down
        let penColour = PenColour.Blue
                
        let lines = addTurtleMove lines penState penColour angle distance

        let finalLine = List.last lines
        
        Assert.True(isAtPosition finalLine.To {x = 100.0; y = 100.0})
        
        
        
        
        






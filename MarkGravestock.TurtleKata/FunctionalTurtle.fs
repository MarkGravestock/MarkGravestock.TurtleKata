module FunctionalTurtle

open TurtleKata.Turtle
open System

let moveTurtle startPosition penState penColour angle distance =
    let radians = convertToRadiansFromDegree angle
    { From = startPosition; To = {x = startPosition.x + (distance * Math.Cos radians); y = startPosition.y + (distance * Math.Sin radians)} ; PenColour = penColour; PenState = penState }
    
let isAtPosition actual expected =
    Math.Abs(actual.x - expected.x) < 0.00001 && Math.Abs(actual.y - expected.y) < 0.00001
    
module TurtleTests =

    open Xunit
    
    [<Fact>]
    let ``It can move some distance in a given direction`` () =
        
        let startPosition = { x = 0.0; y = 0.0 }
        let angle = 0.0<Degrees>
        let penState = PenState.Down
        let penColour = PenColour.Black
        let distance = 100.0
        
        let line = moveTurtle startPosition penState penColour angle distance 
        
        Assert.True(isAtPosition line.To {x = 100.0; y = 0.0})






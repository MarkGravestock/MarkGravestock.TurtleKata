#### Kata based on [13 Ways of Looking at a Turle](https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/)

Will try OO and FP versions.
May try and output SVG

##### Requirements for a Turtle

A turtle supports four instructions:
    
- Move some distance in the current direction.
- Turn a certain number of degrees clockwise or anticlockwise.
- Put the pen down or up. When the pen is down, moving the turtle draws a line.
- Set the pen color (one of black, blue or red).

##### Notes

My initial functional approach is different from the example, in some ways this is down to how I've interpreted/modified the requirements to output HTML/SVG but I think I have the same
core approach of a function which takes the current state (a list of lines) and returns a new state (list with a new item added). The [example](https://swlaschin.gitbooks.io/fsharpforfunandprofit/content/posts/13-ways-of-looking-at-a-turtle.html)
has the client tracking the state and the functions simply return a new state.
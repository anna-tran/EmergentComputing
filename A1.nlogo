breed [wasps wasp]

globals [vespa-ruleset vespula-ruleset parachartergus-ruleset pieces-ruleset lineup-ruleset]

; setup simulation before running it
to setup
  clear-all
  setup-patches
  setup-wasps
  setup-rulesets
end

; set up patch distribution
to setup-patches
  ; set default patches to empty (black)
  ask patches [set pcolor black]
  ; material A is yellow
  ; material B is lime
  ; material C is violet
  ask n-of density-material-a patches [set pcolor yellow]
  ask n-of density-material-b patches [set pcolor lime]
  ask n-of density-material-c patches [set pcolor violet]
end

; set up wasp creation
to setup-wasps
  set-default-shape wasps "butterfly"
  create-wasps number-of-wasps
  ask wasps [setxy random-xcor random-ycor]
end

; set all global rulesets
to setup-rulesets
  set vespa-ruleset
  (list
    (list (list 1 0 0 0 0 0 0 0) 2)
    (list (list 1 2 0 0 0 0 0 0) 2)
    (list (list 1 0 0 0 0 0 0 2) 2)
    (list (list 2 0 0 0 0 0 2 1) 2)
    (list (list 0 0 0 0 2 1 2 0) 2)
    (list (list 2 0 0 0 0 0 1 2) 2)
    (list (list 0 0 0 0 2 2 1 0) 2)
    (list (list 2 0 0 0 0 0 2 1) 2)
    (list (list 1 2 0 0 0 0 0 2) 2)
    (list (list 2 2 0 0 0 0 0 2) 2)
    (list (list 2 2 0 0 0 2 2 2) 2)
    (list (list 2 0 0 0 0 0 2 2) 2)
    (list (list 2 2 2 0 0 0 2 2) 2)
    (list (list 1 2 2 0 0 0 2 2) 2)
    (list (list 2 2 2 2 0 2 2 2) 2)
    (list (list 2 0 0 0 0 2 2 1) 2)
    (list (list 2 2 0 0 0 0 2 1) 2)
    (list (list 2 2 0 0 0 2 2 1) 2))

  set vespula-ruleset
  (list
    (list (list 2 0 0 0 0 0 0 0) 2)
    (list (list 2 2 0 0 0 0 0 0) 2)
    (list (list 2 0 0 0 0 0 0 2) 2)
    (list (list 2 0 0 0 0 0 2 2) 2)
    (list (list 2 2 2 0 0 0 0 0) 2)
    (list (list 2 2 0 0 0 0 0 2) 2)
    (list (list 2 2 0 0 0 0 2 2) 2)
    (list (list 2 2 2 0 0 0 2 2) 2))

  set parachartergus-ruleset
  (list
    (list (list 0 0 0 0 0 0 1 0) 2)
    (list (list 2 0 0 0 0 0 0 1) 2)
    (list (list 0 0 0 0 2 1 0 0) 2)
    (list (list 0 0 0 0 0 2 2 2) 2)
    (list (list 0 0 0 0 2 2 2 0) 2)
    (list (list 2 0 0 0 0 0 2 2) 2)
    (list (list 2 2 0 0 0 0 0 2) 2)
    (list (list 0 0 0 2 2 2 0 0) 2)
    (list (list 2 0 0 0 0 0 0 1) 2)
    (list (list 2 0 0 0 0 2 2 2) 2)
    (list (list 2 2 2 2 0 0 0 0) 2)
    (list (list 2 2 2 0 0 0 2 2) 2)
    (list (list 2 0 0 0 2 2 2 2) 2)
    (list (list 0 0 2 2 2 2 2 0) 2)
    (list (list 2 2 2 0 0 0 0 0) 2)
    (list (list 0 0 2 2 2 0 0 0) 2)
    (list (list 0 0 0 2 2 2 2 2) 2))

  set pieces-ruleset
  (list
    (list (list 0 0 0 3 0 0 1 0) 3)
    (list (list 2 1 0 0 0 0 0 1) 3)
    (list (list 0 0 0 0 3 1 0 0) 3)
    (list (list 0 0 0 0 0 1 0 1) 3)
    (list (list 0 0 0 0 2 2 3 0) 2)
    (list (list 2 0 0 3 1 0 0 2) 3)
    (list (list 0 2 0 3 0 1 0 2) 2)
    (list (list 2 0 3 0 0 0 0 0) 3)
    (list (list 2 0 0 3 0 0 3 1) 2)
    (list (list 2 0 0 0 1 0 3 0) 3)
    (list (list 0 0 0 0 0 0 0 0) 2)
    (list (list 2 0 2 0 0 0 2 3) 3)
    (list (list 1 0 0 0 3 2 0 2) 2)
    (list (list 0 0 2 0 2 0 2 0) 2)
    (list (list 2 0 0 0 0 0 0 0) 3)
    (list (list 0 0 2 1 2 0 0 0) 2)
    (list (list 0 0 0 2 2 0 0 2) 2))

  set lineup-ruleset
  (list
    (list (list 3 1 0 2 0 0 0 0) 3)
    (list (list 0 0 3 0 3 0 2 0) 3)
    (list (list 0 0 3 2 0 2 0 0) 3)
    (list (list 0 0 0 2 3 0 3 0) 2)
    (list (list 0 0 0 0 3 2 0 2) 2)
    (list (list 2 0 0 0 0 3 2 0) 3)
    (list (list 0 2 0 0 0 0 2 0) 2)
    (list (list 2 0 0 0 0 0 0 3) 2)
    (list (list 3 0 0 0 3 0 0 0) 3)
    (list (list 2 0 0 0 0 0 0 0) 3)
    (list (list 3 0 0 0 0 0 0 0) 2))
end

;; this reporter converts an asymmetric rule set into a symmetric ruleset
to-report convert-ruleset-to-symmetric [asymruleset]
  let symruleset (list)
  foreach asymruleset [ rule ->
    let asymrule first rule
    let result last rule
    ; add the asymmetric rule
    set symruleset lput rule symruleset
    ;add the 3 other symmetric rules
    foreach [2 4 6] [ num ->
      set symruleset lput (list (sentence (sublist asymrule num 8) (sublist
        asymrule 0 num)) result) symruleset
    ]
  ]
  report symruleset
end

; start the simulation
to go
  ; materials in order of empty, material A, material B and material C
  let materials (list black yellow lime violet)

  ; ruleset to use
  let associated-ruleset (get-ruleset ruleset)
  let aRuleset (convert-ruleset-to-symmetric associated-ruleset)

  ; move the wasps around and try to build materials based on the ruleset
  move-wasps
  build-nest aRuleset materials
end

; get defined rulesets associated with user's choice of rulset
to-report get-ruleset [ruleset-name]
  if ruleset-name = "vespa" [report vespa-ruleset]
  if ruleset-name = "vespula" [report vespula-ruleset]
  if ruleset-name = "parachartergus" [report parachartergus-ruleset]
  if ruleset-name = "pieces" [report pieces-ruleset]
  if ruleset-name = "lineup" [report lineup-ruleset]
end

; move all wasps around
to move-wasps
  ask wasps [
    right random 360
    forward 1
  ]
end

; build the wasp nest according to a ruleset
to build-nest [aRuleset materials]
  let surrounding (list [0 1][1 1][1 0][1 -1][0 -1][-1 -1][-1 0][-1 1])
  ask wasps [
    let i 0

    ; check all rules in ruleset
    while [i < (length aRuleset)] [
      let rule (item i aRuleset)
      let build-conditions (item 0 rule)
      let result (item 1 rule)

      ; buildable is true only if the patch the wasp is sitting on is empty
      let buildable true
      ask patch-here [
        set buildable (pcolor = (item 0 materials))
      ]

      ; check if the surrounding patches fit the build-conditions
      ; if they do, a resulting material is buildable
      (foreach build-conditions surrounding [
        [cond sur] ->
        ask patch-at (item 0 sur) (item 1 sur) [
          let matColor (item cond materials)
          set buildable (buildable and (pcolor = matColor))
        ]
      ])

      ; if buildable, then build the resulting material (by changing the patch color)
      ifelse buildable = true [
        let matColor (item result materials)
        ask patch-here [ set pcolor matColor ]
        ; set i = end of ruleset so the turtle doesn't check any other rules
        set i (length aRuleset)
      ] [
        ; if the current build-conditions do not apply, try the next rule in the ruleset (if available)
        set i (i + 1)
      ]
    ]
  ]
end
@#$#@#$#@
GRAPHICS-WINDOW
210
10
647
448
-1
-1
13.0
1
10
1
1
1
0
1
1
1
-16
16
-16
16
0
0
1
ticks
30.0

BUTTON
20
143
86
176
NIL
setup
NIL
1
T
OBSERVER
NIL
NIL
NIL
NIL
1

BUTTON
105
143
168
176
NIL
go
T
1
T
OBSERVER
NIL
NIL
NIL
NIL
1

INPUTBOX
19
194
168
254
number-of-wasps
50.0
1
0
Number

CHOOSER
19
353
191
398
ruleset
ruleset
"vespa" "vespula" "parachartergus" "pieces" "lineup"
1

SLIDER
19
254
191
287
density-material-a
density-material-a
0
100
39.0
1
1
NIL
HORIZONTAL

SLIDER
19
287
191
320
density-material-b
density-material-b
0
100
44.0
1
1
NIL
HORIZONTAL

SLIDER
19
320
191
353
density-material-c
density-material-c
0
100
10.0
1
1
NIL
HORIZONTAL

@#$#@#$#@
## WHAT IS IT?

Wasps build complex yet well structured nests for their colonies. Each wasp species builds a structurally distinct nest using similar building materials. With no master builder, the task of nest building is distributed over all the involved wasps. How does each wasp know what to do in order to succeed in building a structurally correct and functional nest? One of the theories to explain this emergent phenomenon uses the idea of qualitative stigmergy where the wasps use visual queues (stigma) in their environment to decide on their actions. In this assignment, we implement a model of wasp nest construction based on qualitative stigmergy.

## HOW IT WORKS

Initially, a set number of wasps and materials are randomly distributed. The wasps follow a ruleset for building materials for their nest. The rules of these rulesets define what materials the 8 patches immediately surrouding a wasp should have for the wasp to put down a certain material (further building its nest). A wasp can only build a new material if the patch it is sitting on is empty.

The rulesets "vespa", "vespula" and "parachartergus" are provided from the CPSC 565 Winter 2018 course. The other two rulesets, "pieces" and "lineup" are custom-made.

The "pieces" ruleset causes the wasps to build the nest in a way that each component of the nest seems to be a small piece. These pieces, however, don't often merge together and so they form a collection of pieces.

The "lineup" ruleset causes the wasps to build and extend the nest in lines or snake-like structures, sometimes with each patch alternating between materials.

## HOW TO USE IT

First, push the "setup" button to create a random distribution of materials and wasps.
The following input should be set beforehand:
 	- The number of wasps ("number-of-wasps" input box)
	- The density of material A ("density-material-a" slider)
	- The density of material B ("density-material-b" slider)
	- The density of material C ("density-material-c" slider)
	- The ruleset for building the wasp nest ("ruleset" dropdown)

Second, press the "go" button to start the simulation, which will go on forever until the "go" button is pressed again.

## THINGS TO NOTICE

Three types of materials are provided but not all of them need to be used to have the simulation work, even with custom rulesets. Note also that wasps are of random colors.
Material A is yellow.
Material B is lime.
Material C is violet.

## THINGS TO TRY

Try to vary the number of wasps, the number of different materials, the ruleset or density. Try to vary the numerical input with very small and very big numbers. Some configurations will lead wasps to build bigger nests, while in other cases they may not be able to build more than a few new materials.

## EXTENDING THE MODEL

To extend the model, add the ability to vary the number of individual material types to have a better view of nest building. It is possible to add new rulesets:

	1. add the ruleset name to the dropdown interface button
	2. add the ruleset name to the list of global variables
	3. set the ruleset in the "setup-rulesets" procedure
	4. add the appropriate line to the "print-ruleset" procedure to indicate how many materials is needed for the ruleset to function

With each ruleset, it may be required to add more materials (if the rules depend on new materials). In the Interface tab, this can be done by adding in new material density sliders for the new materials (to initially populate the map). In the Code tab, the "setup-patches" and "go" procedures should be modified to include the new materials.

To change the colors in the simulation so that they contrast more and are easier to distinguish, a different materials/pcolor list can be hardcoded instead. Note that the list in the "go" procedure requires that the default 'empty' color be listed first, however.

## NETLOGO FEATURES

An interesting feature that NetLogo provides is iterating through two lists  in a "foreach" loop, such that the index of the item referenced is the same for both lists.

Because "ask turtles" (or "ask wasps" in this case) is required for setting the caller to the turtles and not the observer, all manipulation involving the turtles in that step needed to be written in the same procedure.

NetLogo does not allow intializing global variables when they are declared so a separate setup procedure is required to initialize them.

## RELATED MODELS

Life

## CREDITS AND REFERENCES

The idea and the rulesets "vespa", "vespula" and "parachartergus" for this assignment was provided by the CPSC 565 Winter 2018 course.
@#$#@#$#@
default
true
0
Polygon -7500403 true true 150 5 40 250 150 205 260 250

airplane
true
0
Polygon -7500403 true true 150 0 135 15 120 60 120 105 15 165 15 195 120 180 135 240 105 270 120 285 150 270 180 285 210 270 165 240 180 180 285 195 285 165 180 105 180 60 165 15

arrow
true
0
Polygon -7500403 true true 150 0 0 150 105 150 105 293 195 293 195 150 300 150

box
false
0
Polygon -7500403 true true 150 285 285 225 285 75 150 135
Polygon -7500403 true true 150 135 15 75 150 15 285 75
Polygon -7500403 true true 15 75 15 225 150 285 150 135
Line -16777216 false 150 285 150 135
Line -16777216 false 150 135 15 75
Line -16777216 false 150 135 285 75

bug
true
0
Circle -7500403 true true 96 182 108
Circle -7500403 true true 110 127 80
Circle -7500403 true true 110 75 80
Line -7500403 true 150 100 80 30
Line -7500403 true 150 100 220 30

butterfly
true
0
Polygon -7500403 true true 150 165 209 199 225 225 225 255 195 270 165 255 150 240
Polygon -7500403 true true 150 165 89 198 75 225 75 255 105 270 135 255 150 240
Polygon -7500403 true true 139 148 100 105 55 90 25 90 10 105 10 135 25 180 40 195 85 194 139 163
Polygon -7500403 true true 162 150 200 105 245 90 275 90 290 105 290 135 275 180 260 195 215 195 162 165
Polygon -16777216 true false 150 255 135 225 120 150 135 120 150 105 165 120 180 150 165 225
Circle -16777216 true false 135 90 30
Line -16777216 false 150 105 195 60
Line -16777216 false 150 105 105 60

car
false
0
Polygon -7500403 true true 300 180 279 164 261 144 240 135 226 132 213 106 203 84 185 63 159 50 135 50 75 60 0 150 0 165 0 225 300 225 300 180
Circle -16777216 true false 180 180 90
Circle -16777216 true false 30 180 90
Polygon -16777216 true false 162 80 132 78 134 135 209 135 194 105 189 96 180 89
Circle -7500403 true true 47 195 58
Circle -7500403 true true 195 195 58

circle
false
0
Circle -7500403 true true 0 0 300

circle 2
false
0
Circle -7500403 true true 0 0 300
Circle -16777216 true false 30 30 240

cow
false
0
Polygon -7500403 true true 200 193 197 249 179 249 177 196 166 187 140 189 93 191 78 179 72 211 49 209 48 181 37 149 25 120 25 89 45 72 103 84 179 75 198 76 252 64 272 81 293 103 285 121 255 121 242 118 224 167
Polygon -7500403 true true 73 210 86 251 62 249 48 208
Polygon -7500403 true true 25 114 16 195 9 204 23 213 25 200 39 123

cylinder
false
0
Circle -7500403 true true 0 0 300

dot
false
0
Circle -7500403 true true 90 90 120

face happy
false
0
Circle -7500403 true true 8 8 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Polygon -16777216 true false 150 255 90 239 62 213 47 191 67 179 90 203 109 218 150 225 192 218 210 203 227 181 251 194 236 217 212 240

face neutral
false
0
Circle -7500403 true true 8 7 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Rectangle -16777216 true false 60 195 240 225

face sad
false
0
Circle -7500403 true true 8 8 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Polygon -16777216 true false 150 168 90 184 62 210 47 232 67 244 90 220 109 205 150 198 192 205 210 220 227 242 251 229 236 206 212 183

fish
false
0
Polygon -1 true false 44 131 21 87 15 86 0 120 15 150 0 180 13 214 20 212 45 166
Polygon -1 true false 135 195 119 235 95 218 76 210 46 204 60 165
Polygon -1 true false 75 45 83 77 71 103 86 114 166 78 135 60
Polygon -7500403 true true 30 136 151 77 226 81 280 119 292 146 292 160 287 170 270 195 195 210 151 212 30 166
Circle -16777216 true false 215 106 30

flag
false
0
Rectangle -7500403 true true 60 15 75 300
Polygon -7500403 true true 90 150 270 90 90 30
Line -7500403 true 75 135 90 135
Line -7500403 true 75 45 90 45

flower
false
0
Polygon -10899396 true false 135 120 165 165 180 210 180 240 150 300 165 300 195 240 195 195 165 135
Circle -7500403 true true 85 132 38
Circle -7500403 true true 130 147 38
Circle -7500403 true true 192 85 38
Circle -7500403 true true 85 40 38
Circle -7500403 true true 177 40 38
Circle -7500403 true true 177 132 38
Circle -7500403 true true 70 85 38
Circle -7500403 true true 130 25 38
Circle -7500403 true true 96 51 108
Circle -16777216 true false 113 68 74
Polygon -10899396 true false 189 233 219 188 249 173 279 188 234 218
Polygon -10899396 true false 180 255 150 210 105 210 75 240 135 240

house
false
0
Rectangle -7500403 true true 45 120 255 285
Rectangle -16777216 true false 120 210 180 285
Polygon -7500403 true true 15 120 150 15 285 120
Line -16777216 false 30 120 270 120

leaf
false
0
Polygon -7500403 true true 150 210 135 195 120 210 60 210 30 195 60 180 60 165 15 135 30 120 15 105 40 104 45 90 60 90 90 105 105 120 120 120 105 60 120 60 135 30 150 15 165 30 180 60 195 60 180 120 195 120 210 105 240 90 255 90 263 104 285 105 270 120 285 135 240 165 240 180 270 195 240 210 180 210 165 195
Polygon -7500403 true true 135 195 135 240 120 255 105 255 105 285 135 285 165 240 165 195

line
true
0
Line -7500403 true 150 0 150 300

line half
true
0
Line -7500403 true 150 0 150 150

pentagon
false
0
Polygon -7500403 true true 150 15 15 120 60 285 240 285 285 120

person
false
0
Circle -7500403 true true 110 5 80
Polygon -7500403 true true 105 90 120 195 90 285 105 300 135 300 150 225 165 300 195 300 210 285 180 195 195 90
Rectangle -7500403 true true 127 79 172 94
Polygon -7500403 true true 195 90 240 150 225 180 165 105
Polygon -7500403 true true 105 90 60 150 75 180 135 105

plant
false
0
Rectangle -7500403 true true 135 90 165 300
Polygon -7500403 true true 135 255 90 210 45 195 75 255 135 285
Polygon -7500403 true true 165 255 210 210 255 195 225 255 165 285
Polygon -7500403 true true 135 180 90 135 45 120 75 180 135 210
Polygon -7500403 true true 165 180 165 210 225 180 255 120 210 135
Polygon -7500403 true true 135 105 90 60 45 45 75 105 135 135
Polygon -7500403 true true 165 105 165 135 225 105 255 45 210 60
Polygon -7500403 true true 135 90 120 45 150 15 180 45 165 90

sheep
false
15
Circle -1 true true 203 65 88
Circle -1 true true 70 65 162
Circle -1 true true 150 105 120
Polygon -7500403 true false 218 120 240 165 255 165 278 120
Circle -7500403 true false 214 72 67
Rectangle -1 true true 164 223 179 298
Polygon -1 true true 45 285 30 285 30 240 15 195 45 210
Circle -1 true true 3 83 150
Rectangle -1 true true 65 221 80 296
Polygon -1 true true 195 285 210 285 210 240 240 210 195 210
Polygon -7500403 true false 276 85 285 105 302 99 294 83
Polygon -7500403 true false 219 85 210 105 193 99 201 83

square
false
0
Rectangle -7500403 true true 30 30 270 270

square 2
false
0
Rectangle -7500403 true true 30 30 270 270
Rectangle -16777216 true false 60 60 240 240

star
false
0
Polygon -7500403 true true 151 1 185 108 298 108 207 175 242 282 151 216 59 282 94 175 3 108 116 108

target
false
0
Circle -7500403 true true 0 0 300
Circle -16777216 true false 30 30 240
Circle -7500403 true true 60 60 180
Circle -16777216 true false 90 90 120
Circle -7500403 true true 120 120 60

tree
false
0
Circle -7500403 true true 118 3 94
Rectangle -6459832 true false 120 195 180 300
Circle -7500403 true true 65 21 108
Circle -7500403 true true 116 41 127
Circle -7500403 true true 45 90 120
Circle -7500403 true true 104 74 152

triangle
false
0
Polygon -7500403 true true 150 30 15 255 285 255

triangle 2
false
0
Polygon -7500403 true true 150 30 15 255 285 255
Polygon -16777216 true false 151 99 225 223 75 224

truck
false
0
Rectangle -7500403 true true 4 45 195 187
Polygon -7500403 true true 296 193 296 150 259 134 244 104 208 104 207 194
Rectangle -1 true false 195 60 195 105
Polygon -16777216 true false 238 112 252 141 219 141 218 112
Circle -16777216 true false 234 174 42
Rectangle -7500403 true true 181 185 214 194
Circle -16777216 true false 144 174 42
Circle -16777216 true false 24 174 42
Circle -7500403 false true 24 174 42
Circle -7500403 false true 144 174 42
Circle -7500403 false true 234 174 42

turtle
true
0
Polygon -10899396 true false 215 204 240 233 246 254 228 266 215 252 193 210
Polygon -10899396 true false 195 90 225 75 245 75 260 89 269 108 261 124 240 105 225 105 210 105
Polygon -10899396 true false 105 90 75 75 55 75 40 89 31 108 39 124 60 105 75 105 90 105
Polygon -10899396 true false 132 85 134 64 107 51 108 17 150 2 192 18 192 52 169 65 172 87
Polygon -10899396 true false 85 204 60 233 54 254 72 266 85 252 107 210
Polygon -7500403 true true 119 75 179 75 209 101 224 135 220 225 175 261 128 261 81 224 74 135 88 99

wheel
false
0
Circle -7500403 true true 3 3 294
Circle -16777216 true false 30 30 240
Line -7500403 true 150 285 150 15
Line -7500403 true 15 150 285 150
Circle -7500403 true true 120 120 60
Line -7500403 true 216 40 79 269
Line -7500403 true 40 84 269 221
Line -7500403 true 40 216 269 79
Line -7500403 true 84 40 221 269

wolf
false
0
Polygon -16777216 true false 253 133 245 131 245 133
Polygon -7500403 true true 2 194 13 197 30 191 38 193 38 205 20 226 20 257 27 265 38 266 40 260 31 253 31 230 60 206 68 198 75 209 66 228 65 243 82 261 84 268 100 267 103 261 77 239 79 231 100 207 98 196 119 201 143 202 160 195 166 210 172 213 173 238 167 251 160 248 154 265 169 264 178 247 186 240 198 260 200 271 217 271 219 262 207 258 195 230 192 198 210 184 227 164 242 144 259 145 284 151 277 141 293 140 299 134 297 127 273 119 270 105
Polygon -7500403 true true -1 195 14 180 36 166 40 153 53 140 82 131 134 133 159 126 188 115 227 108 236 102 238 98 268 86 269 92 281 87 269 103 269 113

x
false
0
Polygon -7500403 true true 270 75 225 30 30 225 75 270
Polygon -7500403 true true 30 75 75 30 270 225 225 270
@#$#@#$#@
NetLogo 6.0.2
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
default
0.0
-0.2 0 0.0 1.0
0.0 1 1.0 0.0
0.2 0 0.0 1.0
link direction
true
0
Line -7500403 true 150 150 90 180
Line -7500403 true 150 150 210 180
@#$#@#$#@
0
@#$#@#$#@

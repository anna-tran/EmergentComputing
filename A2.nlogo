breed [ants ant]
patches-own [
  food-units
  pheromones
  nest-smell
  nest?
]

globals [e-burchelli e-rapax sparse counter]

; setup simulation before running it
to setup
  clear-all
  define-distributions
  let distribution (select-distribution)

  setup-ants
  ask patches [
    setup-nest
    setup-food distribution
    set-color
  ]
end

; set up nest - patch procedure
to setup-nest
  set nest? (distancexy 0 0) < 4
  set nest-smell (50 - distancexy 0 0)
end

; define food distributions, each distribution has
; 1) number of units of food per patch
; 2) x, where 1/x is the probability of a patch initialized with food
to define-distributions
  set e-burchelli (list 1 2)
  set e-rapax (list 400 100)
  set sparse (list 2 10)
end

; initialize the food distribution according to the parameter supplied in the interface
to-report select-distribution
  if food-distribution = "E. burchelli" [report e-burchelli]
  if food-distribution = "E. rapax" [report e-rapax]
  if food-distribution = "sparse" [report sparse]
end


; set up food distribution - patch procedure
to setup-food [distribution]
  if not nest? [
    let units (item 0 distribution)
    let x (item 1 distribution)
    let rand (random x)
    ifelse (rand = 0)
      [set food-units units]
      [set food-units 0]
  ]
end

; set up colors
to set-color
  ifelse (nest?)
    [set pcolor violet]
  [
    ifelse (food-units > 0)
    [ifelse (show-food?) [set pcolor magenta] [set pcolor black]]
    [ifelse (show-pheromones?)
      [set pcolor scale-color orange pheromones 1 25]
      [set pcolor black]
    ]
  ]
  ;set plabel round nest-smell

end

; set up ant creation - ant procedure
to setup-ants
  set-default-shape ants "bug"
  create-ants ant-population [set color brown]
end

; start the simulation
; ant states are as follows:
; brown - looking for food
; red - returning to nest without food
; lime - returning to nest with food

to go
  ask ants [
    ifelse (color = brown) [look-for-food][
      ifelse (color = lime) [return-to-nest True] [
        ifelse (color = red) [return-to-nest False][]
      ]
    ]
    drop-pheromones color
  ]

  ; diffuse and evaporate pheromones
  diffuse pheromones (1 / 40)
  ask patches [
    set pheromones (pheromones * (29 / 30))
    set-color
  ]

  show counter
  ; reset the ticks to allow another 10 ants to leave the nest
  set counter 0

end

; have an ant drop its pheromones on a site
; each site can hold a maximum of 1000 units of pheromones
; ants drop different amounts of pheromones depending on the state
to drop-pheromones [state]
  let amount 0
  ifelse (state = brown) [set amount 1][
    ifelse (state = lime) [set amount 10][
      ifelse (state = red) [set amount 1][]
    ]
  ]
  ask patch-here [if (pheromones + amount) < 1000 [set pheromones (pheromones + amount)]]
end

; the ant is leaving the nest to look for food
; the ant chooses to move only a patch farthest from the nest (among its 8 neighboring patches)
; if the ant finds food while in this state, it changes state and returns to the nest
to look-for-food
  let found-food? False

  ask patch-here [
    ; if found food, signal
    if (food-units > 0) [
      set food-units (food-units - 1)
      set found-food? True
    ]
  ]

  ; if ants find food, change color/state and return to nest with food
  if found-food? [
    set color lime
    rt 180
  ]
  ; try moving
  let possible-patches (sort (min-n-of 2 neighbors [nest-smell]))
  let patch1 (item 0 possible-patches)
  let patch2 (item 1 possible-patches)
  try-moving patch1 patch2
end

; ant will move depending on probability and pheromone concentration
to try-moving [patch1 patch2]
  ; a1 - pheromone concentration of patch 1
  ; a2 - pheromone concentration of patch 2
  let a1 0
  let a2 0
  ask patch1 [set a1 pheromones]
  ask patch2 [set a2 pheromones]

  let prob-move ((1 / 2) * (1 + tanh (((a1 + a2) / 100) - 1)))

  ; if ant decides to move
  if (random-float 1.0) < prob-move [
    check-for-edge
    ; p1         - probability of moving to patch1
    ; rand-patch - randomly chosen patch of patch1 and patch2
    let p1 0
    if (a1 + a2) > 0 [set p1 (a1 / (a1 + a2))]
    let rand-patch (random-float 1.0)


    ; move to patch1 or patch2 only if there are less than 20 ants on those patches
    ifelse (rand-patch < p1 and (count ants-on patch1) < 20)
    [wiggle-to patch1]
    [if ((count ants-on patch2) < 20)
      [wiggle-to patch2]
    ]
  ]
end


; based on the turtle procedure from Biology/Ant Netlogo model
; make the ant move forward in whatever direction its destination patch is
; the ant wiggles as it moves
to wiggle-to [destination-patch]
  face destination-patch
  rt random 40
  lt random 40

  ; delay for departure from nest so that only 10 ants can leave the nest at each time step
  let nest-to-outside? False
  ; check initially if in nest
  ask patch-here [if nest? [set nest-to-outside? True]]
  ; now check if destination outside nest
  ask destination-patch [
      ifelse (not nest?)
      [set nest-to-outside? (nest-to-outside? and True)]
      [set nest-to-outside? (nest-to-outside? and False)]
  ]
  ; move only if initially in nest and one of the first 10 ants to leave the nest
  ; or if already outside of nest
  if (nest-to-outside? and counter < 10)
  [
    set counter (counter + 1)
    fd 1
  ]
  if (not nest-to-outside?) [fd 1]

end

; if ant is at edge of the NetLogo world, it turns around
to check-for-edge
  if not can-move? 1 [
    rt 180
    set color red
    stop
  ]
end

; tanh function
to-report tanh [x]
  report ((e ^ x) - (e ^ (- x))) / ((e ^ x) + (e ^ (- x)))
end

; an ant returns to the nest by looking for and possibly moving the neighboring patch closest to the nest
; if the ant is not carrying any food, but encounters food along the way, it will pick up the food and change
; color/state
to return-to-nest [found-food?]
  let at-nest? False
  let ant-state color

  ask patch-here [
    ; if arrived at nest center
    if (distancexy 0 0) < 1 [set at-nest? True]

    ; pick up food only if not carrying food already
    if (food-units > 0 and not found-food?) [
      set food-units (food-units - 1)
      set found-food? True
    ]
  ]

  if found-food? [set color lime]
  if at-nest? [set color brown]

  ; try moving
  let possible-patches (sort max-n-of 2 neighbors [nest-smell])
  let patch1 (item 0 possible-patches)
  let patch2 (item 1 possible-patches)
  try-moving patch1 patch2

end
@#$#@#$#@
GRAPHICS-WINDOW
223
10
719
507
-1
-1
8.0
1
8
1
1
1
0
0
0
1
-30
30
-30
30
0
0
1
ticks
30.0

BUTTON
33
108
99
141
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

CHOOSER
31
224
169
269
food-distribution
food-distribution
"E. burchelli" "E. rapax" "sparse"
1

SLIDER
32
170
204
203
ant-population
ant-population
1
1000
65.0
1
1
NIL
HORIZONTAL

BUTTON
112
108
175
141
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

SWITCH
31
287
211
320
show-pheromones?
show-pheromones?
1
1
-1000

SWITCH
31
333
162
366
show-food?
show-food?
0
1
-1000

@#$#@#$#@
## WHAT IS IT?

Army ants form aggressive predatory foraging groups (a.k.a. raids) sometimes made up of hundreds of thousands of individual ants. They are often studied due to their distinctive raiding patterns that differentiate various species. In this simulation we take a look at two species of army ants (Eciton rapax and Eciton burchelli) and test the hypothesis that the distinct raiding patterns are due to different spatial distributions of food items in the environment. This model is an example of quantitative stigmergy where the ants use pheromone (chemical) concentration in the environment to decide their actions.

## HOW IT WORKS

Army ants will be modeled using turtles that move based on pheromone concentrations. Ants will leave the nest looking for food in the direction opposite of the nest; more specifically, at each time step, an ant locates two patches (of their 8 neighbouring patches) that are farthest from the nest (such that the absolute difference between the nest-smell of the given patch and the nest-smell at the center of the nest is maximized). It decides on which of the two patches it will move to according to the probabilities provided below. Once an ant found a food source, it should pick it up, turn around, and deliver it back to the nest.

Let the two patches that are farthest from the nest from the ant's position be patch1 and patch2. The probability of an ant moving is p = (1/2) * (1+tanh(((a1+a2)/100)-1)) where a1 and a2 are the respective pheromone concentrations of patch1 and patch2. If the ant does decide to move, the probability of moving to patch1 is a1/(a1+a2) and the probability of moving to patch2 is a2/(a1+a2).

To make the trajectories of ants more natural, besides possible directional moves, each ant also does a random wiggle at each time step. This code was referenced from the Biology/Ants model in the NetLogo models library.

# Simulation rules

The ants have 3 states represented by 3 distinct colours:
1) Looking for food (brown)
2) Returning to nest without food (red)
3) Returning to nest with food (green)

At any point, ants can only carry one unit of food. Ants carrying food drop 10 units of pheromones per each visited site, whereas those without drop 1 unit of pheromones. Each site can only be visited by at most 20 ants at a time, and can only contain at most 1000 units of pheromones.

Pheromones evaporate at a rate of 1/30 per time step, and diffuse at a rate of 1/40 per time step. If the 'show-pheromones?' switch is on, the pheromones are indicated by an orange colour gradient: the lower the concentration, the darker the color and the higher the concentration, the lighter the color. 

Exactly 10 ants can leave the nest and look for food at each time step (tick). If the patch/site it wants to move to is full (20 ants already occupy the patch) then it moves to the second possible patch. If that second patch is also full, then the ant does not move at all.

If an ant finds food, it picks up one unit of food and heads back towards the nest.
If an ant reaches the boundary of the grid in the NetLogo world, then it changes to red, and returns to the nest. If the ant finds food along the way, it picks up one unit of food and then continues its route towards the nest.

# Food distributions

To simulate the different foraging behaviors of army ant species, we will use at least two different initial food distributions:
a) E. burchelli: site contains 1 unit of food with probability of 1/2
b) E. rapax: site contains 400 units of food with probability of 1/100
c) rare: site contains 2 units of food with probability of 1/10

Distributions c) is inspired by a) and b); it creates a sparser distribution of food than a) but denser than b).

## HOW TO USE IT

To use the model, set the initial
	- ant population (with the ant-population slider)
	- food distribution rule (with the food-distribution dropdown)
	- pheromone visualization (with the show-pheromones? switch)
	- food visualization (with the show-food? switch)
then press the "setup" button to setup the simulation.

Press the "go" button to start the simulation, which will run forever unless the button is pressed again.

## THINGS TO NOTICE

The ants in this simulation, due to the way the destination sites are decided, move in one of 8 directions. Initially, an ant is at the center of the world (the nest), in which case all 8 neighbours have the same nest-smell. However, once the ant moves to another patch, there are at most 2 or 3 patches that are considered the farthest from the nest at all times. 

Imagine for example that an ant first moves to the north-west patch. The following image depicts the ant (denoted by X) and its 8 neighbouring patches.

				|-|-| |
				|-|X| |
				| | | |

The '-' indicates that this patch is among the 'destination patches' since these patches are diagonal to the north-west would logically be the furthest from the nest. This pattern would repeat itself and the ant would continuously move in almost a straight line. The pheromones dropped by the ant would signal other ants to follow in that direction as well. This chain reaction will engender more ants to copy this direction of movement. With a large enough ant population, it is easy to note that there are 8 main trajectories the ants will take.

As a result, they will not likely deviate from these trajectories and never reaches some areas of the NetLogo world, which may contain food. This is due to the fact that the movement of the ants is primarily based on the nest-smell, a fixed factor, rather than on the pheromone concentration, which would indicate sources of food.

# Raid Patterns

In terms of retrieving food, however, the ants will pick up food sources closest to the nest before looking for food farther in the same direction.

With the E. burchelli food distribution, about half of the patches were randomly distributed with a unit of food. This meant that the ants would reach the farther patches of the map more slowly because they would quickly fetch the closest food and return to the nest. This delayed reaching food farther away from the nest.

With the E. rapax food distribution, food patches were more scarce (though in greater concentration). Ants were capable of moving farther on the grid and would usually be able to find a food source it could continually return to until all the food was depleted.

The ants under the 'scarce' food distribution acted similarly to the ants in the E. burchelli food distribution. On the other hand, they were able to reach the farther ends of the grid more quickly because of the lower density of food.

## THINGS TO TRY

Try to vary the ant population to see how ants would move in groups, despite only 10 ants leaving the nest at a time. Using different food distributions can clearly show the order in which food sites are visited. The 'show-pheromones?' switch displays or omits the display of pheromones. The 'show-food?' switch displays or omits the display of food, which may be useful to better observe how ants displace themselves with different food distributions.

## EXTENDING THE MODEL

To extend the model, one could modify the choice of destination patches to randomize the overall ant displacement and allow ants to reach the currently unreachable patches on the grid.

This simulation maps single patches with food units, dispersed randomly. An improvement can be to add the option of clustering patches of food units together much like the Ants model from the NetLogo library.

Providing a slider for diffusion and evaporation rates for the pheromones may also help to visualize how effective pheromones are used to communicate between ants regarding food sources.

## NETLOGO FEATURES

It was convenient to use the 'min-n-of' and 'max-n-of' functions in the NetLogo library to obtain the two possible destination patches based on a patch characteristic. This prevented me from needing to implement a similar method myself.

Having the ability to assign multiple characteristics to agents like patches was also helpful in distinguishing between the nest patches and food patches.
(interesting or unusual features of NetLogo that the model uses, particularly in the Code tab; or where workarounds were needed for missing features)

## RELATED MODELS

Biology, Ants

## CREDITS AND REFERENCES

The simulation description, rules and initial food distributions were provided by the CPSC 565 Emergent Computing Assignment 2 description.
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

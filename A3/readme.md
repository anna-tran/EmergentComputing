# Quidditch Simulation (Unity)

This Unity project simulates a game of Quidditch, a competitive sport in the wizarding world of Harry Potter, 
a series of fantasy novels written by J. K. Rowling. It is a game played by wizards and witches, where there’re 
two teams riding on flying broomsticks trying to score as many points as possible. In this assignment we will 
implement a simplified version of this game.

There are 20 players from each team. A Griffindor player is represented by a red disk with a yellow stub indicating
the direction the player is facing. A Slytherin player is represented by a green disk with a black stub. The golden 
snitch will be modeled by a small golden sphere that randomly flies around the Quidditch field, and bounces off the 
ground and invisible boundary walls when it collides with those surfaces.

Both the Griffindor and Slytherin team have starting points in the game. When the game starts, the players move out 
of the starting point sequentially.

## Players

The players have two main urges: to catch the snitch and to avoid frequent collisions with nearby players. In this
implementation, players are always trying to face towards the snitch and move towards them. If the player detects
another player in a certain range, then the player swerves slightly left or right to avoid collision. This is under 
the assumption a player is only able to notice other players in range if they are in direct line of sight. Players 
do not account for the presence of other team or opponent players other than to avoid collisions.

Each player on a team has a max velocity, max acceleration and a probability of successfully tackling an opponent 
player upon collision. If an opponent player does end up being tackled, then the opponent free fall until hitting 
the ground, appear at his team’s starting point, and resume the game. The snitch also has a max velocity and max
acceleration. The two teams have different parameters as the Gryffindor team is known to be faster, and the 
Slytherin team is known to be better at tackling opponent players.

## Player parameters

The parameters set in this simulation are
Gryffindor:
maxVelocity = 16, maxAcceleration = 20, tacklingProb = 0.3
Slytherin:
maxVelocity = 13, maxAcceleration = 17, tacklingProb = 0.7
Snitch:
maxVelocity = 16, maxAcceleration = 20

## Varying parameters

A few variations in the parameters led to different results. In each variation, there was a control group (team).
When both team parameters were equal, the scores were also roughly the same. Sometimes one team would score more
than the other however.
By largely decreasing the velocity and acceleration of players on one team, that team scored less than their 
opponents because they couldn't move as fast to try and catch the snitch.
Largely increasing the velocity and acceleration made it difficult for players to turn around and head back towards
the snitch due to the momentum and velocity. As a result, the faster moving team also scored less than their 
opponents.

Velocity is assigned to the Rigidbody object velocity and acceleration influences the amount of force applied to
a the snitch or a player to get them to move.

## Things to note

With so many objects moving quickly around in a limited space, there is sometimes a lag in the players respawning 
and going back into the game. This may cause an issue as it limits the potential of a team to catch the snitch and
gain points.
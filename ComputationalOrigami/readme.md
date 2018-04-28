# Modular Origami Simulator

This program simulates the interaction between modular origami units, which are folded and connected to one another through insertion into pockets. It is programmed in Unity 3D 2017.3.1.
An initial origami unit will be produced at the start of the simulation and at most 100 new origami units will be produced throughout. These values are hardcoded in the Simulator.cs file,
but could be modified if desired.

## Running the Program 

1. Install Unity 3D 2017.3.1 or a later version.
2. Create a new Unity 3D project and import the ModularOrigamiAssets.unitypackage into the project.
3. Open the OrigamiFitting scene in the Assets folder.
4. Select the 'Simulator' GameObject on the Hierarchy panel on the left-hand side.
5. Input the probabilities of each fold, from 0.0 to 1.0.
6. Press the play button to view the simulation.
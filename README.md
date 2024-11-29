# Thomas Reiffer 
Technical Game Designer - Prototyping Portfolio
thomas.reiffer@gmail.com

This repo contains tools and gameplay logic that I've implemented for my own projects. 
Initial commit is a snapshot of said WIP project, which had been stored on a different VC as git's LFS and UI aren't great for collaborating with non-technical assets and colleagues.

Please bear in mind that all works here are WIP - they are active parts of my side-projects. As such, this code will feature references to systems that are not included and empty spaces where future features will go.


Included tools:

# Action Controller & "Action" State Gameplay Logic
A FSM for character gameplay behaviour, interpreting game state and ouputting both behaviour and animation.

The action controller sits between a character manager class and mecanim. Each update, the action controller iterates through its available actions, evaluates each state's activation parameters, and then transitions between these states accordingly. This drives mecanim state machines, and each action is tightly coupled to gameplay behaviour contained within them.

# VisMap

A solution for full-3D pathfinding for ranged aerial combatants. Assumes a static environment.

Part of The Markov Eclipse's feature list is performant full-3D target- and path-finding for aerial combatants. To achieve this, I wrote an algorithm that bakes cover data from sampled positions in 3D space. For each point, it generates a bit array to represent if it can see each point on the grid in turn. It then serializes them out to a .bin to be streamed in along with level geometry and traditional pathfinding data. The initial bake can take a while depending on the size of the arena, but the binaries are quick to deserialize and the deserialized data is then performant when accessing at runtime due to accessing bits rather than coordinate data.  

# Command Queue
Caching player commands per-unit for an RTS game, and executing them in turn.

The command queue is a classic first in, first out command buffering queue. The complexity here is containing all of the varied data that a squad tactics RPG/RTS needs to include in those commands. Each combatant has its own queue, both NPC and PC, but the PCs' CQ is driven by player inputs. This is the least developed of the systems included. 
	



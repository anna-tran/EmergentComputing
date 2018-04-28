using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Simulates origami units flying around and inserting themselves into the pockets of other units.
 * Each unit is folded by an OrigamiFolder given user inputted probabilities of each fold upon initialization.
 * The number of initial units is defaulted to 1 and the number of additional units to generate defaults to 100.
 * All units can ony be instantiated within the bounds of a zone but can move outside of that zone.
 */
public class Simulator : MonoBehaviour {
	private static float ROTATION_SPEED = 250;			// speed of unit rotation
	private static float SLOW_ROTATION_SPEED = 30;		// slowed speed of unit rotation
    private static int FPS = 30;						// frames per second
	private static int DELAY_SECONDS = 3 * FPS;			// time to delay before spawning a new unit
	private static float TRANSLATION_SPEED = 25.0f;		// speed of unit translation
	private static float SLOW_TRANSLATION_SPEED = 1.5f; // slowed speed of unit translation
	private static int UNIT_LIFETIME = 20 * FPS;		// maximum time unit can exist in each stage before being destroyed (in seconds)
	private static float STOP_DISTANCE = 0.6f;			// maximum distance to be kept between unit and target pocket center

	public OrigamiUnit square;	
	private OrigamiFolder folder;


	public float probHorzFold;							// probability of doing a horizontal fold
	public float probVertFold;							// probability of doing a vertical fold
	public float probDiagRightFold;						// probability of doing a right diagonal fold
	public float probDiagLeftFold;						// probability of doing a left diagonal fold
    private int initUnits;								// number of initial units to generate in game
	private int unitsToGenerate;						// number of units to generate in simulation
    private Transform zone;								// boundary in which units are generated
    private Queue<Pocket> pockets;						// pockets not yet filled by any unit
    private List<Tuple<int,OrigamiUnit>> activeUnits;	// list of units currently trying to fill a pocket
														// int			- lifetime of unit
														// OrigamiUnit	- an instance of a modular origami unit

    private OrigamiUnit squareCopy;						// to create copies of Origami Units

	private int count;									// count for spawning
	private int numGenerated;							// number of spawned units




	void Start()
    {
        count = 0;
		initUnits = 1;
		unitsToGenerate = 100;
        pockets = new Queue<Pocket>();
		activeUnits = new List<Tuple<int,OrigamiUnit>>();
		zone = GameObject.Find ("Zone").transform;
		folder = GameObject.Find ("Folder").GetComponent<OrigamiFolder> ();

        // generate inactive, initial units
		for (int i = 0; i < initUnits; i++) {
			squareCopy = Instantiate (square, transform.position, transform.rotation) as OrigamiUnit;
			squareCopy.rendMesh = true;
			squareCopy.name = "4Square(Base) " + numGenerated;
			folder.RandomlyFold (squareCopy,
				probHorzFold,
				probVertFold,
				probDiagRightFold,
				probDiagLeftFold);
			UnityHelper.RandomlyRotate (squareCopy.transform);
			UnityHelper.RandomlyPosition (squareCopy.transform, zone);
			// for each initial unit, add all pockets to list of available pockets
			foreach (Pocket p in squareCopy.pockets) 
			{
				PushPocket (p);
			}
			numGenerated++;
		}
		numGenerated = 0;

	}

    void Update()
    {
		// spawn a new origami unit over set intervals and only if there are enough pockets
		// on the field for a new unit to insert into
		if (TimeToSpawn() && pockets.Count > 0 && numGenerated < unitsToGenerate)
        {
			InstantiateUnit();
        }
		// toRemove 	- remove inactive units after the frame
		// toDestroy 	- if the unit has been trying to insert itself into the target pocket for too long, kill it
		List<string> toRemove = new List<string>();
		List<string> toDestroy = new List<string>();
		for (int i = 0; i < activeUnits.Count; i++)
        {
			Tuple<int,OrigamiUnit> tup = activeUnits[i];
			OrigamiUnit unit = tup.second;
			// if pocket is no longer available, move the unit backwards first so it has room to move around
			// and move towards a potentially new target pocket
			if (!PocketAvailable (tup)) {
				if ((unit.targetP != null && MovedAwayFromTarget (unit)) || unit.targetP == null) {
					unit.targetP = null;
					unit.ResetStage ();
				} else if (unit.targetP != null) {
					MoveAwayFromTarget (unit);
				}
			}

			tup.first++;
			// unit stages for insertion
			if (unit.stage == 0) {
				SetupFolds (tup, unit, toDestroy);

			} else if (unit.stage == 1) {
				AlignLookAtTarget (tup, unit);

			} else if (unit.stage == 2) {
				RotateAroundTarget (tup, unit);

			} else if (unit.stage == 3) {
				AlignRotationToTarget (tup, unit);
			
			} else if (unit.stage == 4) {
				InsertIntoPocket (tup, unit);

			} else if (unit.stage == 5) {
				ShiftToMatchTarget (tup, unit);
			
			} else {
				FillPocket (unit);
				// add new pockets to queue only if they are not filled by the parent
				foreach (Pocket p in unit.pockets) {
					if (p.InaccessibleDueTo (unit.targetP.pCenter.parent))
						p.filled = true;
					else
						PushPocket (p);
				
				}
				// unit has completed all stages so render it inactive
				toRemove.Add (unit.name);
			}

			// if unit has reached the end of its lifetime while trying to insert, destroy it
			if (EndLifetime (tup.first, unit.stage)) {
				toDestroy.Add (unit.name);
				// put pocket back onto the field
				PushPocket (unit.targetP);
			}
		

        }
		string output = activeUnits.Count.ToString();;
		activeUnits.RemoveAll (tup => toDestroy.Contains(tup.second.name));
		activeUnits.RemoveAll (tup => toRemove.Contains (tup.second.name));
		foreach (string unitName in toDestroy) {
			GameObject.Destroy (GameObject.Find(unitName));
		}

    }

	/*
	 * Check if unit is at a certain distance from target
	 */
	private bool MovedAwayFromTarget(OrigamiUnit unit) {
		Vector3 distFromTarget = unit.GetIV().position - unit.targetP.pCenter.position;
		return distFromTarget.sqrMagnitude > 50.0f;
	}

	/*
	 * Move unit away from center of target pocket
	 */
	private void MoveAwayFromTarget(OrigamiUnit unit) {
		Vector3 pointToCenter = 3.0f * unit.GetAlignmentV3 ();
		unit.transform.position = Vector3.MoveTowards (unit.transform.position, 
			unit.targetP.pCenter.position - pointToCenter, 
			TRANSLATION_SPEED * Time.deltaTime);
	}


	/*
	 * Check if unit's target pocket is no longer available
	 */
	private bool PocketAvailable(Tuple<int,OrigamiUnit> tup) {
		OrigamiUnit unit = tup.second;
		if (unit.targetP == null || unit.targetP.filled) {
			return false;
		}
		return true;
	}

	/*
	 * Check if unit has hit its lifetime and that it has not reached the insertion stage
	 */
	private bool EndLifetime(int life, int stage) 
	{
		return life >= UNIT_LIFETIME && stage >= 0 && stage < 5;
	}

	/*
	 * Instantiate/spawn a new origami unit given the probability of each fold and randomly rotate it
	 */
	private void InstantiateUnit()
	{
		squareCopy = Instantiate(square, transform.position, transform.rotation) as OrigamiUnit;
		squareCopy.name = "4Square " + numGenerated;
		folder.RandomlyFold(squareCopy,
			probHorzFold,
			probVertFold,
			probDiagRightFold,
			probDiagLeftFold);
		UnityHelper.RandomlyRotate (squareCopy.transform);
		activeUnits.Add(new Tuple<int,OrigamiUnit>(0,squareCopy));
		numGenerated++;
	}

	/*
	 * Find a target pocket if it exists, calculate the unit's self rotation vector and move on to the next stage.
	 * If there is no pocket available, then the unit cannot be inserted anywhere so destroy the unit.
	 */
	private void SetupFolds(Tuple<int,OrigamiUnit> tup,OrigamiUnit unit, List<string> toDestroy) 
	{
		// keep checking until an available pocket is found
		// must check p.filled in case the previously available pocket is considered "filled"
		// because of a unit that overlaps with it
		Pocket p;
		do {
			p = PopPocket ();
		} while (p != null && p.filled);

		// if p==null then there are no more available pockets
		// add unit to list of units to destroy since it can't go anywhere
		if (p == null) {
			toDestroy.Add (unit.name);
		} else {
			
			// if different angles (pocket and insertion point) or the type of pocket does not match with the 
			// number of unit folds,
			// put pocket back in queue
			// don't continue and stay in stage 0
			if (!UnityHelper.CanFitPocket (unit, p) 
				&& !UnityHelper.CorrectTargetPocket(unit, p)) {
				PushPocket (unit.targetP);
				unit.ResetTargetPocket ();
				unit.ResetIV ();
			} else {
				unit.targetP = p;
				unit.CalcSelfRotationV3 ();
				unit.MoveToNextStage ();
				// reset unit lifetime
				tup.first = 0;
			}
		}
	}

	/*
	 * Make the origami unit insertion vertex face the target pocket before moving to the next stage
	 */
	private void AlignLookAtTarget(Tuple<int,OrigamiUnit> tup, OrigamiUnit unit) {
		// currV3 		- current direction of insertion vertex relative to center of unit
		// alignToV3 	- direction to align insertion vertex towards
		Vector3 currV3 = unit.GetAlignmentV3 ().normalized;
		Vector3 alignToV3 = (unit.targetP.pCenter.position - unit.GetIV().position).normalized;

		// if the current direction and the intended direction are not the same, rotate the unit to face the pocket
		if (!UnityHelper.V3EqualMagn (currV3, alignToV3)) {
			if ((currV3 - alignToV3).sqrMagnitude < 0.07f) {
				unit.transform.RotateAround (unit.GetIV ().position, unit.selfRotationV3, SLOW_ROTATION_SPEED * Time.deltaTime);	
			} else {
				unit.transform.RotateAround (unit.GetIV ().position, unit.selfRotationV3, ROTATION_SPEED * Time.deltaTime);
			}
		} else {
			// calculate the vector to rotate around the target pocket
			unit.CalcTargetRotationV3 ();
			unit.MoveToNextStage ();


			tup.first = 0;
		}
	}

	/*
	 * Rotate the origami unit around the target pocket until the direction insertion vertex 
	 * (relative to the center of the unit) is aligned with the insertion vector into the pocket
	 */
	private void RotateAroundTarget(Tuple<int,OrigamiUnit> tup,OrigamiUnit unit) {
		// normRotating	- current rotational vector of the unit
		// normStill	- static insertion vector of the pocket
		Vector3 normRotating = (unit.GetIV().position - unit.targetP.pCenter.position).normalized;
		Vector3 normStill = (unit.targetP.GetVectorIn ()).normalized;

		// if the current direction and the intended direction are not the same, rotate the unit to align
		// with the insertion vector of the pocket
		if (!UnityHelper.V3EqualMagn (normRotating, normStill)) {
			if ((normRotating - normStill).sqrMagnitude < 0.07f) {
				unit.transform.RotateAround (unit.targetP.pCenter.position, unit.targetRotationV3, SLOW_ROTATION_SPEED * Time.deltaTime);
			} else {
				unit.transform.RotateAround (unit.targetP.pCenter.position, unit.targetRotationV3, ROTATION_SPEED * Time.deltaTime);
			}
		} else {
			unit.MoveToNextStage ();
			// reset unit lifetime
			tup.first = 0;
		}
	}

	/*
	 * Rotate the origami unit along its local y-axis until the unit is about parallel with its
	 * target pocket, and the number of overlapping children fit the unit 
	 * (this number is based on its number of folds)
	 */
	private void AlignRotationToTarget(Tuple<int,OrigamiUnit> tup,OrigamiUnit unit) {
		// get the two planes formed by the unit and the target pocket
		OrigamiUnit target = unit.targetP.pCenter.parent.GetComponent<OrigamiUnit> ();
		Plane plane1 = UnityHelper.GetPlaneOfVertex (unit,unit.GetIV());
		Plane plane2 = UnityHelper.GetPlaneOfVertex (target, unit.targetP.edge1.end);

		// if the planes are roughly parallel
		if (!UnityHelper.ApproxEqualPlane(plane1,plane2)
			|| !UnityHelper.CorrectOverlap(unit)) { 
			if ((plane1.normal - plane2.normal).sqrMagnitude < 0.1f ||
				(UnityHelper.GetOppositeV3(plane1.normal) - plane2.normal).sqrMagnitude < 0.1f
				) {
				unit.transform.RotateAround (unit.GetIV ().position, unit.GetIV().position - unit.targetP.pCenter.position, -SLOW_ROTATION_SPEED * Time.deltaTime);	
			} else {
				unit.transform.RotateAround (unit.GetIV ().position, unit.GetIV().position - unit.targetP.pCenter.position, -ROTATION_SPEED * Time.deltaTime);
			}
		} else {
			unit.MoveToNextStage ();
			// reset unit lifetime
			tup.first = 0;

		}
	}

	/*
	 * Move the origami unit towards the center of its target pocket until it reaches
	 * a specific distance from the center
	 */
	private void InsertIntoPocket(Tuple<int,OrigamiUnit> tup,OrigamiUnit unit) {
		Vector3 distFromTarget = unit.GetIV().position - unit.targetP.pCenter.position;
		if (distFromTarget.magnitude > STOP_DISTANCE) {
			Vector3 pointToCenter = unit.transform.position - unit.GetIV().position;
			unit.transform.position = Vector3.MoveTowards (unit.transform.position, unit.targetP.pCenter.position + pointToCenter, TRANSLATION_SPEED * Time.deltaTime);
		} else {
			unit.MoveToNextStage ();
			// reset unit lifetime
			tup.first = 0;
		}
	}

	/*
	 * After inserting the origami unit, adjust its position
	 */
	private void ShiftToMatchTarget(Tuple<int,OrigamiUnit> tup,OrigamiUnit unit) {
		Vector3 end1 = unit.targetP.edge1.end.localPosition - unit.targetP.pCenter.localPosition;
		Vector3 end2 = unit.targetP.edge2.end.localPosition - unit.targetP.pCenter.localPosition;
		float yDiff = (Math.Abs (end1.y) > Math.Abs (end2.y)) ? end1.y : end2.y;
		yDiff = (yDiff < Pocket.EDGE_POCKET_DISTANCE) ? yDiff : 1.5f * yDiff;
		Plane plane = unit.targetP.GetPocketPlane ();
		Transform iv = unit.GetIV ();

		// translate the unit by a factor of the distance between the height of the pocket
		if (Math.Abs(plane.GetDistanceToPoint(iv.position)) < yDiff) {
			Vector3 dir = plane.normal;
			dir = (plane.GetSide(iv.position)) ? dir : UnityHelper.GetOppositeV3 (dir);
			unit.transform.position = Vector3.MoveTowards (unit.transform.position, unit.transform.position + dir, SLOW_TRANSLATION_SPEED * Time.deltaTime);
		} else {
			unit.MoveToNextStage();
			// reset unit lifetime
			tup.first = 0;
		}
	}

	/*
	 * Fill the target pocket and remove from list of pockets in the target unit
	 */
	private void FillPocket(OrigamiUnit unit) {
		unit.targetP.filled = true;
		OrigamiUnit target = unit.targetP.pCenter.parent.GetComponent<OrigamiUnit>();

		// check if any of the other target's pockets are filled by this unit
		// if so, mark it as filled
		target.pockets.ForEach (p => {
			if (p.OverlappedBy (unit.transform))
				p.filled = true;
		});
	}

	/*
	 * Add a pocket of the list of available pockets
	 */
    public void PushPocket(Pocket p)
    {
        pockets.Enqueue(p);

    }

	/*
	 * Remove a pocket from the list of available pockets if it exists, and return it
	 */
    public Pocket PopPocket()
    {
       if (pockets.Count > 0)
        {

            Pocket p = pockets.Dequeue();
            return p;
        }
    
        return null;

    }

	/*
	 * Check if it's time to spawn a new origami unit
	 */
    private bool TimeToSpawn()
    {
        count++;
        if (count >= DELAY_SECONDS)
        {
            count = 0;
            return true;
        }
        return false;
    }

	/*
	 * For debugging purposes.
	 * Print out all the values in a collection.
	 */
	public static void PrintValues( IEnumerable myCollection )  {
		string output = "";
		foreach (System.Object obj in myCollection)
			output += " " + ((Pocket) obj).pCenter.parent.name;
		print (output);
	}

	
}
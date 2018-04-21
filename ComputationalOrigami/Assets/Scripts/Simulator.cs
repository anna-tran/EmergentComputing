﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	private static float ROTATION_SPEED = 100;
    private static int FPS = 30;
	private static int DELAY_SECONDS = 5 * FPS;
	private static float INSERTION_SPEED = 8.0f;
	private static int UNIT_LIFETIME = 10 * FPS;
	private static float STOP_DISTANCE = 0.6f;

	public FourSquare square;

    public int initUnits;
	public int unitsToGenerate;
	public float probCenterInsertion;
	public float probVertFold;
	public float probHorzFold;
	public float probDiagRightFold;
	public float probDiagLeftFold;
    private Transform zone;
    private Queue<Pocket> pockets;
    private List<Tuple<int,FourSquare>> activeUnits;
    FourSquare squareCopy;
	int stage;
    int count;
	Transform point;

    private int temp;
	private int numGenerated;




	void Start()
    {
        count = 0;
        pockets = new Queue<Pocket>();
		activeUnits = new List<Tuple<int,FourSquare>>();
		zone = GameObject.Find ("Zone").transform;
        // all inactive units
		for (int i = 0; i < initUnits; i++) {
			squareCopy = Instantiate (square, transform.position, transform.rotation) as FourSquare;
			squareCopy.rendMesh = true;
			squareCopy.name = "4Square(Base) " + temp;
			OrigamiFolder.RandomlyFold (squareCopy,
				probHorzFold,
				probVertFold,
				probDiagRightFold,
				probDiagLeftFold);
			UnityHelper.RandomlyRotate (squareCopy.transform);
			UnityHelper.RandomlyPosition (squareCopy.transform, zone);
			foreach (Pocket p in squareCopy.pockets) 
			{
				PushPocket (p);
			}
			temp++;
		}
		temp = 0;


	}

    void Update()
    {
		// spawn a new origami unit over set intervals and only if there are enough pockets
		// on the field for a new unit to insert into
		if (TimeToSpawn() && pockets.Count > 0 && temp < unitsToGenerate)
        {
			InstantiateUnit();
        }
		// toRemove - remove inactive units after the frame
		// toDestroy - if the unit has been trying to insert itself into the target pocket for too long, kill it
		List<FourSquare> toRemove = new List<FourSquare>();
		List<FourSquare> toDestroy = new List<FourSquare>();
//		PrintValues (pockets);
//		print("num pockets " + pockets.Count);
		foreach (Tuple<int,FourSquare> tup in activeUnits)
        {
			
			FourSquare unit = tup.second;
//			print (unit.stage);
			// if pocket is no longer available, move the unit backwards first so it has room to move around
			// and move towards a potentially new target pocket
			if (!CheckPocketAvailable (tup) && !MovedAwayFromTarget(unit)) {
				continue;
			}
			// if unit has reached the end of its lifetime while trying to insert, destroy it
			if (EndLifetime(tup.first,unit.stage)) {
				toDestroy.Add (unit);
				// put pocket back onto the field
				PushPocket(unit.targetP);
				continue;
			} 
			tup.first++;

			// unit stages
			if (unit.stage == 0) {
				SetupFolds (tup, unit,toDestroy);

			} else if (unit.stage == 1) {
				AlignLookAtTarget (tup, unit);

			} else if (unit.stage == 2) {
				RotateAroundTarget (tup, unit);

			} else if (unit.stage == 3) {
				AlignRotationToTarget (tup, unit);

			} else if (unit.stage == 4) {
				InsertIntoPocket (tup, unit);

			}

			else
            {
				FillPocket (unit);
				// add new pockets to queue only if they are not filled by the parent
//				unit.pockets.ForEach( p => );
				foreach (Pocket p in unit.pockets) 
				{
					if (!p.Intersects (unit.targetP.pCenter.parent))
						PushPocket (p);
					else 
						p.filled = true;
				}
                toRemove.Add(unit);
            }


        }
		Tuple<int,FourSquare>[] copies = new Tuple<int,FourSquare>[activeUnits.Count];
		activeUnits.CopyTo (copies);
		foreach (Tuple<int,FourSquare> tup in copies) {
			if (toRemove.Contains (tup.second) || toDestroy.Contains (tup.second))
				activeUnits.Remove (tup);
		}

		foreach (FourSquare unit in toDestroy) {
//			print ("destroying " + unit.name);
			GameObject.Destroy (unit.gameObject);
		}

    }

	private bool MovedAwayFromTarget(FourSquare unit) {
		Vector3 distFromTarget = unit.iv.position - unit.targetP.pCenter.position;
//		print (distFromTarget.magnitude);
		if (distFromTarget.sqrMagnitude < 30.0f) {
			Vector3 pointToCenter = 3.0f * unit.GetAlignmentV3 ();
			unit.transform.position = Vector3.MoveTowards (unit.transform.position, unit.targetP.pCenter.position - pointToCenter, INSERTION_SPEED * Time.deltaTime);
			return false;
		}
		return true;
	}


	// if unit's target pocket is no longer available, try to find another pocket
	private bool CheckPocketAvailable(Tuple<int,FourSquare> tup) {
		FourSquare unit = tup.second;
		if (unit.targetP != null && unit.targetP.filled) {
//			print (unit.name + " target filled!");
			unit.ResetStage();
			return false;
		}
		return true;
	}

	private bool EndLifetime(int life, int stage) 
	{
		return life == UNIT_LIFETIME && stage >= 0 && stage < 3;
	}

	private void InstantiateUnit()
	{
		squareCopy = Instantiate(square, transform.position, transform.rotation) as FourSquare;
		squareCopy.name = "4Square " + temp;
		OrigamiFolder.RandomlyFold(squareCopy,
			probHorzFold,
			probVertFold,
			probDiagRightFold,
			probDiagLeftFold);
		UnityHelper.RandomlyRotate (squareCopy.transform);
		activeUnits.Add(new Tuple<int,FourSquare>(0,squareCopy));
		temp++;
	}

	private void SetupFolds(Tuple<int,FourSquare> tup,FourSquare unit, List<FourSquare> toDestroy) 
	{
		Pocket p;
		do {
			p = PopPocket ();
			unit.targetP = p;
		} while (p != null && p.filled);

		if (p == null) {
			toDestroy.Add (unit);
		} else {
			unit.ChooseInsertionVertice (probCenterInsertion);
//			print (unit.name + " has iv " + unit.iv.name);

			// if different angles (pocket and insertion point)
			// put pocket back in queue
			// don't continue and stay in same stage
			if (!UnityHelper.CanFitPocket (unit, unit.targetP)) {
//				print (unit.name + " put pocket back");
				PushPocket (unit.targetP);
				unit.targetP = null;
				unit.iv = null;
			} else {
				unit.CalcSelfRotationV3 ();
				unit.MoveToNextStage ();
				tup.first = 0;
//				print ("unit: " + unit.name + "\ntarget: " + unit.targetP.ToString ());
			}
		}
	}

	private void AlignLookAtTarget(Tuple<int,FourSquare> tup, FourSquare unit) {
		

		Vector3 currV3 = unit.GetAlignmentV3 ().normalized;

		Debug.DrawLine (unit.iv.position, unit.iv.position + unit.selfRotationV3, Color.cyan);
		Debug.DrawLine (unit.iv.position, unit.targetP.pCenter.position, Color.blue);

		Vector3 alignToV3 = (unit.targetP.pCenter.position - unit.iv.position).normalized;
		if (!UnityHelper.V3Equal (currV3, alignToV3)) {
			if ((currV3 - alignToV3).sqrMagnitude < 0.07f)
				unit.transform.RotateAround (unit.iv.position, unit.selfRotationV3, 0.2f * ROTATION_SPEED * Time.deltaTime);	
			else
				unit.transform.RotateAround (unit.iv.position, unit.selfRotationV3, ROTATION_SPEED * Time.deltaTime);
		} else {
			unit.CalcTargetRotationV3 ();
			unit.MoveToNextStage ();
			tup.first = 0;
//			print (unit.stage);
		}
	}

	private void RotateAroundTarget(Tuple<int,FourSquare> tup,FourSquare unit) {

		Vector3 normRotating = (unit.iv.position - unit.targetP.pCenter.position).normalized;
		Vector3 normStill = (unit.targetP.GetVectorIn ()).normalized;

		if (!UnityHelper.V3Equal (normRotating, normStill)) {
//			Debug.Log (normRotating + "\n" + normStill);
//			Debug.DrawLine (unit.targetP.pCenter.position, unit.targetP.pCenter.position + 3.0f*normStill, Color.blue);
//			Debug.DrawLine (unit.iv.position, unit.targetP.pCenter.position, Color.blue);
			if ((normRotating - normStill).sqrMagnitude < 0.07f) {
				unit.transform.RotateAround (unit.targetP.pCenter.position, unit.targetRotationV3, 0.2f * ROTATION_SPEED * Time.deltaTime);
			} else {
				unit.transform.RotateAround (unit.targetP.pCenter.position, unit.targetRotationV3, ROTATION_SPEED * Time.deltaTime);
			}
		} else {
			unit.MoveToNextStage ();
			tup.first = 0;
//			print (unit.stage);
		}
	}

	private void AlignRotationToTarget(Tuple<int,FourSquare> tup,FourSquare unit) {
		Vector3 v11 = unit.targetP.edge1.end.position - unit.ivNeighbor1.position;
		Vector3 v12 = unit.iv.position - unit.ivNeighbor1.position;
		Vector3 v21 = unit.targetP.edge2.end.position - unit.ivNeighbor2.position;
		Vector3 v22 = unit.iv.position - unit.ivNeighbor2.position;
		Plane plane1 = new Plane (unit.iv.position, unit.ivNeighbor1.position, unit.ivNeighbor2.position);
		Plane plane2 = unit.targetP.GetPocketPlane ();
//		print ("plane1 " + plane1.normal + "\nplane2 " + plane2.normal);
		if (!UnityHelper.V3ApproxEqual (plane1.normal, plane2.normal)) {
			if ((plane1.normal - plane2.normal).sqrMagnitude < 0.07f)
				unit.transform.RotateAround (unit.iv.position, unit.targetP.GetVectorIn (), 0.2f * ROTATION_SPEED * Time.deltaTime);	
			else
				unit.transform.RotateAround (unit.iv.position, unit.targetP.GetVectorIn (), ROTATION_SPEED * Time.deltaTime);
		} else {
			// before inserting into pocket, translate the unit so that provides a 3rd dimension to the structure
//			Vector3 unitToTarget = unit.center.position - unit.targetP.pCenter.position;
//			Vector3 ivToTarget = unit.iv.position - unit.targetP.pCenter.position;
//			float diff = unitToTarget.y - ivToTarget.y;
//			UnityHelper.SetV3Value(unit.transform,"y",unit.transform.position.y+2*diff);
//			print ("translated " + unit.name + " by " + diff);


			unit.MoveToNextStage ();
			tup.first = 0;
//			print (unit.stage);

		}
	}

	private void InsertIntoPocket(Tuple<int,FourSquare> tup,FourSquare unit) {
		
		Vector3 distFromTarget = unit.iv.position - unit.targetP.pCenter.position;
		if (distFromTarget.magnitude > STOP_DISTANCE) {
			Vector3 pointToCenter = unit.transform.position - unit.iv.position;
			unit.transform.position = Vector3.MoveTowards (unit.transform.position, unit.targetP.pCenter.position + pointToCenter, INSERTION_SPEED * Time.deltaTime);
		} else {
			unit.MoveToNextStage ();
			tup.first = 0;
		}
	}

	private void FillPocket(FourSquare unit) {
		// fill the target pocket and remove from list of pockets in the target unit
		unit.targetP.filled = true;
		FourSquare target = unit.targetP.pCenter.parent.GetComponent<FourSquare>();

		// check if any of the other target's pockets are filled by this unit
		// if so, mark it as filled
		target.pockets.ForEach (p => {
			if (p.Intersects (unit.transform))
				p.filled = true;
		});

		bool allFilled = true;
		target.pockets.ForEach (p => {
			allFilled &= p.filled;
		});
		// disable script of unit if it has no pockets
		if (allFilled) {
//			print (target.name + " has no more pockets");
			target.Disable ();
		}
	}


    public void PushPocket(Pocket p)
    {
        pockets.Enqueue(p);

    }

    public Pocket PopPocket()
    {
       if (pockets.Count > 0)
        {

            Pocket p = pockets.Dequeue();
            return p;
        }
    
        return null;

    }

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


	public static void PrintValues( IEnumerable myCollection )  {
		string output = "";
		foreach (System.Object obj in myCollection)
			output += " " + ((Pocket) obj).pCenter.parent.name;
		print (output);
	}

	
}
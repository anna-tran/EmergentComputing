using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	private static float ROTATION_SPEED = 80;
    private static int FPS = 30;
	private static int DELAY_SECONDS = 5 * FPS;
	private static float INSERTION_SPEED = 8.0f;
	private static int UNIT_LIFETIME = 20 * FPS;
	private static float STOP_DISTANCE = 0.6f;

	public FourSquare square;

    public int initUnits;
	public int unitsToGenerate;
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
		if (TimeToSpawn() && pockets.Count > 0 && temp < unitsToGenerate)
        {
			InstantiateUnit();
        }
		List<FourSquare> toRemove = new List<FourSquare>();
		List<FourSquare> toDestroy = new List<FourSquare>();
//		PrintValues (pockets);
		foreach (Tuple<int,FourSquare> tup in activeUnits)
        {
			
			FourSquare unit = tup.second;

			if (EndLifetime(tup.first,unit.stage)) {
				toDestroy.Add (unit);
				// put pocket back onto the field
				PushPocket(unit.targetP);
				continue;
			} 
			tup.first++;

			
			if (unit.stage == 0) {
				SetupFolds (tup, unit,toDestroy);

			} else if (unit.stage == 1) {
				AlignGroundRotation (tup, unit);

			} else if (unit.stage == 2) {
				RotateAroundTarget (tup, unit);

			} else if (unit.stage == 3) {
				AlignRotationToTarget (tup, unit);

			} else if (unit.stage == 4) {
				InsertIntoPocket (tup, unit);

			}
			else
            {
				foreach (Pocket p in unit.pockets) 
				{
					PushPocket (p);
				}
                toRemove.Add(unit);
            }

            //            unit.iv.LookAt(unit.targetP.pCenter);

//			if (unit.targetP != null ) {
//					
//	            Debug.DrawLine(unit.targetP.pCenter.position, 
//	                unit.targetP.pCenter.position - unit.targetRotationV3,
//	                Color.green);
//	            Debug.DrawLine(unit.iv.position, unit.targetP.pCenter.position, Color.cyan);
//			}
        }
		Tuple<int,FourSquare>[] copies = new Tuple<int,FourSquare>[activeUnits.Count];
		activeUnits.CopyTo (copies);
		foreach (Tuple<int,FourSquare> tup in copies) {
			if (toRemove.Contains (tup.second) || toDestroy.Contains (tup.second))
				activeUnits.Remove (tup);
		}
//		activeUnits.RemoveAll(tup => toRemove.Contains(tup.second));
//		activeUnits.RemoveAll(tup => toDestroy.Contains(tup.second));

		foreach (FourSquare unit in toDestroy) {
			GameObject.Destroy (unit.gameObject);
		}

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
		activeUnits.Add(new Tuple<int,FourSquare>(0,squareCopy));
		temp++;
	}

	private void SetupFolds(Tuple<int,FourSquare> tup,FourSquare unit, List<FourSquare> toDestroy) 
	{
		Pocket p = PopPocket ();
		unit.targetP = p;

		if (p == null) {
			toDestroy.Add (unit);
		} else {
			unit.ChooseInsertionVertice ();

			// if different angles (pocket and insertion point)
			// put pocket back in queue
			// don't continue and stay in same stage
			if (!UnityHelper.CanFitPocket (unit, unit.targetP)) {
				PushPocket (unit.targetP);
				unit.targetP = null;
			} else {

				// remove pockets of units that have been destroyed
				unit.CalcTargetRotationV3 ();
				unit.CalcSelfRotationV3 ();

				unit.MoveToNextStage ();
				tup.first = 0;
				print ("unit: " + unit.name + "\ntarget: " + unit.targetP.ToString ());
			}
		}
	}

	private void AlignGroundRotation(Tuple<int,FourSquare> tup, FourSquare unit) {
		Vector3 currV3 = unit.GetAlignmentV3 ();
		//				Debug.DrawLine (unit.iv.position, unit.iv.position + unit.selfRotationV3, Color.cyan);

		currV3.y = 0;
		currV3.Normalize ();
		Vector3 alignToV3 = unit.targetP.pCenter.position - unit.iv.position;
		alignToV3.y = 0;
		alignToV3.Normalize ();
		//				print (currV3 + "\n" + alignToV3);
		if (!UnityHelper.V3Equal (currV3, alignToV3)) {
			unit.transform.RotateAround (unit.iv.position, unit.selfRotationV3, ROTATION_SPEED * Time.deltaTime);
		} else {
			unit.MoveToNextStage ();
			tup.first = 0;
		}
	}

	private void RotateAroundTarget(Tuple<int,FourSquare> tup,FourSquare unit) {

		Vector3 normRotating = (unit.iv.position - unit.targetP.pCenter.position);
		normRotating.Normalize ();
		Vector3 normStill = (unit.targetP.GetVectorIn ());
		normStill.Normalize ();

		if (!UnityHelper.V3Equal (normRotating, normStill)) {
			//					print (normRotating + "\n" + normStill);
			unit.transform.RotateAround (unit.targetP.pCenter.position, unit.targetRotationV3, ROTATION_SPEED * Time.deltaTime);
		} else {
			unit.MoveToNextStage ();
			tup.first = 0;
		}
	}

	private void AlignRotationToTarget(Tuple<int,FourSquare> tup,FourSquare unit) {
		Vector3 acute1 = UnityHelper.acuteAngle (unit.transform.eulerAngles);
		Vector3 acute2 = UnityHelper.acuteAngle (unit.targetP.pCenter.eulerAngles);
		if (!unit.AlignedWithTarget ()) {
			unit.transform.RotateAround (unit.iv.position, unit.targetP.GetVectorIn (), ROTATION_SPEED * Time.deltaTime);                   
		} else {
			print("unit " + unit.name + "\niv " + unit.iv.name + "\nunit vector in " + unit.GetAlignmentV3() + "\ntarget vector in " + unit.targetP.GetVectorIn());
			unit.MoveToNextStage ();
			tup.first = 0;
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

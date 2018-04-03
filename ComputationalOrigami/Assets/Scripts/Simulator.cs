using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	private static float ROTATION_SPEED = 80;
    private static int FPS = 30;
    private static int DELAY_SECONDS = 7 * FPS;
	private static float INSERTION_SPEED = 7.0f;
	private static int UNIT_LIFETIME = 30 * FPS;


    public int initUnits;
	public FourSquare square;
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

    int temp;


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
			temp++;
		}


	}

    void Update()
    {
		if (TimeToSpawn() && pockets.Count > 0)
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
		List<FourSquare> toRemove = new List<FourSquare>();
		List<FourSquare> toDestroy = new List<FourSquare>();
		foreach (Tuple<int,FourSquare> tup in activeUnits)
        {
			
			FourSquare unit = tup.second;

			if (tup.first == UNIT_LIFETIME && stage >= 0 && stage < 3) {
				toDestroy.Add (unit);
				// put pocket back onto the field
				pockets.Enqueue (unit.targetP);
				continue;
			} else if (stage < 3) {
				tup.first++;
			}
			
			if (unit.stage == -1) {
				Pocket p = PopPocket ();
				unit.targetP = p;

				if (p == null) {
					toDestroy.Add (unit);
				} else {
					unit.ChooseInsertionVertice ();
					unit.CalcTargetRotationV3 ();
					unit.CalcSelfRotationV3 ();

					unit.MoveToNextStage ();
					tup.first = 0;
				}
			} else if (unit.stage == 0) {
				print ("from " + unit.transform.name + " to " + unit.targetP.pCenter.parent.name);

			
				Vector3 currV3 = unit.GetAlignmentV3 ();
				currV3.y = 0;
				currV3.Normalize ();
				Vector3 alignToV3 = unit.targetP.pCenter.position - unit.iv.position;
				alignToV3.y = 0;
				alignToV3.Normalize ();
				if (!UnityHelper.V3Equal (currV3, alignToV3)) {
					unit.transform.RotateAround (unit.iv.position, unit.selfRotationV3, ROTATION_SPEED * Time.deltaTime);
				} else {
					unit.MoveToNextStage ();
					tup.first = 0;
				}
			} else if (unit.stage == 1) {

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
			} else if (unit.stage == 2) {
				Vector3 acute1 = UnityHelper.acuteAngle (unit.transform.eulerAngles);
				Vector3 acute2 = UnityHelper.acuteAngle (unit.targetP.pCenter.eulerAngles);
				if (unit.AlignedWithTarget ()) {
					unit.MoveToNextStage ();
					tup.first = 0;
                    
				} else {
//					print (unit.targetP.GetVectorIn ());
					unit.transform.RotateAround (unit.iv.position, unit.targetP.GetVectorIn (), ROTATION_SPEED * Time.deltaTime);
				}
			} else if (unit.stage == 3) {
				Vector3 distFromTarget = unit.iv.position - unit.targetP.pCenter.position;
				if (distFromTarget.magnitude > 0.35f) {
					Vector3 pointToCenter = unit.transform.position - unit.iv.position;
					unit.transform.position = Vector3.MoveTowards (unit.transform.position, unit.targetP.pCenter.position + pointToCenter, INSERTION_SPEED * Time.deltaTime);
				} else {
					unit.MoveToNextStage ();
					tup.first = 0;
				}

			} else if (unit.stage == 4) {
				// add the fitted unit pockets to the field
				foreach (Pocket p in unit.pockets) {
					pockets.Enqueue (p);
					print ("adding pocket from " + unit.name);
				}
				tup.first = 0;
			}
			else
            {
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
		activeUnits.RemoveAll(tup => toRemove.Contains(tup.second));
		activeUnits.RemoveAll(tup => toDestroy.Contains(tup.second));

		foreach (FourSquare unit in toDestroy) {
			GameObject.Destroy (unit.gameObject);
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



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T1,T2> {

	public T1 first { get; set;}
	public T2 second { get; set; }

	internal Tuple(T1 fst, T2 snd) {
		first = fst;
		second = snd;
	}
}

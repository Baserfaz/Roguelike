using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour {

	private List<int> experienceRanges = new List<int>();

	public int currentExp = 0;
	public int currentLevel = 1;

	private int maxLevel = 25;
	private int baseExp = 100;
	private float expMagnifier = 1.25f;

	void Awake() { CalculateExpRanges(); }

	private void CalculateExpRanges() {
		int exp = 0;
		for(int i = 1; i <= maxLevel; i++) {
			exp = Mathf.FloorToInt(baseExp * expMagnifier * i);
			experienceRanges.Add(exp);
		}
	}

	public int GetLevelRequirementExp(int level) {
		level --;
		return experienceRanges[level];
	}


}

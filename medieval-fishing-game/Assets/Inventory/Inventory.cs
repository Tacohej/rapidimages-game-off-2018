using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : ScriptableObject {
	public UnityAction<RodItem> onSelectedRodChanged;
	public UnityAction<BaitItem> onSelectedBaitChanged;

	public bool test;

	[NonSerialized] private RodItem m_SelectedRod;
	[NonSerialized] private BaitItem m_SelectedBait;

	public RodItem SelectedRod  {
		get { return m_SelectedRod; }
		set {
			if (m_SelectedRod != value && onSelectedRodChanged != null) {
				onSelectedRodChanged.Invoke (value);
			}
			m_SelectedRod = value;
		}
	}
	public BaitItem SelectedBait {
		get { return m_SelectedBait; }
		set  {
			if (m_SelectedBait != value && onSelectedBaitChanged != null) {
				onSelectedBaitChanged.Invoke (value);
			}
			m_SelectedBait = value;
		}
	}
}
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;

public class PianoKeyController : MonoBehaviour
{
	[Title("References")]
	public MidiPlayer MidiPlayer;

	#region [------------	WE DON'T NEED FOR NOW A TRANSFORM PARENT AND PEDAL	------------]
	//	public Transform PianoKeysParent;
	//	public Transform SustainPedal;
	#endregion

	[BoxGroup("Notes", centerLabel: true)]
	[InfoBox("The notes are sorting auto :)")]
	[OnValueChanged(nameof(SortMusic))]
	[PropertySpace(5)]
	public AudioClip[] Notes;
	internal void SortMusic()
	{
#if UNITY_EDITOR
		Regex sortReg = new Regex(@Regex);
		Notes = Notes.OrderBy(note => sortReg.Match(note.name).Value).ToArray();
#endif
	}

	[BoxGroup("Propierties",centerLabel:true)]
	[PropertySpace(10)]
	public string StartKey = "A";           // If the first key is not "A", change it to the appropriate note.
	[ReadOnly]
	[BoxGroup("Propierties")]
	public int StartOctave;                 // Start Octave can be increased if the piano/keyboard is not full length. 
	[BoxGroup("Propierties")]
	public float PedalReleasedAngle;        // Local angle that a pedal is considered to be released, or off.
	[BoxGroup("Propierties")]
	public float PedalPressedAngle;         // Local angle that a pedal is considered to be pressed, or on.
	[BoxGroup("Propierties"),Range(0,8)]
	public float SustainSeconds = 5;        // May want to reduce this if there's too many AudioSources being generated per key.
	[BoxGroup("Propierties")]
	public float PressAngleThreshold = 355f;// Rate of keys being slowly released.
	[BoxGroup("Propierties")]
	public float PressAngleDecay = 1f;      // Rate of keys being slowly released.
	[BoxGroup("Propierties")]
	public bool NoMultiAudioSource;         // Will prevent duplicates if true, if you need to optimise. Multiple Audio sources are necessary to remove crackling.	

	[BoxGroup("Attributes", centerLabel: true)]
	public bool SustainPedalPressed = true; // When enabled, keys will not stop playing immediately after release.
	[BoxGroup("Attributes")]
	public bool KeyPressAngleDecay = true;  // When enabled, keys will slowly be released.
	[BoxGroup("Attributes")]
	public bool RepeatedKeyTeleport = true; // When enabled, during midi mode, a note played on a pressed key will force the rotation to reset.	
	[ReadOnly]
	[BoxGroup("Attributes")]
	public string Regex = "[0-9][0-9]-[a-z]-*[0-9]";

	private float _sustainPedalLerp = 1;

	#region [------------	WE DON'T NEED A PHYSICAL KEYNOTE	------------]
	//public KeyMode KeyMode => MidiPlayer ? MidiPlayer.KeyMode : KeyMode.Physical;
	#endregion

	#region [------------	ShowMIDIChannelColours (WE DON´T NEED SHOW COLORS)	------------]
	/*
	public bool ShowMIDIChannelColours => MidiPlayer ? MidiPlayer.ShowMIDIChannelColours : false;	
	*/
	#endregion

	[DictionaryDrawerSettings(KeyLabel = "Note Type", ValueLabel = "Value")]
	public Dictionary<string, NoteKey> PianoNotes = new Dictionary<string, NoteKey>() { };

	private readonly string[] _keyIndex = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

	#region [------------	SINGLETON	------------]

	private static PianoKeyController _Instance;
	public static PianoKeyController Instance
	{
		get => _Instance;		
	}

	#endregion

	void Awake ()
	{
		#region [------------	SINGLETON INITIALIZE	------------]

		if (_Instance != null && _Instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_Instance = this;
		}

        #endregion

		#region [------------	PIANO	------------]

		var count = 0;

		/*for (int i = 0; i < PianoKeysParent.childCount; i++)
		{
			AudioSource keyAudioSource = PianoKeysParent.GetChild(i).GetComponent<AudioSource>();
			
			if (keyAudioSource)
			{
				NoteKey pianoKey = PianoKeysParent.GetChild(i).GetComponent<NoteKey>();
				
				keyAudioSource.clip = Notes[count];
				PianoNotes.Add(KeyString(count + Array.IndexOf(_keyIndex, StartKey)), pianoKey);
				//pianoKey.PianoKeyController = this;
				
				count++;
			}
		}*/
		#endregion

	}

	void Update()
	{
		_sustainPedalLerp -= Time.deltaTime * (SustainPedalPressed ? 1 : -1) * 3.5f;
		_sustainPedalLerp = Mathf.Clamp01(_sustainPedalLerp);

		#region [------------	WE DON'T NEED MOVE THE PEDAL	------------] 
		/*	if (PedalPressedAngle > PedalReleasedAngle)
				SustainPedal.localRotation = Quaternion.Lerp(Quaternion.Euler(PedalReleasedAngle, 0, 0), Quaternion.Euler(PedalPressedAngle, 0, 0), _sustainPedalLerp);
			else
				SustainPedal.localRotation = Quaternion.Lerp(Quaternion.Euler(PedalPressedAngle, 0, 0), Quaternion.Euler(PedalReleasedAngle, 0, 0), _sustainPedalLerp);
		*/
		#endregion
	}

	// https://stackoverflow.com/a/228060
	string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse( charArray );
		return new string( charArray );
	}	

    string KeyString (int count) =>_keyIndex[count % 12] + (Mathf.Floor(count / 12) + StartOctave);
	
}
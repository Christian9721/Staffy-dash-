using System;
using UnityEngine;
using UnityEngine.Events;
using MayenCore;
using Sirenix.OdinInspector;

public class MidiPlayer : MonoBehaviour
{
	[Header("References")]
	public PianoKeyController PianoKeyDetector;

	[Header("Properties")]
	public float GlobalSpeed = 1;
	public RepeatType RepeatType;

	//public KeyMode KeyMode;	<-------- we don´t need this

    #region [------------	WE DON'T NEED COLORS	------------]
    //public bool ShowMIDIChannelColours;
    #endregion

	[Header("Ensure Song Name is filled for builds")]
	public MidiSong[] MIDISongs;

	//[HideInInspector]
	[Title("Note List", "The value of each note of the song")]
	public MidiNote[] MidiNotes;

	[SerializeField]
	public UnityEvent OnPlayTrack;

	private MidiFileInspector _midi;

	// Current Path of the files (midi).
	string _path;

	string[] _keyIndex;
	int _noteIndex = 0;
	int _midiIndex;
	float _timer = 0;
	[SerializeField, HideInInspector]
	bool _preset = false;

	void Start ()
	{
		PianoKeyDetector = PianoKeyController.Instance;
		OnPlayTrack = new UnityEvent();	// <----------	A ver si no es necesario al ser publico

        #region [------------	FOR NOW WE DON'T NEED THIS EVENT	------------]
        //OnPlayTrack.AddListener(delegate{FindObjectOfType<MusicText>().StartSequence(MIDISongs[_midiIndex].Details);});
        #endregion

        _midiIndex = 0;

		if (!_preset)
			PlayCurrentMIDI();
		else
		{
#if UNITY_EDITOR
			_path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[0].MIDIFile.name);
#else
			_path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[0].SongFileName);
#endif

			_midi = new MidiFileInspector(_path);

			#region [------------	EVENT MAY BE NULL	------------]
			//OnPlayTrack.Invoke();
			#endregion

			OnPlayTrack?.Invoke();	// This will never be null :)
		}
    }

	void Update ()
	{
		if (MIDISongs.Length <= 0)
			enabled = false;
		
		if (_midi != null && MidiNotes.Length > 0 && _noteIndex < MidiNotes.Length)
		{
			_timer += Time.deltaTime * GlobalSpeed * (float)MidiNotes[_noteIndex].Tempo;

			while (_noteIndex < MidiNotes.Length && MidiNotes[_noteIndex].StartTime < _timer)
			{
				if (PianoKeyDetector.PianoNotes.ContainsKey(MidiNotes[_noteIndex].Note))
				{
					#region [------------	WE DONT NEED PLAY WITH COLORS, ONLY THE NOTE	------------]
					/*
					if (ShowMIDIChannelColours)
					{
						PianoKeyDetector.PianoNotes[MidiNotes[_noteIndex].Note].Play(MIDIChannelColours[MidiNotes[_noteIndex].Channel],
																				MidiNotes[_noteIndex].Velocity, 
																				MidiNotes[_noteIndex].Length, 
																				PianoKeyDetector.MidiPlayer.GlobalSpeed * MIDISongs[_midiIndex].Speed);
					}
					else
						PianoKeyDetector.PianoNotes[MidiNotes[_noteIndex].Note].Play(MidiNotes[_noteIndex].Velocity, 
																				MidiNotes[_noteIndex].Length,
																				PianoKeyDetector.MidiPlayer.GlobalSpeed * MIDISongs[_midiIndex].Speed);
                    */
					#endregion

					PianoKeyDetector.PianoNotes[MidiNotes[_noteIndex].Note].Play(MidiNotes[_noteIndex].Velocity,
																				MidiNotes[_noteIndex].Length,
																				PianoKeyDetector.MidiPlayer.GlobalSpeed * MIDISongs[_midiIndex].Speed);
					
				}
                _noteIndex++;
			}
		}
		else
		{
			SetupNextMIDI();
		}
	}

	void SetupNextMIDI()
	{
		if (_midiIndex >= MIDISongs.Length - 1)
		{
			if (RepeatType != RepeatType.NoRepeat)
				_midiIndex = 0;
			else
			{
				_midi = null;
				return;
			}
		}
		else
		{
			if (RepeatType != RepeatType.RepeatOne)
				_midiIndex++;
		}

		PlayCurrentMIDI();
	}

	void PlayCurrentMIDI()
	{
		_timer = 0;

#if UNITY_EDITOR
		_path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[_midiIndex].MIDIFile.name);
#else
		_path = string.Format("Audio/{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[_midiIndex].SongFileName);
#endif
		_midi = new MidiFileInspector(_path);
		MidiNotes = _midi.GetNotes();
		_noteIndex = 0;

		OnPlayTrack.Invoke();
	}

	[ContextMenu("Preset MIDI")]
	void PresetFirstMIDI()
	{
#if UNITY_EDITOR
		_path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[0].MIDIFile.name);
#else
		_path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, MIDISongs[0].SongFileName);
#endif
		_midi = new MidiFileInspector(_path);
		MidiNotes = _midi.GetNotes();
		
		_preset = true;
	}

	[ContextMenu("Clear MIDI")]
	void ClearPresetMIDI()
	{
		MidiNotes = new MidiNote[0];
		_preset = false;
	}

#if UNITY_EDITOR
	[ContextMenu("MIDI to name")]
	public void MIDIToPlaylist()
	{
		for (int i = 0; i < MIDISongs.Length; i++)
		{
			MIDISongs[i].SongFileName = MIDISongs[i].MIDIFile.name;
		}
	}
#endif
}

public enum RepeatType { NoRepeat, RepeatLoop, RepeatOne }

#region [------------	WE DONT NEED A PHYSICAL PIANO KEY	------------]
//public enum KeyMode { Physical, ForShow }
#endregion

[Serializable]
public class MidiSong
{
#if UNITY_EDITOR
	[Space]
	[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
	public UnityEngine.Object MIDIFile;
#endif
	public string SongFileName;
	public float Speed = 1;
	[MultiLineProperty(5)]
	public string Details;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio.Midi;
using System.IO;
using System.Linq;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// Framework that provides a bunch of methods, enums and structs to make the staff game
/// </summary>
namespace MayenCore 
{
    public interface INote
    {

    }
    /// <summary>
    /// Treble types of the music staff.
    /// </summary>
    public enum ETrebleClef
    {
        /// <summary>SOL</summary>
        G,
        /// <summary>FA</summary>
        F,
        /// <summary>DO</summary>
        C
    }

    /// <summary>
    /// Type of notes depends of its duration time.
    /// </summary>
    public enum ENoteType
    {
        /// <summary>REDONDA</summary>
        whole,
        /// <summary>BLANCA CON PUNTILLO</summary>
        dotted_half,
        /// <summary>BLANCA</summary>
        half,
        /// <summary>NEGRA</summary>
        quarter,
        /// <summary>CORCHEA</summary>
        eighth,
        /// <summary>DOBLE CORCHEA</summary>
        sixteenth
    }

    /// <summary>
    /// The accidentals of the notes
    /// </summary>
    public enum EAccidental
    {
        /// <summary>SOSTENIDO #</summary>
        sharp,
        /// <summary>BEMOL b</summary>
        flat,
        /// <summary>BECUADRO</summary>
        natural,
        /// <summary>DOBLE SOSTENIDO x</summary>
        double_sharp,
        /// <summary>DOBLE BEMOL bb</summary>
        bouble_flat
    }
    
    /// <summary>
    /// The type of 'tempo' of the music sheet
    /// </summary>
    public static partial class Note
    {
        /// <summary>
        /// This returns the tempo of the sheet in format like : numerator/denominator
        /// Like '3/4', '4/4', 2/2
        /// </summary>
        /// <param name="note">The midi file</param>
        /// <returns></returns>
        public static (byte, byte) GetTimeSignature<T>(T midi) where T : MidiFileInspector
        {
            //aqui van matematicas :b
            //midi.MidiFile.Tracks;
            return (5,5);
        }
    }  
    
    /// <summary>
    /// This assign a type of note based in real music sheet. 
    /// </summary>    
    [SerializeField]
    public struct NoteData
    {
        // aqui vamos a usar diccionarios para los tipos de notas con sus sprites y mas


    }
	[Serializable]  //adding this to visualize
	public class MidiFileInspector
	{
		[SerializeField]
		public MidiFile MidiFile;

		[SerializeField]
		private List<TempoEvent> _tempoEvents = new List<TempoEvent>();

		[SerializeField]
		public MidiFileInspector(string fileName, bool strictCheck = false)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(string.Format("Midi file not found at {0}!", fileName));

			try
			{
				MidiFile = new MidiFile(fileName, strictCheck);
			}
			catch (FormatException formatEx)
			{
				throw formatEx;
			}
		}

		[SerializeField]
		public MidiNote[] GetNotes()
		{
			BuildTempoList();

			List<MidiNote> notes = new List<MidiNote>();

			double defaultTempo = 120;

			for (int n = 0; n < MidiFile.Tracks; n++)
			{
				foreach (MidiEvent midiEvent in MidiFile.Events[n])
				{
					try
					{
						var tempoEvent = (NAudio.Midi.TempoEvent)midiEvent;
						defaultTempo = tempoEvent.Tempo;
					}
					catch { }
				}
			}

			for (int n = 0; n < MidiFile.Tracks; n++)
			{
				foreach (MidiEvent midiEvent in MidiFile.Events[n])
				{
					if (MidiEvent.IsNoteOn(midiEvent))
					{
						try
						{
							var t_note = (NoteOnEvent)midiEvent;
							MidiNote noteOn = new MidiNote();

							noteOn.Note = t_note.NoteName;
							//noteOn.Channel = t_note.Channel;
							noteOn.Channel = n;
							noteOn.Velocity = t_note.Velocity;
							noteOn.StartTime = t_note.AbsoluteTime;

							if (_tempoEvents.Count > 0)
							{
								try
								{
									noteOn.Tempo = MidiFile.DeltaTicksPerQuarterNote * _tempoEvents.Last(a => a.AbsoluteTime <= t_note.AbsoluteTime).BPM / 60;
								}
								catch (Exception e)
								{
									noteOn.Tempo = MidiFile.DeltaTicksPerQuarterNote * defaultTempo / 60;
								}
							}
							else
							{
								noteOn.Tempo = MidiFile.DeltaTicksPerQuarterNote * defaultTempo / 60;
							}

							noteOn.Length = t_note.NoteLength / (float)noteOn.Tempo;

							notes.Add(noteOn);
						}
						catch (Exception formatEx)
						{
							throw formatEx;
						}
					}
				}
			}

			return SortNotes(notes).ToArray();
		}

		[SerializeField]
		public List<MidiNote> SortNotes(List<MidiNote> midiNotes)
		{
			while (true)
			{
				bool sorted = false;

				for (int i = 0; i < midiNotes.Count - 1; i++)
				{
					if (midiNotes[i].StartTime > midiNotes[i + 1].StartTime)
					{
						var x = midiNotes[i];
						var y = midiNotes[i + 1];

						midiNotes[i + 1] = x;
						midiNotes[i] = y;

						sorted = true;
					}
				}

				if (!sorted)
					break;
			}

			return midiNotes;
		}

		/// <summary>
		/// Builds the tempo list
		/// </summary>
		private void BuildTempoList()
		{
			var currentbpm = 120.00;
			var realtime = 0.0;
			var reldelta = 0;
			_tempoEvents = new List<TempoEvent>();

			foreach (var ev in MidiFile.Events[0])
			{
				reldelta += ev.DeltaTime;
				if (ev.CommandCode != MidiCommandCode.MetaEvent)
				{
					continue;
				}
				var tempo = (MetaEvent)ev;

				if (tempo.MetaEventType != MetaEventType.SetTempo)
				{
					continue;
				}
				var relativetime = (double)reldelta / MidiFile.DeltaTicksPerQuarterNote * (60000.0 / currentbpm);
				currentbpm = ((NAudio.Midi.TempoEvent)tempo).Tempo;
				realtime += relativetime;
				reldelta = 0;
				var tempo_event = new TempoEvent
				{
					AbsoluteTime = tempo.AbsoluteTime,
					RealTime = realtime,
					BPM = currentbpm
				};
				_tempoEvents.Add(tempo_event);
			}
		}

		/// <summary>
		/// Calculates the midi real time in seconds by given Absolute time.
		/// </summary>
		/// <param name="currentNoteAbsTime">The absolute note time which will be converted.</param>
		/// <returns></returns>
		private double GetRealtime(long currentNoteAbsTime)
		{
			var BPM = 120.0;   //As per the MIDI specification, until a tempo change is reached, 120BPM is assumed
			var reldelta = currentNoteAbsTime;   //The number of delta ticks between the delta time being converted and the tempo change immediately at or before it
			var time = 0.0;   //The real time position of the tempo change immediately at or before the delta time being converted
			foreach (var tempo in _tempoEvents.Where(tempo => tempo.AbsoluteTime <= currentNoteAbsTime))
			{
				BPM = tempo.BPM;
				time = tempo.RealTime;
				reldelta = currentNoteAbsTime - tempo.AbsoluteTime;
			}
			time += ((double)reldelta / MidiFile.DeltaTicksPerQuarterNote) * (60000.0 / BPM);
			return Math.Round(time / 1000.0, 5);
		}

		/// <summary>
		/// Helper class needed for storing tempo events
		/// with its Absolute time, Real time and BPM. 
		/// </summary>
		internal class TempoEvent
		{
			public long AbsoluteTime { get; set; }
			public double RealTime { get; set; }
			public double BPM { get; set; }
		}
	}

	[Serializable]
	public class MidiNote
	{

		public double StartTime;
		public int Channel;

		public string Note;
		public float Length;
		public int Velocity;
		public double Tempo;

		public MidiNote()
		{

		}

		[SerializeField]
		public MidiNote(double startTime, int channel, string note, int length, int velocity)
		{
			StartTime = startTime;
			Channel = channel;
			Note = note;
			Length = length;
			Velocity = velocity;
		}
	}
}
namespace MayenCore
{
    /// <summary>
    /// The term based in the loud of the note
    /// </summary>
    public enum ETerm
    {
        /// <summary>VERY SOFT</summary>
        pianissimo,
        /// <summary>SOFT</summary>
        piano,
        /// <summary>MODERATE SOFT</summary>
        mezzo_piano,
        /// <summary>SLIGHTLY SOFT</summary>
        mezzo_forte,
        /// <summary>LOUD</summary>
        forte,
        /// <summary>VERY LOUD</summary>
        fortissimo,
        /// <summary>LOUD THEN SOFT</summary>
        fortepiano,
        /// <summary>SUDDEN ACCENT</summary>
        sforzando,
    }
    public static partial class Note
    {
        public static string GetSymbolTerm(ETerm term)
        {
            string Symbol = null;
            switch (term)
            {
                case ETerm.pianissimo:
                    Symbol = "pp";
                    break;
                case ETerm.piano:
                    Symbol = "p";
                    break;
                case ETerm.mezzo_piano:
                    Symbol = "mp";
                    break;
                case ETerm.mezzo_forte:
                    Symbol = "mf";
                    break;
                case ETerm.forte:
                    Symbol = "f";
                    break;
                case ETerm.fortissimo:
                    Symbol = "ff";
                    break;
                case ETerm.fortepiano:
                    Symbol = "fp";
                    break;
                case ETerm.sforzando:
                    Symbol = "sfz";
                    break;
            }
            return Symbol;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio.Midi;

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


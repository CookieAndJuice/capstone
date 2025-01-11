using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperEx
{
    // Get a Sentis model in Assets/StreamingAssets
    public const string LogMelSpectroModelName = "LogMelSepctro.sentis"; // Log Mel Spectrogram model name
    public const string EncoderModelName = "AudioEncoder_Tiny.sentis"; // Audio encoder model name
    public const string DecoderModelName = "AudioDecoder_Tiny.sentis"; // Audio decoder model name
    public const string VocabName = "vocab.json"; // Vocabulary file name

    public const int MaxTokens = 100; // Maximum number of tokens
    public const int EndOfText = 50257; // Token indicating the end of text
    public const int StartOfTranscript = 50258; // Token indicating the start of the transcript
    public const int Transcribe = 50359; // Token for transcribing speech to text in a specific language
    public const int Translate = 50358; // Token for translating to English
    public const int NoTimeStamps = 50363; // Token for removing timestamps
    public const int StartTime = 50364; // Token indicating the start of timestamps

    public enum Language
    {
        English = 50259, // English
        Korean = 50264, // Korean
        Japanese = 50266 // Japanese
    }
}


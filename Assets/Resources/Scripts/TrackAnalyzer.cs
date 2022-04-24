using UnityEngine;

public class TrackAnalyzer : TrackPlayer {

    private float[] sampleSpectrum = new float[512];
    private float[] fband = new float[8];
    private float[] audioBand = new float[8];
    private float[] freqBandHighest = new float[8];

    public TrackAnalyzer(AudioSource newAudioSrc) : base(newAudioSrc) {
        this.audioSrc = newAudioSrc;
    }

    public void GenerateSpectrumData() {
        audioSrc.GetSpectrumData(sampleSpectrum, 0, FFTWindow.BlackmanHarris);
    }

    public float[] GetAudioBand() {
        return audioBand;
    }

    /* 
        22050/512 = 43 hertz per sample
        20 - 60 | 60 - 250 | 250 - 500 | 500 - 2000 | 2000 - 4000 | 4000 - 6000 | 6000 - 20000
        0: 2 = 86 Hz
        1: 4 = 172 Hz (87 - 258)
        2: 8 = 344 (259 - 602)
        3: 16 = 688 (603 - 1290)
        4: 32 = 1376 (1291 - 2666)
        5: 64 = 2752 (2667 - 5418)
        6: 128 = 5504 (5419 - 10922)
        7: 256 = 11008 (10923 - 21930)
    */
    public void CreateFrequencyBands() {
    int count = 0;
        for (int i = 0; i < 8; i++) {
            float avg = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7) sampleCount += 2;
            for (int j = 0; j < sampleCount; j++){
                avg += sampleSpectrum[count];
                count++;
            }
            avg /= count;
            fband[i] = avg * 10;
        }
    }

    public void CreateAudioBand() {
        for (int i = 0; i < 8; i++) {
            if (fband[i] > freqBandHighest[i]) freqBandHighest[i] = fband[i];
            audioBand[i] = (fband[i] / freqBandHighest[i]);
        }
    }

}

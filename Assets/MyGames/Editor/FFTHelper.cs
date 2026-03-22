using UnityEngine;
using System;

public static class FFTHelper
{
    private struct Complex
    {
        public double real;
        public double imag;
        public Complex(double r, double i) { real = r; imag = i; }
    }

    public static float[] GetSpectrum(float[] samples)
    {
        int n = samples.Length;
        Complex[] buffer = new Complex[n];
        for (int i = 0; i < n; i++)
        {
            buffer[i] = new Complex(samples[i], 0);
        }

        ComputeFFT(buffer);
        float[] spectrum = new float[n / 2];
        for (int i = 0; i < n / 2; i++)
        {
            // y•ÏX‰ÓŠzÅŒã‚É "/ n" ‚ð’Ç‰Á‚µ‚Ä”’l‚ð³‹K‰»‚·‚é
            spectrum[i] = (float)Math.Sqrt(buffer[i].real * buffer[i].real + buffer[i].imag * buffer[i].imag) / n;
        }
        return spectrum;
    }
    private static void ComputeFFT(Complex[] buffer)
    {
        int n = buffer.Length;
        if (n <= 1) return;

        Complex[] even = new Complex[n / 2];
        Complex[] odd = new Complex[n / 2];

        for (int i = 0; i < n / 2; i++)
        {
            even[i] = buffer[2 * i];
            odd[i] = buffer[2 * i + 1];
        }

        ComputeFFT(even);
        ComputeFFT(odd);

        for (int k = 0; k < n / 2; k++)
        {
            double angle = -2 * Math.PI * k / n;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            Complex t = new Complex(
                cos * odd[k].real - sin * odd[k].imag,
                sin * odd[k].real + cos * odd[k].imag
            );

            buffer[k] = new Complex(even[k].real + t.real, even[k].imag + t.imag);
            buffer[k + n / 2] = new Complex(even[k].real - t.real, even[k].imag - t.imag);
        }
    }
}

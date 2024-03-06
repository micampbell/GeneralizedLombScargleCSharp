# Generalized Lomb-Scargle Periodogram
This is a C# implementation of the Generalized Lomb-Scargle Periodogram

It is based on the approach by Zechmeister and Kuerster (2009).
https://www.aanda.org/articles/aa/pdf/2009/11/aa11296-08.pdf
and their repository at https://github.com/mzechmeister/GLS

The main routine is not static but requires to constructor to set up the frequencies to sample in advance. This is by design since one can call on many signals without having to re-establish the frequencies to sample. 

The main routine is CalculatePowers which return an array of the power spectrum - the same length as the frequencies.
In addition, there is another public function to get the data on the harmonic with the greatest power: GetLargestHarmonic. This returns the frequency, amplitude, phase shift, and vertical offset of the signal.

Three examples are provided and can be called from a console project or a WPF project which produces a plot of relevant data.

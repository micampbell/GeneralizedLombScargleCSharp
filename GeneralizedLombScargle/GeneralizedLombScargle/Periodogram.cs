namespace GeneralizedLombScargle
{
    /// <summary>
    /// This is ported from the C code in the Generalized Lomb-Scargle paper by Zechmeister and Kuerster (2009).
    /// https://www.aanda.org/articles/aa/pdf/2009/11/aa11296-08.pdf
    /// https://github.com/mzechmeister/GLS
    /// </summary>

    public class Periodogram
    {
        /// <summary>
        /// Frequences as an array of doubles. These are the frequencies at which the power spectrum is calculated.
        /// It is set in the constructor and cannot be changed. The idea is that multiple calls to CalculatePowers
        /// can be completed with the same frequencies.
        /// </summary>
        public double[] Frequencies { get; }

        /// <summary>
        /// The number of discrete frequencies in the Frequencies array.
        /// </summary>
        public int NumberOfFrequencies { get; }

        /// <summary>
        /// The uniform distance between the frequencies in the Frequencies array.
        /// </summary>
        private readonly double frequencyStep;

        /// <summary>
        /// Constructor for the Generalized Lomb-Scargle algorithm.
        /// </summary>
        /// <param name="startFrequency">Frequency to start the analysis</param>
        /// <param name="endFrequency">Frequency to end the analysis</param>
        /// <param name="frequencyStepSize">The step size of the frequency to sample at. If number of steps is also provided, then this will be ignored</param>
        /// <param name="numberOfFrequencySteps">The number of frequency steps to take. This is optional is frequencyStepSize is provided.</param>
        public Periodogram(double startFrequency, double endFrequency, double frequencyStepSize = double.NaN, int numberOfFrequencySteps = -1)
        {
            if (double.IsNaN(frequencyStepSize) && numberOfFrequencySteps <1)
                throw new ArgumentException("Either frequencyStepSize or numberOfFrequencySteps must be provided.");
            if (numberOfFrequencySteps >= 1)
            {
                NumberOfFrequencies = numberOfFrequencySteps;
                Frequencies = new double[NumberOfFrequencies];
                // the minus 1 is because the number of steps is the number of fence posts, not the number of intervals
                // e.g. if given 0 start, 10 end, and 11 steps, then the interval should be 1
                frequencyStep = (endFrequency - startFrequency) / (NumberOfFrequencies - 1);
            }
            else // frequencyStepSize is provided
            {
                frequencyStep = frequencyStepSize;
                var freqRange = endFrequency - startFrequency;
                NumberOfFrequencies = (int)(freqRange / frequencyStepSize);
                var lastFencePostExists = (freqRange % frequencyStepSize) / frequencyStepSize < 0.001;
                if (lastFencePostExists) // if the last fence post is at the endFrequency
                    NumberOfFrequencies++;
            }
            Frequencies = new double[NumberOfFrequencies];
            for (int i = 0; i < NumberOfFrequencies; i++)
            {
                Frequencies[i] = startFrequency + i * frequencyStep;
            }
        }

        /// <summary>
        /// Calulate the power spectrum of the data over the frequencies given in the constructor.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="values"></param>
        /// <param name="uncertainties">Uncertainties is optional. If provided it must be the same length as other
        /// two arguments and must not contain any zeros. These are essentially the sigma's or standard deviations for each point.</param>
        /// <returns></returns>
        public double[] CalculatePowers(IEnumerable<double> times, IEnumerable<double> values, IEnumerable<double> uncertainties = null)
        {
            var timesArray = times as IList<double> ?? times.ToArray();
            var valuesArray = values as IList<double> ?? values.ToArray();
            var n = valuesArray.Count;

            SetWeightsFromUncertainties(uncertainties, n, out double[] w, out double wSum);
            CalculateFrequenceIndependentTerms(timesArray, valuesArray, n, w, wSum,
                out double YY, out double[] wy, out double[] cosdx, out double[] sindx);

            // main loop
            int k = 0;
            bestPower = 0.0;
            var powers = new double[NumberOfFrequencies];
            double[] cosx = new double[n];
            double[] sinx = new double[n];
            foreach (var f in Frequencies)
            {
                var C = 0.0; // The paper defines constants C is sum of the cosines
                var S = 0.0; // S is sum of the sines
                var YC = 0.0;
                var YS = 0.0;
                var CC = 0.0; // cosine squared
                var CS = 0.0; // cosine times sine

                for (int i = 0; i < n; i++)
                {
                    if (k % 1000 == 0)
                    {
                        // init/refresh recurrences to stop error propagation 
                        cosx[i] = Math.Cos(Math.Tau * f * timesArray[i]);
                        sinx[i] = Math.Sin(Math.Tau * f * timesArray[i]);
                    }

                    C += w[i] * cosx[i];      // Eq. (8) 
                    
                    S += w[i] * sinx[i];     // Eq. (9) 
                    YC += wy[i] * cosx[i];       // Eq. (11) 
                    YS += wy[i] * sinx[i];         // Eq. (12) 
                  
                    CC += w[i] * cosx[i] * cosx[i];  // Eq. (13) 
                    CS += w[i] * cosx[i] * sinx[i];   // Eq. (15) 

                    // increase freq for next loop 
                    var tmp = cosx[i] * cosdx[i] - sinx[i] * sindx[i];
                    sinx[i] = cosx[i] * sindx[i] + sinx[i] * cosdx[i];
                    cosx[i] = tmp;
                }

                var SS = 1.0 - CC;
                CC -= C * C;          // Eq. (13) 
                SS -= S * S;         // Eq. (14) 
                CS -= C * S;          // Eq. (15) 
                var D = CC * SS - CS * CS;      // Eq. (6) 
                var power = (SS * YC * YC + CC * YS * YS - 2 * CS * YC * YS) / (YY * D); // Eq. (5)
                powers[k++] = power;
                if (bestPower < power)
                {
                    // keep track of highest power in case the user wants to know details on the highest frequency
                    bestPower = power;
                    bestFreq = f;
                    bestC = C;
                    bestS = S;
                    bestYC = YC;
                    bestSS = SS;
                    bestYS = YS;
                    bestCC = CC;
                    bestCS = CS;
                }
            }
            return powers;
        }
        double bestPower, bestC, bestS, bestYC, bestSS, bestYS, bestCC, bestCS, Y, bestFreq;

        /// <summary>
        /// Following the calculation of the power spectrum, this method can be called to get the details of 
        /// the frquency with the highest power.
        /// </summary>
        /// <param name="frequency">in cycles per second (Hz)</param>
        /// <param name="amplitude">dimensionless length of input data</param>
        /// <param name="phase">radians in the start phase</param>
        /// <param name="offset">dimensionless length to the center of the signal</param>
        /// <returns>The normalized power (zero to 1) of this frequency. If it is low (e.g. ~0.5), then its mostly noise.
        /// If greater than 0.99 then nearly pure signal.</returns>
        public double GetLargestHarmonic(out double frequency, out double amplitude, out double phase,
            out double offset)
        {
            var D = bestCC * bestSS - bestCS * bestCS;      // Eq. (6) 

            var a = (bestYC * bestSS - bestYS * bestCS) / D;     // Eq. (A.4) 
            var b = (bestYS * bestCC - bestYC * bestCS) / D;

            amplitude = Math.Sqrt(a * a + b * b);
            frequency = bestFreq;
            phase = Math.Atan2(a,b);
            offset = Y - a * bestC - b * bestS; // parenthetical equation between A.3 and A.4

            return bestPower;
        }

        private void CalculateFrequenceIndependentTerms(IList<double> timesArray, IList<double> valuesArray, int n, double[] w, double wSum, out double YY, out double[] wy, out double[] cosdx, out double[] sindx)
        {
            Y = 0.0;
            for (int i = 0; i < n; i++)
            {
                // mean 
                w[i] /= wSum;                 // normalised weights 
                Y += w[i] * valuesArray[i];             // Eq. (7) 
            }
            YY = 0.0;
            wy = new double[n];
            cosdx = new double[n];
            sindx = new double[n];
            for (int i = 0; i < n; i++)
            {
                var time = timesArray[i];
                // variance 
                wy[i] = valuesArray[i] - Y;     // Subtract weighted mean 
                YY += w[i] * wy[i] * wy[i];   // Eq. (10) 
                wy[i] *= w[i];      // attach weights 

                // Prepare trigonometric recurrences cos(dx)+i sin(dx) 
                cosdx[i] = Math.Cos(Math.Tau * frequencyStep * time);
                sindx[i] = Math.Sin(Math.Tau * frequencyStep * time);
            }
        }

        private static void SetWeightsFromUncertainties(IEnumerable<double> uncertainties, int n, out double[] w, out double wSum)
        {
            w = new double[n];
            wSum = 0.0;
            int i = 0;
            if (uncertainties == null)
            {
                for (i = 0; i < n; i++)
                    w[i] = 1;
                wSum = n;
                return;
            }

            foreach (var errorInY in uncertainties)
            {
                var errSqd = 1.0 / errorInY * errorInY;
                w[i++] = errSqd;
                wSum += errSqd;
            }
        }
    }
}
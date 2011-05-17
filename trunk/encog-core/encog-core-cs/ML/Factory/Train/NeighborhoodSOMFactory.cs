using System;
using System.Collections.Generic;
using Encog.MathUtil.RBF;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.SVM;
using Encog.ML.Train;
using Encog.Neural.SOM;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// Train an SOM network with a neighborhood method.
    /// </summary>
    ///
    public class NeighborhoodSOMFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new EncogError(
                    "Neighborhood training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.7d);
            String neighborhoodStr = holder.GetString(
                MLTrainFactory.PROPERTY_NEIGHBORHOOD, false, "rbf");
            String rbfTypeStr = holder.GetString(
                MLTrainFactory.PROPERTY_RBF_TYPE, false, "gaussian");

            RBFEnum t;

            if (rbfTypeStr.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfTypeStr.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfTypeStr.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfTypeStr.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            INeighborhoodFunction nf = null;

            if (neighborhoodStr.Equals("bubble", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodBubble(1);
            }
            else if (neighborhoodStr.Equals("rbf", StringComparison.InvariantCultureIgnoreCase))
            {
                String str = holder.GetString(
                    MLTrainFactory.PROPERTY_DIMENSIONS, true, null);
                int[] size = NumberList.FromListInt(CSVFormat.EG_FORMAT, str);
                nf = new NeighborhoodRBF(size, t);
            }
            else if (neighborhoodStr.Equals("rbf1d", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodRBF1D(t);
            }
            if (neighborhoodStr.Equals("single", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodSingle();
            }

            var result = new BasicTrainSOM((SOMNetwork) method,
                                           learningRate, training, nf);

            if (args.ContainsKey(MLTrainFactory.PROPERTY_ITERATIONS))
            {
                int plannedIterations = holder.GetInt(
                    MLTrainFactory.PROPERTY_ITERATIONS, false, 1000);
                double startRate = holder.GetDouble(
                    MLTrainFactory.PROPERTY_START_LEARNING_RATE, false, 0.05d);
                double endRate = holder.GetDouble(
                    MLTrainFactory.PROPERTY_END_LEARNING_RATE, false, 0.05d);
                double startRadius = holder.GetDouble(
                    MLTrainFactory.PROPERTY_START_RADIUS, false, 10);
                double endRadius = holder.GetDouble(
                    MLTrainFactory.PROPERTY_END_RADIUS, false, 1);
                result.SetAutoDecay(plannedIterations, startRate, endRate,
                                    startRadius, endRadius);
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Analyze
{
    /// <summary>
    /// This class represents a field that the Encog Analyst is in the process of
    /// analyzing. This class is used to track statistical information on the field
    /// that will help the Encog analyst determine what type of field this is, and
    /// how to normalize it.
    /// </summary>
    ///
    public class AnalyzedField : DataField
    {
        /// <summary>
        /// A mapping between the class names that the class items.
        /// </summary>
        ///
        private readonly IDictionary<String, AnalystClassItem> classMap;

        /// <summary>
        /// The analyst script that the results are saved to.
        /// </summary>
        ///
        private readonly AnalystScript script;

        /// <summary>
        /// The total for standard deviation calculation.
        /// </summary>
        ///
        private double devTotal;

        /// <summary>
        /// The number of instances of this field.
        /// </summary>
        ///
        private int instances;

        /// <summary>
        /// Tge sum of all values of this field.
        /// </summary>
        ///
        private double total;

        /// <summary>
        /// Construct an analyzed field.
        /// </summary>
        ///
        /// <param name="theScript">The script being analyzed.</param>
        /// <param name="name">The name of the field.</param>
        public AnalyzedField(AnalystScript theScript, String name) : base(name)
        {
            classMap = new Dictionary<String, AnalystClassItem>();
            instances = 0;
            script = theScript;
        }

        /// <summary>
        /// Get the class members.
        /// </summary>
        public IList<AnalystClassItem> AnalyzedClassMembers
        {
            get
            {
                var sorted = new List<String>();
                foreach (string item in classMap.Keys)
                {
                    sorted.Add(item);
                }

                sorted.Sort();

                IList<AnalystClassItem> result = new List<AnalystClassItem>();

                foreach (String str  in  sorted)
                {
                    result.Add(classMap[str]);
                }

                return result;
            }
        }

        /// <summary>
        /// Perform a pass one analysis of this field.
        /// </summary>
        ///
        /// <param name="str">The current value.</param>
        public void Analyze1(String str)
        {
            bool accountedFor = false;

            if (str.Trim().Length == 0 || str.Equals("?"))
            {
                Complete = false;
                return;
            }

            instances++;

            if (Real)
            {
                try
                {
                    double d = CSVFormat.EG_FORMAT.Parse(str);
                    Max = Math.Max(d, Max);
                    Min = Math.Min(d, Min);
                    total += d;
                    accountedFor = true;
                }
                catch (FormatException )
                {
                    Real = false;
                    if (!Integer)
                    {
                        Max = 0;
                        Min = 0;
                        StandardDeviation = 0;
                    }
                }
            }

            if (Integer)
            {
                try
                {
                    int i = Int32.Parse(str);
                    Max = Math.Max(i, Max);
                    Min = Math.Min(i, Min);
                    if (!accountedFor)
                    {
                        total += i;
                    }
                }
                catch (FormatException )
                {
                    Integer = false;
                    if (!Real)
                    {
                        Max = 0;
                        Min = 0;
                        StandardDeviation = 0;
                    }
                }
            }

            if (Class)
            {
                AnalystClassItem item;

                // is this a new class?
                if (!classMap.ContainsKey(str))
                {
                    item = new AnalystClassItem(str, str, 1);
                    classMap[str] = item;

                    // do we have too many different classes?
                    int max = script.Properties.GetPropertyInt(
                        ScriptProperties.SETUP_CONFIG_MAX_CLASS_COUNT);
                    if (classMap.Count > max)
                    {
                        Class = false;
                    }
                }
                else
                {
                    item = classMap[str];
                    item.IncreaseCount();
                }
            }
        }

        /// <summary>
        /// Perform a pass two analysis of this field.
        /// </summary>
        ///
        /// <param name="str">The current value.</param>
        public void Analyze2(String str)
        {
            if (str.Trim().Length == 0)
            {
                return;
            }

            if (Real || Integer)
            {
                if (!str.Equals("") && !str.Equals("?"))
                {
                    double d = CSVFormat.EG_FORMAT.Parse(str);
                    devTotal += Math.Pow((d - Mean), 2);
                }
            }
        }

        /// <summary>
        /// Complete pass 1.
        /// </summary>
        ///
        public void CompletePass1()
        {
            devTotal = 0;

            if (instances == 0)
            {
                Mean = 0;
            }
            else
            {
                Mean = total/instances;
            }
        }

        /// <summary>
        /// Complete pass 2.
        /// </summary>
        ///
        public void CompletePass2()
        {
            StandardDeviation = Math.Sqrt(devTotal/instances);
        }

        /// <summary>
        /// Finalize the field, and create a DataField.
        /// </summary>
        ///
        /// <returns>The new DataField.</returns>
        public DataField FinalizeField()
        {
            var result = new DataField(Name);

            result.Name = Name;
            result.Min = Min;
            result.Max = Max;
            result.Mean = Mean;
            result.StandardDeviation = StandardDeviation;
            result.Integer = Integer;
            result.Real = Real;
            result.Class = Class;
            result.Complete = Complete;

            result.ClassMembers.Clear();

            if (result.Class)
            {
                IList<AnalystClassItem> list = AnalyzedClassMembers;
                foreach (AnalystClassItem item in list)
                {
                    result.ClassMembers.Add(item);
                }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" total=");
            result.Append(total);
            result.Append(", instances=");
            result.Append(instances);
            result.Append("]");
            return result.ToString();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Fuzzy.Sets;
using Fuzzy.Terms;

namespace Fuzzy
{
    public class Variable
    {
        private Dictionary<string, FuzzySet> categories;
        private double minRange;
        private double maxRange;

        public Variable()
        {
            this.categories = new Dictionary<string, FuzzySet>();
        }

        public SetProxy AddTriangleSet(string name, double minBound, double peakPoint, double maxBound)
        {
            this.categories.Add(name, new TriangleSet(peakPoint, peakPoint - minBound, maxBound - peakPoint));
            AdjustRange(minBound, maxBound);

            return new SetProxy(this.categories[name]);
        }

        public SetProxy AddRightShoulder(string name, double minBound, double peakPoint, double maxBound)
        {
            this.categories.Add(name, new RightShoulder(peakPoint, peakPoint - minBound));
            AdjustRange(minBound, maxBound);

            return new SetProxy(this.categories[name]);
        }

        public SetProxy AddLeftShoulder(string name, double minBound, double peakPoint, double maxBound)
        {
            this.categories.Add(name, new LeftShoulder(peakPoint, maxBound - peakPoint));
            AdjustRange(minBound, maxBound);

            return new SetProxy(this.categories[name]);
        }

        public void Fuzzify(double value)
        {
            foreach (FuzzySet set in this.categories.Values) { set.DegreeOfMembership = set.CalculateMembership(value); }
        }

        public IEnumerable<(string name, double membership)> Memberships => this.categories.Select(pair => (pair.Key, pair.Value.DegreeOfMembership));

        public double DefuzzifyMaxAverage()
        {
            double numerator = 0.0;
            double denominator = 0.0;

            foreach (FuzzySet category in this.categories.Values)
            {
                numerator += category.RepresentativeValue * category.DegreeOfMembership;
                denominator += category.DegreeOfMembership;
            }

            // Assure you're not dividing by 0
            return denominator > 0.0 ? numerator / denominator : 0.0;
        }

        public double DefuzzifyCentroid(int sampleCount)
        {
            double range = this.maxRange - this.minRange;
            double stepSize = range / sampleCount;

            double numerator = 0.0;
            double denominator = 0.0;

            double sampleValue = this.minRange;

            double TruncateMembership(FuzzySet category, double value)
            {
                double currentMembership = category.DegreeOfMembership;
                double newMembership = category.CalculateMembership(value);

                return currentMembership < newMembership ? currentMembership : newMembership;
            }

            for (int i = 0; i < sampleCount; i++)
            {
                sampleValue += stepSize;
                double membershipSum = this.categories.Values.Select(category => TruncateMembership(category, sampleValue)).Sum();

                numerator += membershipSum * sampleValue;
                denominator += membershipSum;
            }

            // Assure you're not dividing by 0
            return denominator > 0.0 ? numerator / denominator : 0.0;
        }

        private void AdjustRange(double newMinRange, double newMaxRange)
        {
            if (newMinRange < this.minRange) { this.minRange = newMinRange; }

            if (newMaxRange > this.maxRange) { this.maxRange = newMaxRange; }
        }
    }
}
using System;
namespace Greet.UnitLib3
{
    public interface IUnit
    {
        /// <summary>
        /// OLD Abbrev equivalent
        /// </summary>
        string Abbrev { get; set; }
        string BaseGroupName { get; set; }
        string Expression { get; }
        double FromSI(double val);
        string Name { get; }
        double Si_intercept { get; }
        double Si_slope { get; }
        double ToSI(double val);
    }
}

﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerAlgebra;

namespace Circuit
{
    /// <summary>
    /// Ideal voltage source.
    /// </summary>
    [Category("Standard")]
    [DisplayName("Voltage Source")]
    [DefaultProperty("Voltage")]
    [Description("Ideal voltage source.")]
    public class VoltageSource : TwoTerminal
    {
        /// <summary>
        /// Expression for voltage V.
        /// </summary>
        private Quantity voltage = new Quantity(Call.Sin(100 * 2 * 3.14 * t), Units.V);
        [Description("Voltage generated by this voltage source.")]
        [Serialize]
        public Quantity Voltage { get { return voltage; } set { if (voltage.Set(value)) NotifyChanged("Voltage"); } }
        
        public VoltageSource() { Name = "V1"; }

        public static void Analyze(Analysis Mna, string Name, Node Anode, Node Cathode, Expression V)
        {
            // Unknown current.
            Mna.AddPassiveComponent(Name, Anode, Cathode, Name.Length > 0 ? Mna.AddNewUnknown("i" + Name) : Mna.AddNewUnknown());
            // Set the voltage.
            Mna.AddEquation(Anode.V - Cathode.V, V);
            // Add initial conditions, if necessary.
            Expression V0 = V.Evaluate(t, 0);
            if (!(V0 is Constant))
                Mna.AddInitialConditions(Arrow.New(V0, 0));
        }

        public override void Analyze(Analysis Mna)
        {
            Analyze(Mna, Name, Anode, Cathode, Voltage);
        }

        public override void LayoutSymbol(SymbolLayout Sym)
        {
            base.LayoutSymbol(Sym);

            int r = 10;

            Sym.AddWire(Anode, new Coord(0, r));
            Sym.AddWire(Cathode, new Coord(0, -r));

            Sym.AddCircle(EdgeType.Black, new Coord(0, 0), r);
            Sym.DrawPositive(EdgeType.Black, new Coord(0, 7));
            Sym.DrawNegative(EdgeType.Black, new Coord(0, -7));
            if (!(Voltage.Value is Constant))
                Sym.DrawFunction(
                    EdgeType.Black,
                    (t) => t * r * 0.75,
                    (t) => Math.Sin(t * 3.1415) * r * 0.5, -1, 1);

            Sym.DrawText(() => Voltage.ToString(), new Point(r * 0.7, r * 0.7), Alignment.Near, Alignment.Near);
            Sym.DrawText(() => Name, new Point(r * 0.7, r * -0.7), Alignment.Near, Alignment.Far);
        }
    }
}


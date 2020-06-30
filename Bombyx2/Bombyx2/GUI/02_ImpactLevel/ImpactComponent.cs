﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel
{
    public class ImpactComponent : GH_Component
    {
        public ImpactComponent()
          : base("Component impact",
                 "Component impact",
                 "Calculates impacts of PE, GWP, UBP",
                 "Bombyx 2",
                 "Impacts")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Layer properties", "Layer\nproperties", "List of layer properties", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Reference study period", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference service life", "RSL (years)", "Reference service life", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Component properties (text)", "Component properties (text)", "Component properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Component properties (values)", "Component properties (values)", "Component properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var layer = new List<double>();
            if (!DA.GetDataList(0, layer)) { return; }
            var RSP = 0;
            if (!DA.GetData(1, ref RSP)) { return; }
            var RSL = 0;
            if (!DA.GetData(2, ref RSL)) { return; }

            var valueSets = layer.Select((x, i) => new { Index = i, Value = x })
                                 .GroupBy(x => x.Index / 12)
                                 .Select(x => x.Select(v => v.Value).ToList())
                                 .ToList();

            double repNum = 0;
            double tmp = ((double)RSP / (double)RSL) - 1;
            if (RSL != 0 && RSP != 0)
            {
                repNum = Math.Ceiling(tmp);
            }
            if (repNum < 0)
            {
                repNum = 0;
            }
            if (repNum == 0)
            {
                repNum = 0;
            }

            var results = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", 0 },
                { "UBP13 Replacements (P/m\xB2)", 0 },
                { "UBP13 End of Life (P/m\xB2)", 0 },
                { "PE Total Embodied (kWh oil-eq)", 0 },
                { "PE Total Replacements (kWh oil-eq)", 0 },
                { "PE Total End of Life (kWh oil-eq)", 0 },
                { "PE Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Renewable End of Life (kWh oil-eq)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", 0 },
                { "R value", 0 }
            };

            foreach (var item in valueSets)
            {
                results["UBP13 Embodied (P/m\xB2)"] += item[1];
                results["UBP13 Replacements (P/m\xB2)"] += ((item[1] + item[2]) * repNum);
                results["UBP13 End of Life (P/m\xB2)"] += item[2];
                results["PE Total Embodied (kWh oil-eq)"] += item[3];
                results["PE Total Replacements (kWh oil-eq)"] += ((item[3] + item[4]) * repNum);
                results["PE Total End of Life (kWh oil-eq)"] += item[4];
                results["PE Renewable Embodied (kWh oil-eq)"] += item[5];
                results["PE Renewable Replacements (kWh oil-eq)"] += ((item[5] + item[6]) * repNum);
                results["PE Renewable End of Life (kWh oil-eq)"] += item[6];
                results["PE Non Renewable Embodied (kWh oil-eq)"] += item[7];
                results["PE Non Renewable Replacements (kWh oil-eq)"] += ((item[7] + item[8]) * repNum);
                results["PE Non Renewable End of Life (kWh oil-eq)"] += item[8];
                results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)"] += item[9];
                results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)"] += ((item[9] + item[10]) * repNum);
                results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)"] += item[10];
                results["R value"] = Math.Round(results["R value"] + item[11], 4);
            }

            var resultValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.impactComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("e9f20241-5dab-4a84-8deb-4d2e05a2e5d8"); }
        }
    }
}
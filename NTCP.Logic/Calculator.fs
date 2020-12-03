namespace VMS.TPS

open VMS.TPS.Common.Model.API
open VMS.TPS.Common.Model.Types

type Script() = 
    member this.Execute(context:ScriptContext) =
        let mutable output = ""

        output <- output + sprintf "%s, %s (%s)\n" context.Patient.LastName context.Patient.FirstName context.Patient.Id
        let totalDose = context.PlanSetup.TotalDose
        output <- output + sprintf "\nTotal Dose: %A\n" totalDose
        let numFractions = double context.PlanSetup.NumberOfFractions.Value
        output <- output + sprintf "Number of Fractions: %0.0f\n" numFractions
        let dosePerFraction = context.PlanSetup.DosePerFraction
        output <- output + sprintf "Dose per Fraction: %A\n" dosePerFraction


        let structure = context.PlanSetup.StructureSet.Structures |> Seq.filter (fun x -> x.Id = "Lungs-GTV") |> Seq.head
        output <- output + sprintf "\nStructure: %s\n" structure.Id
        let ``a/b`` = new DoseValue(250.0, DoseValue.DoseUnit.cGy)
        output <- output + sprintf "α/ß: %0.1f Gy\n" (``a/b``.Dose / 100.0)
        let Vtot = structure.Volume
        output <- output + sprintf "Total Volume: %0.1f cc\n" Vtot

        let cumDvhData = context.PlanSetup.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1)
        let diffDvhData = cumDvhData.CurveData
                            |> Array.pairwise
                            |> Array.map (fun x -> new DVHPoint((snd x).DoseValue, (fst x).Volume - (snd x).Volume, (snd x).VolumeUnit))
        let eqd2DvhData = diffDvhData 
                            |> Array.map(fun x -> new DVHPoint(new DoseValue(x.DoseValue.Dose * (x.DoseValue.Dose / numFractions + ``a/b``.Dose) / (200.0 + ``a/b``.Dose), DoseValue.DoseUnit.cGy), x.Volume, x.VolumeUnit))

        let Dmax = cumDvhData.MaxDose
        output <- output + sprintf "Dmax: %A\n" Dmax
        let meanDose = cumDvhData.MeanDose
        output <- output + sprintf "Mean Dose: %A\n\n" meanDose
        let m = 0.37
        output <- output + sprintf "m (Slope): %0.3f\n" m
        let n = 0.99
        output <- output + sprintf "n (Volume susceptibility): %0.3f\n" n
        let TD50 = new DoseValue(3080.0, DoseValue.DoseUnit.cGy)
        output <- output + sprintf "TD50: %A\n" TD50
        
        let Veff1 = eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue / Dmax) ** (1.0 / n)) |> Array.sum
        let Veff2 = eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue / totalDose) ** (1.0 / n)) |> Array.sum
        let EUD = (eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue.Dose) ** (1.0 / n)) |> Array.sum) ** n
        let meanEQD2Dose = (eqd2DvhData |> Array.map (fun x -> x.Volume * x.DoseValue.Dose) |> Array.sum) / Vtot
        output <- output + sprintf "\nVeff1: %0.3f\n" Veff1
        output <- output + sprintf "Veff2: %0.3f\n" Veff2
        output <- output + sprintf "Mean EQD2 Dose: %0.1f cGy\n" meanEQD2Dose
        output <- output + sprintf "EUD: %0.1f cGy\n" EUD

        let normsdist x = (1.0 + FSharp.Stats.SpecialFunctions.Errorfunction.Erf( x / sqrt(2.0) ) ) / 2.0
        let NTCP = normsdist ((totalDose.Dose - (TD50.Dose / (Veff2 ** n))) / (m * (TD50.Dose / (Veff2 ** n))))
        let NTCPCheck = normsdist ((Dmax.Dose - (TD50.Dose / (Veff1 ** n))) / (m * (TD50.Dose / (Veff1 ** n))))
        let NTCPEUD = normsdist ((EUD - TD50.Dose) / (m * TD50.Dose))

        output <- output + sprintf "\nNTCP: %0.3f\n" NTCP
        output <- output + sprintf "NTCP Check: %0.3f\n" NTCPCheck
        output <- output + sprintf "NTCP(EUD): %0.3f" NTCPEUD

        System.Windows.MessageBox.Show(output) |> ignore

namespace NTCP

open VMS.TPS.Common.Model.API
open VMS.TPS.Common.Model.Types

module Calculator =

    let TocGy (dose:DoseValue) =
        if dose.Unit = DoseValue.DoseUnit.Gy
        then new DoseValue(dose.Dose * 100.0, DoseValue.DoseUnit.cGy)
        else dose

    let getDifferentialDvhData (plan:PlanSetup) structure =    
        plan.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1).CurveData
        |> Array.pairwise
        |> Array.map (fun x -> new DVHPoint((snd x).DoseValue |> TocGy, (fst x).Volume - (snd x).Volume, (snd x).VolumeUnit))

    let getEqd2DvhData (diffDvhData:DVHPoint[]) numFractions (``a/b``:DoseValue) =
        let cGyAB = ``a/b`` |> TocGy
        diffDvhData 
        |> Array.map(fun x -> new DVHPoint(new DoseValue(x.DoseValue.Dose * (x.DoseValue.Dose / numFractions + cGyAB.Dose) / (200.0 + cGyAB.Dose), DoseValue.DoseUnit.cGy), x.Volume, x.VolumeUnit))

    /// Returns NTCP (%), NTCPEUD (%), NTCPCHeck (%), meanEQD2DOse(%)
    let getNTCPs (eqd2DvhData:DVHPoint[]) (TD50:DoseValue) n m Vtot Dmax totalDose  =  
        
        let cGyTD50 = TD50 |> TocGy
        let cGyDmax = Dmax |> TocGy
        let cGyTotalDose = totalDose |> TocGy

        let Veff1 = eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue / cGyDmax) ** (1.0 / n)) |> Array.sum
        let Veff2 = eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue / cGyTotalDose) ** (1.0 / n)) |> Array.sum
        let cGyEUD = (eqd2DvhData |> Array.map (fun x -> x.Volume / Vtot * (x.DoseValue.Dose) ** (1.0 / n)) |> Array.sum) ** n
        let cGymeanEQD2Dose = (eqd2DvhData |> Array.map (fun x -> x.Volume * x.DoseValue.Dose) |> Array.sum) / Vtot
   
        let normsdist x = (1.0 + FSharp.Stats.SpecialFunctions.Errorfunction.Erf( x / sqrt(2.0) ) ) / 2.0
        let NTCP = normsdist ((cGyTotalDose.Dose - (cGyTD50.Dose / (Veff2 ** n))) / (m * (cGyTD50.Dose / (Veff2 ** n))))
        let NTCPCheck = normsdist ((cGyDmax.Dose - (cGyTD50.Dose / (Veff1 ** n))) / (m * (cGyTD50.Dose / (Veff1 ** n))))
        let NTCPEUD = normsdist ((cGyEUD - cGyTD50.Dose) / (m * cGyTD50.Dose))

        {|
            NTCP = NTCP * 100.0
            NTCPEUD = NTCPEUD * 100.0
            NTCPCheck = NTCPCheck * 100.0
            MeanEqd2Dose = 
                if totalDose.Unit = DoseValue.DoseUnit.Gy
                then new DoseValue(cGymeanEQD2Dose / 100.0, DoseValue.DoseUnit.Gy)
                else new DoseValue(cGymeanEQD2Dose, DoseValue.DoseUnit.cGy)
        |}

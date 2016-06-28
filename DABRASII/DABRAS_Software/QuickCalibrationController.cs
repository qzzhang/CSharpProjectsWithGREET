using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DABRAS_Software
{
    class QuickCalibrationController
    {

        #region Data Members

        private readonly mainFramework frmParent;
        private List<Radioactive_Source> ListOfSources;
        private bool SourcesModified = false;

        private DABRAS DABRAS;

        private bool KillAll = false;

        #region Const Identifiers for "as found" checks
        private const int BackgroundAsFoundAlphaLimit = 7;
        private const int BackgroundAsFoundBetaLimit = 350;
        private const int AsFoundEff_Alpha_LowerLimit = 23;
        private const int AsFoundEff_Alpha_UpperLimit = 32;
        private const int AsFoundEff_Beta_LowerLimit = 47;
        private const int AsFoundEff_Beta_UpperLimit = 56;
        #endregion

        #region Stored Data from As Found Checks
        private double AsFoundBG_Alpha = -1;
        private double AsFoundBG_Beta = -1;
        private double AsFoundAlpha = -1;
        private double AsFoundBeta = -1;
        private double AsFoundEfficiency_Alpha = -1;
        private double AsFoundEfficiency_Beta = -1;
        #endregion

        #region Const Identifiers for Background HiLo
        private const int MaxAlphaBGCPM = 3;
        private const int MaxBetaBGCPM = 300;
        #endregion

        #region Stored Data from Background HiLo
        private double HiLo_BG_Alpha_CPM = -1;
        private double HiLo_BG_Beta_CPM = -1;
        private double HiLo_BG_Alpha_LL = -1;
        private double HiLo_BG_Beta_LL = -1;
        private double HiLo_BG_Alpha_UL = -1;
        private double HiLo_BG_Beta_UL = -1;
        #endregion

        #region Stored Data from Alpha HiLo
        private double HiLo_Alpha_CPM = -1;
        private double HiLo_Alpha_UL = -1;
        private double HiLo_Alpha_LL = -1;
        #endregion

        #region Stored Data from Beta HiLo
        private double HiLo_Beta_CPM;
        private double HiLo_Beta_UL;
        private double HiLo_Beta_LL;
        #endregion

        #region Stored Data from Efficiencies
        string SourceFailed = "";
        #endregion

        #region Defines for Final Calibration Check
        private string[] FinalCalSources = { "Am-241", "Sr-90" };
        private const double Max_DPM_Error = 5;
        #endregion

        #region Data for Final Calibration Check
        private double[,] FinalCalResults;
        #endregion

        #region Defines for Uniformity tests
        private const int NUM_SLOTS = 5;
        private const int MAX_DISPARITY = 20;
        #endregion

        #region Stored data from Uniformity test
        private double[] Uniformity_Results;
        #endregion

        #endregion

        #region Constructor

        public QuickCalibrationController(mainFramework _Parent)
        {
            this.frmParent = _Parent;
            this.ListOfSources = frmParent.GetListOfSources();
            this.DABRAS = frmParent.GetDABRAS();
        }

        #endregion

        #region Main Calibration Thread
        public void RunCalibrationProc()
        {
            #region Initialization

            Radioactive_Source Am241 = ListOfSources.Find(x => x.GetName() == "Am-241");
            Radioactive_Source BG = ListOfSources.Find(x => x.GetName() == "Background");
            Radioactive_Source Sr90 = ListOfSources.Find(x => x.GetName() == "Sr-90");
            Radioactive_Source Tc99 = ListOfSources.Find(x => x.GetName() == "Tc-99");

            //WTF?!
            //foreach (Radioactive_Source R in ListOfSources)
            //{
            //    R.SetAlphaEfficiency(-1);
            //    R.SetBetaEfficiency(-1);
            //    R.SetAnnualAlphaCPM(-1);
            //    R.SetAnnualBetaCPM(-1);
            //}

            #endregion
            
            MessageBox.Show("Welcome to the quick calibration tool. Press shift+ctrl+alt at any time to cancel the calibration.");

            #region Source Edit Information
            if (!KillAll)
            {
                MessageBox.Show("Please confirm that all of the saved source information is correct.");

                ViewEditSources ViewSourcesForm = new ViewEditSources(this.frmParent, this.ListOfSources);
                ViewSourcesForm.ShowDialog();

                if (ViewSourcesForm.WereSourcesModified())
                {
                    this.ListOfSources = ViewSourcesForm.GetRadioActiveSourceList();
                    this.SourcesModified = true;
                }
            }
            #endregion

            #region As Found Tests

            #region Background
            /*Background as found test*/
            MessageBox.Show("We will begin with the background as found test. Verify that the DABRAS tray is empty, and press OK.");
            mainFramework AsFoundChecks = new mainFramework(00);
            //AsFoundChecks.Start1minCheck(this, ListOfSources, "Am-241", "Sr-90");

            AsFoundChecks.ShowDialog();

            //If safely closed, save results. Also save if closed with "x" button and a test was completed.
            /*
            if ((AsFoundChecks.DialogResult == DialogResult.OK) || (AsFoundChecks.DialogResult == DialogResult.Cancel && AsFoundChecks.WasTestCompleted()))
            {
                if (AsFoundChecks.GetAlphaGCPM() < BackgroundAsFoundAlphaLimit && AsFoundChecks.GetBetaGCPM() < BackgroundAsFoundBetaLimit)
                {
                    MessageBox.Show(String.Format("Test successfully completed with AlphaGCPM of {0} and Beta GCPM of {1}", StaticMethods.RoundToSigFigs(AsFoundChecks.GetAlphaGCPM()), StaticMethods.RoundToSigFigs(AsFoundChecks.GetBetaGCPM())));
                    this.AsFoundBG_Alpha = AsFoundChecks.GetAlphaGCPM();
                    this.AsFoundBG_Beta = AsFoundChecks.GetBetaGCPM();
                }

                else
                {
                    MessageBox.Show("Test failed. Stopping...");
                    KillAll = true;
                }

            }

            //If killed, don't save
            else
            {
                MessageBox.Show("Test not successfully completed. Stop.");
                KillAll = true;
            }
            */

            #endregion

            #region Alpha
            if (!KillAll)
            {
                MessageBox.Show("Please insert Am-241 source into the DABRAS counting tray and click OK");
                AsFoundChecks = new FormRSC();
                AsFoundChecks.Start1minCheck(this, ListOfSources, "Am-241", "Sr-90");

                AsFoundChecks.ShowDialog();

                if ((AsFoundChecks.DialogResult == DialogResult.OK) || (AsFoundChecks.DialogResult == DialogResult.Cancel && AsFoundChecks.WasTestCompleted()))
                {
                    this.AsFoundAlpha = AsFoundChecks.GetAlphaGCPM();
                }

                else
                {
                    MessageBox.Show("Test not successfully completed. Stop.");
                    KillAll = true;
                }
            }

            #endregion

            #region Beta
            if (!KillAll)
            {
                MessageBox.Show("Please insert Sr-90 source into the DABRAS counting tray and click OK");
                AsFoundChecks = new mainFramework(0);
                AsFoundChecks.Start1minCheck(this, ListOfSources, "Am-241", "Sr-90");

                AsFoundChecks.ShowDialog();

                if ((AsFoundChecks.DialogResult == DialogResult.OK) || (AsFoundChecks.DialogResult == DialogResult.Cancel && AsFoundChecks.WasTestCompleted()))
                {
                    /*Verify results*/
                    this.AsFoundBeta = AsFoundChecks.GetBetaGCPM();

                    if (this.AsFoundBG_Alpha > BackgroundAsFoundAlphaLimit || this.AsFoundBG_Beta > BackgroundAsFoundBetaLimit)
                    {
                        MessageBox.Show(String.Format("Alpha and Beta results outside of acceptable range. Alpha result was {0}, limit was {1}. Beta Result was {2}, Limit was {3}. Test failed. Stopping.", StaticMethods.RoundToDecimal(AsFoundAlpha, 1), StaticMethods.RoundToDecimal(BackgroundAsFoundAlphaLimit, 1), StaticMethods.RoundToDecimal(AsFoundBG_Beta, 1), StaticMethods.RoundToDecimal(BackgroundAsFoundBetaLimit, 1)));
                        KillAll = true;
                    }

                    /*Compute efficiencies and check their results*/

                    double TargetCounts_Alpha = Am241.GetCorrectedDPM();
                    double TargetCounts_Beta = Sr90.GetCorrectedDPM();

                    this.AsFoundEfficiency_Alpha = 100 * (this.AsFoundAlpha - this.AsFoundBG_Alpha) / TargetCounts_Alpha;
                    this.AsFoundEfficiency_Beta = 100 * (this.AsFoundBeta - this.AsFoundBG_Beta) / TargetCounts_Beta;

                    if ((this.AsFoundEfficiency_Alpha > AsFoundEff_Alpha_LowerLimit) && (this.AsFoundEfficiency_Alpha < AsFoundEff_Alpha_UpperLimit) && (this.AsFoundEfficiency_Beta > AsFoundEff_Beta_LowerLimit) && (this.AsFoundEfficiency_Beta < AsFoundEff_Beta_UpperLimit))
                    {
                        MessageBox.Show(String.Format("Test successfully completed with AlphaGCPM of {0} and Beta GCPM of {1}\nAlpha Efficiency = {2}, Beta Efficiency = {3}.", StaticMethods.RoundToDecimal(AsFoundAlpha, 1), StaticMethods.RoundToDecimal(AsFoundBeta, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Alpha, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Beta, 1)));
                    }

                    else
                    {
                        MessageBox.Show(String.Format("Test Failed. AlphaGCPM was {0}, Beta GCPM was {1}\n\nThe computed Alpha Efficiency was {2}%, with the maximum and minimum allowable values being {3}% and {4}%.\n\n The computed Beta Efficiency was {5}%, with the maximum and minimum allowable values being {6}% and {7}%. Test stopping.", StaticMethods.RoundToDecimal(AsFoundAlpha, 1), StaticMethods.RoundToDecimal(AsFoundBeta, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Alpha, 1), StaticMethods.RoundToDecimal(AsFoundEff_Alpha_UpperLimit, 1), StaticMethods.RoundToDecimal(AsFoundEff_Alpha_LowerLimit, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Beta, 1), StaticMethods.RoundToDecimal(AsFoundEff_Beta_UpperLimit, 1), StaticMethods.RoundToDecimal(AsFoundEff_Beta_LowerLimit, 1)));
                        KillAll = true;
                    }

                }

                else
                {
                    MessageBox.Show("Test not successfully completed. Stop.");
                    KillAll = true;
                }
            }
            #endregion

            #endregion         

            #region High Voltage Plateau
            if (!KillAll)
            {
                MessageBox.Show("Next, we will perform a high voltage plateau test. You will need to manually set the parameters on the next page. Please place the Sr-90 source inside of the device.");
                CalibrationForm CF = new CalibrationForm(this.Parent);
                HighVoltagePlateau HVP = new HighVoltagePlateau(CF);
                HVP.ShowDialog();

                CF.Hide();
                /*HVC gets set during form*/
            }

            #endregion

            #region Voltage Confirmation

            if (!KillAll)
            {
                MessageBox.Show("Please confirm that the high voltage settings are correct.");

                SetHighVoltageForm SetHV = new SetHighVoltageForm(this.DABRAS);
                SetHV.ShowDialog();

                /*Nothing to save/return*/
            }
            #endregion

            #region Background HiLo
            /*Call HiLo for Background*/
            if (!KillAll)
            {
                MessageBox.Show("We will now begin the calibration with the 10 x 10 minute Background Hi/Lo calibration. This step will take 100 minutes to complete. Be sure that there are no sources in the DABRAS. Press OK to continue.");

                HiLo NewHiLo = new HiLo(new CalibrationForm(this.Parent), HiLo.TypeOfHiLo.BACKGROUND); //hmm...

                bool done = false;
                while (!done)
                {
                    NewHiLo.StartBackgroundHiLo(this, ListOfSources);

                    NewHiLo.ShowDialog(); //Don't auto check results from this one; let the user decide

                    if ((NewHiLo.DialogResult == DialogResult.OK) || (NewHiLo.DialogResult == DialogResult.Cancel && NewHiLo.WasTestCompleted()))
                    {
                        /*Check results*/

                        if ((NewHiLo.GetAlphaAvg() >= MaxAlphaBGCPM) || (NewHiLo.GetBetaAvg() >= MaxBetaBGCPM))
                        {
                            MessageBox.Show(String.Format("Error: Alpha or Beta counts out of range. Alpha CPM was {0}, maximum allowable was {1}.\n Beta CPM was {2}, maximum allowable was {3}. Test stopping.", StaticMethods.RoundToSigFigs(NewHiLo.GetAlphaAvg()), StaticMethods.RoundToSigFigs(MaxAlphaBGCPM), StaticMethods.RoundToSigFigs(NewHiLo.GetBetaAvg()), StaticMethods.RoundToSigFigs(MaxBetaBGCPM)));
                            KillAll = true;
                        }

                        else
                        {
                            MessageBox.Show(String.Format("Background Calibration passed. Alpha CPM was {0}, with a Hi/Lo set as {1} and {2}. Beta CPM was {3}, with a Hi/Lo set as {4} and {5}", NewHiLo.GetAlphaAvg(), NewHiLo.GetAlphaUL(), NewHiLo.GetAlphaLL(), NewHiLo.GetBetaAvg(), NewHiLo.GetBetaUL(), NewHiLo.GetBetaLL()));

                            /*Save data*/
                            this.HiLo_BG_Alpha_CPM = NewHiLo.GetAlphaAvg();
                            this.HiLo_BG_Alpha_LL = NewHiLo.GetAlphaLL();
                            this.HiLo_BG_Alpha_UL = NewHiLo.GetAlphaUL();
                            this.HiLo_BG_Beta_CPM = NewHiLo.GetBetaAvg();
                            this.HiLo_BG_Beta_LL = NewHiLo.GetBetaLL();
                            this.HiLo_BG_Beta_UL = NewHiLo.GetBetaUL();

                            BG.SetAnnualAlphaCPM(Convert.ToInt32(HiLo_BG_Alpha_CPM));
                            BG.SetAnnualBetaCPM(Convert.ToInt32(HiLo_BG_Beta_CPM));

                            BG.SetAlphaHi(Convert.ToInt32(this.HiLo_BG_Alpha_UL));
                            BG.SetAlphaLo(Convert.ToInt32(this.HiLo_BG_Alpha_LL));
                            BG.SetBetaHi(Convert.ToInt32(this.HiLo_BG_Beta_UL));
                            BG.SetBetaLo(Convert.ToInt32(this.HiLo_BG_Beta_LL));
                            done = true;
                        }
                    }

                    else
                    {
                        if (MessageBox.Show("Test not completed successfully. Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            KillAll = true;
                            done = true;
                        }
                    }
                }
            }

            #endregion

            #region Efficiencies

            /*Compute efficiency for every source in the list*/
            if (!KillAll)
            {
                foreach (Radioactive_Source R in ListOfSources)
                {
                    bool done = false;
                    while (!done)
                    {
                        if (R.GetName() == "Background")
                        {
                            done = true;
                            continue;
                        }
                        
                        if (R.GetName() != "Background" && !KillAll)
                        {
                            MessageBox.Show(String.Format("We will now attempt to calibrate the efficiencies of {0}. Please place the correct source in the detector and press OK.", R.GetName()));
                            CalibrationForm CalForm = new CalibrationForm(Parent);
                            if (CalForm.AutoCalibrateSourceEfficiency(this, this.ListOfSources, R.GetName()))
                            {
                                /*Source was found on list. Continue*/
                                CalForm.ShowDialog();

                                if ((CalForm.DialogResult == DialogResult.OK) || (CalForm.DialogResult == DialogResult.Cancel && CalForm.WasCalCompleted()))
                                {
                                    /*Ask user to confirm results*/

                                    if (R.GetSourceType() == Radioactive_Source.RadiationType.Alpha)
                                    {
                                        if (MessageBox.Show(String.Format("Alpha Efficiency is {0}. Confirm?", StaticMethods.RoundToSpecificSigFigs(CalForm.GetAlphaEff(), 3)), "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                        {
                                            R.SetAlphaEfficiency(CalForm.GetAlphaEff());
                                            R.SetBetaEfficiency(0);
                                            done = true;
                                        }

                                        else
                                        {
                                            /*Give option to kill all*/
                                            if (MessageBox.Show("Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                            {
                                                MessageBox.Show("Calibration cancelled.");
                                                SourceFailed = R.GetName();
                                                KillAll = true;
                                                KillAll = true;
                                                done = true;
                                                break;
                                            }
                                        }
                                    }

                                    else if (R.GetSourceType() == Radioactive_Source.RadiationType.Beta)
                                    {
                                        if (MessageBox.Show(String.Format("Beta Efficiency is {0}. Confirm?", StaticMethods.RoundToSpecificSigFigs(CalForm.GetBetaEff(), 3)), "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                        {
                                            R.SetAlphaEfficiency(0);
                                            R.SetBetaEfficiency(CalForm.GetBetaEff());
                                            done = true;
                                        }

                                        else
                                        {
                                            /*For now, kill all*/
                                            if (MessageBox.Show("Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                            {
                                                MessageBox.Show("Calibration cancelled.");
                                                SourceFailed = R.GetName();
                                                KillAll = true;
                                                done = true;
                                            }
                                        }
                                    }

                                }

                                else
                                {
                                    /*For now, kill all*/
                                    if (MessageBox.Show("Test not successfully completed. Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                    {
                                        MessageBox.Show("Calibration cancelled.");
                                        SourceFailed = R.GetName();
                                        KillAll = true;
                                        done = true;
                                        break;
                                    }
                                }
                            }

                            else
                            {
                                /*This should not ever be executed...*/
                                MessageBox.Show("Source Not Found on List?");
                            }
                        }
                        
                    }

                    if (KillAll)
                    {
                        break;
                    }
                }
            }

            #endregion

            #region Alpha/Beta HiLos

            #region Alpha HiLo
            if (!KillAll)
            {
                MessageBox.Show("We will now begin the Alpha HiLo Calibration with the 10 x 1 minute Hi/Lo calibration. Be sure that the Am-241 source is in the DABRAS. Press OK to continue.");

                
                HiLo NewHiLo = new HiLo(new CalibrationForm(this.Parent), HiLo.TypeOfHiLo.ALPHA); //hmm...

                bool done = false;
                while (!done)
                {
                    NewHiLo.StartBackgroundHiLo(this, this.ListOfSources);
                
                    NewHiLo.ShowDialog(); //Don't auto check results from this one; let the user decide

                
                    if ((NewHiLo.DialogResult == DialogResult.OK) || (NewHiLo.DialogResult == DialogResult.Cancel && NewHiLo.WasTestCompleted()))
                    {
                        if (MessageBox.Show(String.Format("Alpha Results = {0}, with a Hi/Lo of {1} / {2}.", NewHiLo.GetAlphaAvg(), NewHiLo.GetAlphaUL(), NewHiLo.GetAlphaLL()), "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {

                            /*Save data*/
                            this.HiLo_Alpha_CPM = NewHiLo.GetAlphaAvg();
                            this.HiLo_Alpha_LL = NewHiLo.GetAlphaLL();
                            this.HiLo_Alpha_UL = NewHiLo.GetAlphaUL();

                            Am241.SetAlphaHi(Convert.ToInt32(this.HiLo_Alpha_UL));
                            Am241.SetAlphaLo(Convert.ToInt32(this.HiLo_Alpha_LL));
                            done = true;
                        }

                        else
                        {
                            if (MessageBox.Show("Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                MessageBox.Show("Test failed. Stop.");
                                KillAll = true;
                                done = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Beta HiLo
            if (!KillAll)
            {
                MessageBox.Show("We will now begin the Beta HiLo Calibration with the 10 x 1 minute Hi/Lo calibration. Be sure that the Sr-90 source is in the DABRAS. Press OK to continue.");

                HiLo NewHiLo = new HiLo(new CalibrationForm(this.Parent), HiLo.TypeOfHiLo.BETA); //hmm...

                bool done = false;
                while (!done)
                {
                    NewHiLo.StartBackgroundHiLo(this, this.ListOfSources);

                    NewHiLo.ShowDialog(); //Don't auto check results from this one; let the user decide

                    if ((NewHiLo.DialogResult == DialogResult.OK) || (NewHiLo.DialogResult == DialogResult.Cancel && NewHiLo.WasTestCompleted()))
                    {
                        if (MessageBox.Show(String.Format("Beta Results = {0}, with a Hi/Lo of {1} / {2}.", StaticMethods.RoundToSigFigs(NewHiLo.GetBetaAvg()), StaticMethods.RoundToSigFigs(NewHiLo.GetBetaUL()), StaticMethods.RoundToSigFigs(NewHiLo.GetBetaLL())), "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {

                            /*Save data*/
                            this.HiLo_Beta_CPM = NewHiLo.GetBetaAvg();
                            this.HiLo_Beta_UL = NewHiLo.GetBetaUL();
                            this.HiLo_Beta_LL = NewHiLo.GetBetaLL();

                            Sr90.SetBetaHi(Convert.ToInt32(this.HiLo_Beta_UL));
                            Sr90.SetBetaLo(Convert.ToInt32(this.HiLo_Beta_LL));
                            done = true;
                        }

                        else
                        {
                            if (MessageBox.Show("Retry?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                MessageBox.Show("Test failed. Stop.");
                                KillAll = true;
                                done = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #endregion

            #region Parameter Summary Check
            //if (!KillAll)
            //{
            //    MessageBox.Show("We will now verify that the parameters of the DABRAS are set correctly.");

            //    ParamSummaryForm ParamForm = new ParamSummaryForm(Parent);
            //    ParamForm.ShowDialog();
            //    /*TODO: and...what should happen here?*/
            //}
            #endregion

            #region Final Counting Verification

            if (!KillAll)
            {
                MessageBox.Show("We will now begin the final calibration verification.");

                this.FinalCalResults = new double[3, FinalCalSources.Length];

                for (int i = 0; i < FinalCalSources.Length; i++)
                {
                    bool done = false;
                    while (!done)
                    {
                        string SrcName = FinalCalSources[i];

                        MessageBox.Show(String.Format("Please place the {0} source in the detector.", SrcName));

                        mainFramework FinalForm = new mainFramework(0);

                        Radioactive_Source R = ListOfSources.Find(x => x.GetName() == SrcName);

                        if (R.GetSourceType() == Radioactive_Source.RadiationType.Alpha)
                        {
                            FinalForm.Start1minCheck(this, ListOfSources, R.GetName(), "Sr-90");
                        }

                        else if (R.GetSourceType() == Radioactive_Source.RadiationType.Beta)
                        {
                            FinalForm.Start1minCheck(this, ListOfSources, "Am-241", R.GetName());
                        }

                        else
                        {
                            FinalForm.Start1minCheck(this, ListOfSources, "Am-241", "Sr-90");
                        }

                        FinalForm.ShowDialog();

                        if ((FinalForm.DialogResult == DialogResult.OK) || (FinalForm.DialogResult == DialogResult.Cancel && FinalForm.WasTestCompleted()))
                        {

                            if (R != null)
                            {
                                double DisparityPercentage;
                                double GrossDPM;

                                if (R.GetSourceType() == Radioactive_Source.RadiationType.Alpha)
                                {
                                    double z = FinalForm.GetAlphaDPM();
                                    double y = R.GetCorrectedDPM();

                                    DisparityPercentage = 100 * Math.Abs((FinalForm.GetAlphaDPM() - R.GetCorrectedDPM()) / R.GetCorrectedDPM());
                                    GrossDPM = FinalForm.GetAlphaDPM();
                                }

                                else
                                {
                                    double z = FinalForm.GetBetaDPM();
                                    double y = R.GetCorrectedDPM();

                                    DisparityPercentage = 100 * Math.Abs((FinalForm.GetBetaDPM() - R.GetCorrectedDPM()) / R.GetCorrectedDPM());
                                    GrossDPM = FinalForm.GetBetaDPM();
                                }

                                if (DisparityPercentage > Max_DPM_Error)
                                {
                                    if (MessageBox.Show("Error: Source out of calibration. Retry procedure?", "Confirm Retry", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                    {
                                        FinalCalResults[0, i] = -1; //Set stop flag for writer
                                        i = FinalCalSources.Length + 1; //Set break flag for loop;
                                        done = true;
                                        KillAll = true;
                                    }
                                }

                                else
                                {
                                    FinalCalResults[0, i] = GrossDPM;
                                    FinalCalResults[1, i] = R.GetCorrectedDPM();
                                    FinalCalResults[2, i] = DisparityPercentage;
                                    done = true;
                                }
                            }

                            else
                            {
                                MessageBox.Show("Error: Source not found. Try Manual calibration.");
                                KillAll = true;
                                done = true;
                            }
                        }

                        else
                        {
                            MessageBox.Show("Error: Test terminated. Stop.");
                            KillAll = true;
                            done = true;
                        }
                    }
                }
            }

            #endregion

            #region Uniformity Test

            /*Initialize the Uniformity test data structure*/
            if (!KillAll)
            {
                bool done = false;
                while (!done)
                {

                    this.Uniformity_Results = new double[NUM_SLOTS];

                    for (int i = 0; i < NUM_SLOTS; i++)
                    {
                        Uniformity_Results[0] = 0;
                    }

                    MessageBox.Show("We will now begin the Uniformity Test.");

                    for (int i = 0; i < NUM_SLOTS; i++)
                    {
                        MessageBox.Show(String.Format("Please place the Sr-90 source in slot {0} and press OK", i + 1));

                        mainFramework UniformForm = new mainFramework(0);
                        UniformForm.StartUniformityTest(this, ListOfSources);

                        UniformForm.ShowDialog();

                        if ((UniformForm.DialogResult == DialogResult.OK) || (UniformForm.DialogResult == DialogResult.Cancel && UniformForm.WasTestCompleted()))
                        {
                            /*Store data*/
                            if (MessageBox.Show(String.Format("Save result of {0}GCPM?", UniformForm.GetBetaGCPM()), "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                Uniformity_Results[i] = UniformForm.GetBetaGCPM();
                            }

                            else
                            {
                                MessageBox.Show("Test failed.");
                                Uniformity_Results[NUM_SLOTS / 2] = -500;
                                break;
                            }
                        }

                        else
                        {
                            MessageBox.Show("Test Terminated.");
                            Uniformity_Results[NUM_SLOTS / 2] = -500;
                            break;
                        }

                    }


                    /*verify results*/

                    double Target = Uniformity_Results[NUM_SLOTS / 2];
                    int NumPassed = 0;

                    if (Target != -500) //check for break condition
                    {
                        for (int i = 0; i < NUM_SLOTS; i++)
                        {
                            if ((Math.Abs(Uniformity_Results[i] - Target) / Target) > MAX_DISPARITY)
                            {
                                if (MessageBox.Show(String.Format("Error: Slot {0} failed to pass. Retry?", i + 1), "Confirm Retry", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    done = true;
                                    KillAll = true;
                                    NumPassed = NUM_SLOTS + 1; //So we don't trigger the equals condition.
                                    break;
                                }
                            }

                            else
                            {
                                NumPassed++;
                            }

                        }
                    }

                    else
                    {
                        if (MessageBox.Show("Error: Test incomplete. Retry?", "Confirm Retry", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            KillAll = true;
                            done = true;
                        }
                    }

                    if (NumPassed == NUM_SLOTS)
                    {
                        MessageBox.Show("All slots passed.\n");
                        done = true;
                    }
                }
            }
            #endregion

            /**/
            #region Present results

            /*TODO: Present results*/

            #endregion

            #region Finalization and saving

            if (!KillAll)
            {
                MessageBox.Show("Calibration successfully completed!");

                foreach (Radioactive_Source R in ListOfSources)
                {
                    R.SetAnnualCalibratedDate(DateTime.Now);
                }
            }

            MessageBox.Show("Please save the results of this test to a text file.");
            WriteToFile(!KillAll);

            MessageBox.Show("Quick Calibration routine completed. You will now be returned to the home screen.");

            #endregion

            return;
        }
        #endregion

        #region TextWriter Functions
        private void WriteToFile(bool Passed)
        {
            
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Text File|*.txt";
            SD.ShowDialog();

            if (SD.FileName != "")
            {
                FileStream F = (FileStream)SD.OpenFile();

                TextWriter TW = new StreamWriter(F);

                /*Here we go...*/

                TW.WriteLine("DABRAS Quick Calibration Results");
                TW.WriteLine(String.Format("Calibration finished on {0}", DateTime.Now));
                Write_Spacer(TW);

                #region As Found Results

                TW.WriteLine("As-Found Results");

                #region Background
                if (this.AsFoundBG_Alpha != -1 && this.AsFoundBG_Beta != -1)
                {
                    TW.WriteLine(String.Format("The as found background alpha GCPM and beta GCPM were determined to be {0}CPM and {1}CPM", StaticMethods.RoundToDecimal(AsFoundBG_Alpha, 1), StaticMethods.RoundToDecimal(AsFoundBG_Beta, 1)));
                }
                else
                {
                    TW.WriteLine(String.Format("The as found Background checks failed, causing the calibration process to terminate."));
                    TW.Close();
                    F.Close();
                    return;
                }
                #endregion

                #region Alpha
                if (this.AsFoundAlpha != -1)
                {
                    TW.WriteLine(String.Format("The as found alpha source response was {0} leaving an efficiency of {1}%.", StaticMethods.RoundToDecimal(AsFoundAlpha, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Alpha, 1)));
                }
                else
                {
                    TW.WriteLine(String.Format("The as found alpha source test failed, causing the calibration process to terminate."));
                    TW.Close();
                    F.Close();
                    return;
                }
                #endregion

                #region Beta
                if (this.AsFoundBeta != -1)
                {
                    TW.WriteLine(String.Format("The as found beta source response was {0}, leaving an efficiency of {1}%.", StaticMethods.RoundToDecimal(AsFoundBeta, 1), StaticMethods.RoundToDecimal(AsFoundEfficiency_Beta, 1)));
                }
                else
                {
                    TW.WriteLine(String.Format("The as found beta source test failed, cuasing the calibration process to terminate."));
                    TW.Close();
                    F.Close();
                    return;
                }
                #endregion

                Write_Spacer(TW);

                #endregion

                

                #region Background HiLo
                TW.WriteLine("Background HiLo Calibration");

                if (HiLo_BG_Alpha_CPM != -1 && HiLo_BG_Beta_CPM != -1)
                {
                    TW.WriteLine(String.Format("The Background HiLo was set at {0}/{1}, with an average of {2} for alpha.", StaticMethods.RoundToDecimal(HiLo_BG_Alpha_UL, 1), StaticMethods.RoundToDecimal(HiLo_BG_Alpha_LL, 1), StaticMethods.RoundToDecimal(HiLo_BG_Alpha_CPM, 1)));
                    TW.WriteLine(String.Format("The Background HiLo was set at {0}/{1}, with an average of {2} for beta.", StaticMethods.RoundToDecimal(HiLo_BG_Beta_UL, 1), StaticMethods.RoundToDecimal(HiLo_BG_Beta_LL, 1), StaticMethods.RoundToDecimal(HiLo_BG_Beta_CPM, 1)));
                }

                else
                {
                    TW.WriteLine("The calibration failed at the Background HiLo calibration phase.");
                    TW.Close();
                    F.Close();
                    return;
                }

                #endregion

                #region Source Information

                TW.WriteLine("Final Calibrate Source Information.");

                foreach (Radioactive_Source R in ListOfSources)
                {
                    string Name = R.GetName();

                    if (SourceFailed != R.GetName() || (R.GetName() == "Background"))
                    {

                        TW.WriteLine(String.Format("The Alpha NCPM for the {0} source was {1}", Name, StaticMethods.RoundToDecimal(R.GetAnnualAlphaCPM(), 1)));
                        TW.WriteLine(String.Format("The Beta NCPM for the {0} source was {1}", Name, StaticMethods.RoundToDecimal(R.GetAnnualBetaCPM(), 1)));

                        TW.WriteLine(String.Format("The Alpha Efficiency for the {0} source was {1}", Name, StaticMethods.RoundToDecimal(R.GetAlphaEfficiency(), 1)));
                        TW.WriteLine(String.Format("The Beta Efficiency for the {0} source was {1}", Name, StaticMethods.RoundToDecimal(R.GetBetaEfficiency(), 1)));

                        TW.WriteLine("---------------------------------------------------------------");
                    }

                    else
                    {
                        TW.WriteLine(String.Format("The test failed on the calibration of {0}", Name));
                        TW.Close();
                        F.Close();
                        return;
                    }

                }

                Write_Spacer(TW);

                #endregion

                #region HiLos
                TW.WriteLine("Hi/Lo Levels");

                if (HiLo_Alpha_CPM != -1)
                {
                    TW.WriteLine(String.Format("The HiLo for the alpha source was set at {0}/{1} with an average of {2}", StaticMethods.RoundToDecimal(HiLo_Alpha_UL, 1), StaticMethods.RoundToDecimal(HiLo_Alpha_LL, 1), StaticMethods.RoundToDecimal(HiLo_Alpha_CPM, 1)));
                }
                else
                {
                    TW.WriteLine(String.Format("The calibration failed in the alpha HiLo Calibration phase."));
                    TW.Close();
                    F.Close();
                    return;
                }

                if (HiLo_Beta_CPM != -1)
                {
                    TW.WriteLine(String.Format("The HiLo for the beta source was set at {0}/{1} with an average of {2}", StaticMethods.RoundToDecimal(HiLo_Beta_UL, 1), StaticMethods.RoundToDecimal(HiLo_Beta_LL, 1), StaticMethods.RoundToDecimal(HiLo_Beta_CPM, 1)));
                }
                else
                {
                    TW.WriteLine(String.Format("The calibration failed in the beta HiLo Calibration phase."));
                    TW.Close();
                    F.Close();
                    return;
                }

                Write_Spacer(TW);
                #endregion

                #region Calibration Verification
                TW.WriteLine("Final Calibration");

                
                for (int i = 0; i < FinalCalSources.Length; i++)
                {
                    if (FinalCalResults[i, 0] != -1)
                    {
                        TW.WriteLine(String.Format("The Final Calibration results for {0} was a DPM of {1}. The expected DPM was {2}, with an error of {3}.", FinalCalSources[i], StaticMethods.RoundToSigFigs(FinalCalResults[0, i]), StaticMethods.RoundToSigFigs(FinalCalResults[1, i]), StaticMethods.RoundToSigFigs(FinalCalResults[2, i])));
                    }
                    else
                    {
                        TW.WriteLine(String.Format("The calibration failed on the Final calibration of {0}.", FinalCalSources[i]));
                        TW.Close();
                        F.Close();
                        return;
                    }
                }

                Write_Spacer(TW);
                #endregion

                #region Uniformity Test
                TW.WriteLine("Uniformity Test Result");

                for (int i = 0; i < Uniformity_Results.Length; i++)
                {
                    if (Uniformity_Results[i] != -1)
                    {
                        TW.WriteLine("The CPM for position {0} was {1}.", i + 1, StaticMethods.RoundToDecimal(Uniformity_Results[i], 1));
                    }

                    else
                    {
                        TW.WriteLine(String.Format("The calibration failed on position {0} of the uniformity check.", i + 1));
                        TW.Close();
                        F.Close();
                        return;
                    }
                }

                Write_Spacer(TW);
                #endregion

                #region FinalResult
                if (Passed)
                {
                    TW.WriteLine("Overall, the machine has been calibrated and cleared for use.");
                }
                else
                {
                    TW.WriteLine("The machine has NOT been calibrated. Do not use without recalibration.");
                }
                #endregion

                TW.Close();
                F.Close();

                return;

            }
        }

        private void Write_Spacer(TextWriter T)
        {
            T.WriteLine();
            T.WriteLine("****************************************************************");
            T.WriteLine();
            return;
        }
        #endregion

        #region Getters
        public List<Radioactive_Source> GetListOfSources()
        {
            return this.ListOfSources;
        }
        #endregion
    }
}

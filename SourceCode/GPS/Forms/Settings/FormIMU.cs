﻿using System;
using System.Linq;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormIMU : Form
    {
        private readonly FormGPS mf = null;

        private string headingFromWhichSource;
        private decimal minFixStepDistance;

        public FormIMU(Form callingForm)
        {
            mf = callingForm as FormGPS;
            InitializeComponent();

            //Languages

            groupBox4.Text = gStr.gsFixFrom;

            this.headingGroupBox.Text = gStr.gsGPSHeadingFrom;
            this.label13.Text = gStr.gsDualAntenna;
            this.label12.Text = gStr.gsFromVTGorRMC;
            this.label11.Text = gStr.gsFixToFixCalc;
            this.btnRollZero.Text = gStr.gsRollZero;
            this.btnRemoveZeroOffset.Text = gStr.gsRemoveOffset;
            this.label10.Text = gStr.gsALLSettingsRequireRestart;

            this.groupBox6.Text = gStr.gsRollSource;
            this.rbtnRollGPS.Text = gStr.gsFromGPS;
            this.rbtnRollAutoSteer.Text = gStr.gsFromAutoSteer;
            rbtnRollUDP.Text = gStr.gsUDP;

            this.groupBoxHeadingCorrection.Text = gStr.gsHeadingCorrectionSource;
            this.rbtnHeadingCorrAutoSteer.Text = gStr.gsFromAutoSteer;
            rbtnHeadingCorrUDP.Text = gStr.gsUDP;
            rbtnHeadingCorrNone.Text = gStr.gsNone;
            rbtnRollNone.Text = gStr.gsNone;

            this.groupBox1.Text = gStr.gsFixToFixDistance;
            this.label35.Text = gStr.gsMeters;
            this.lblSimGGA.Text = gStr.gsUseGGAForSimulator;

            this.Text = gStr.gsDataSources;

            tabHeading.Text = gStr.gsHeading;
            tabFix.Text = gStr.gsFix;
            tabRoll.Text = gStr.gsRoll;

            nudMinFixStepDistance.Controls[0].Enabled = false;
        }

        #region EntryExit

        private void bntOK_Click(object sender, EventArgs e)
        {
            ////Display ---load the delay slides --------------------------------------------------------------------
            if (headingFromWhichSource == "Fix") Properties.Settings.Default.setGPS_headingFromWhichSource = "Fix";
            else if (headingFromWhichSource == "GPS") Properties.Settings.Default.setGPS_headingFromWhichSource = "GPS";
            else if (headingFromWhichSource == "HDT") Properties.Settings.Default.setGPS_headingFromWhichSource = "HDT";
            mf.headingFromSource = headingFromWhichSource;

            Properties.Settings.Default.setIMU_UID = tboxTinkerUID.Text.Trim();

            mf.minFixStepDist = (double)minFixStepDistance;
            Properties.Settings.Default.setF_minFixStep = mf.minFixStepDist;


            Properties.Settings.Default.setIMU_isHeadingCorrectionFromAutoSteer = rbtnHeadingCorrAutoSteer.Checked;
            mf.ahrs.isHeadingCorrectionFromAutoSteer =  rbtnHeadingCorrAutoSteer.Checked;

            Properties.Settings.Default.setIMU_isHeadingCorrectionFromBrick =  rbtnHeadingCorrBrick.Checked;
            mf.ahrs.isHeadingCorrectionFromBrick = rbtnHeadingCorrBrick.Checked;

            Properties.Settings.Default.setIMU_isHeadingCorrectionFromExtUDP = rbtnHeadingCorrUDP.Checked;
            mf.ahrs.isHeadingCorrectionFromExtUDP = rbtnHeadingCorrUDP.Checked;


            Properties.Settings.Default.setIMU_isRollFromAutoSteer = rbtnRollAutoSteer.Checked;
            mf.ahrs.isRollFromAutoSteer = rbtnRollAutoSteer.Checked;

            Properties.Settings.Default.setIMU_isRollFromGPS = rbtnRollGPS.Checked;
            mf.ahrs.isRollFromGPS = rbtnRollGPS.Checked;

            Properties.Settings.Default.setIMU_isRollFromExtUDP = rbtnRollUDP.Checked;
            mf.ahrs.isRollFromExtUDP = rbtnRollUDP.Checked;


            Properties.Settings.Default.setGPS_isRTK = cboxIsRTK.Checked;
            mf.isRTK = cboxIsRTK.Checked;

            Properties.Settings.Default.Save();
            Properties.Vehicle.Default.Save();

            //back to FormGPS
            DialogResult = DialogResult.OK;
            Close();
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            { DialogResult = DialogResult.Cancel; Close(); }
        }

        private void FormDisplaySettings_Load(object sender, EventArgs e)
        {
            cboxNMEAHz.Text = Properties.Settings.Default.setPort_NMEAHz.ToString();

            minFixStepDistance = (decimal)Properties.Settings.Default.setF_minFixStep;
            if (nudMinFixStepDistance.CheckValue(ref minFixStepDistance)) nudMinFixStepDistance.BackColor = System.Drawing.Color.OrangeRed;
            nudMinFixStepDistance.Value = minFixStepDistance;

            tboxTinkerUID.Text = Properties.Settings.Default.setIMU_UID;
            
            //heading correction
            rbtnHeadingCorrAutoSteer.Checked = Properties.Settings.Default.setIMU_isHeadingCorrectionFromAutoSteer;
            rbtnHeadingCorrBrick.Checked = Properties.Settings.Default.setIMU_isHeadingCorrectionFromBrick;
            rbtnHeadingCorrUDP.Checked = Properties.Settings.Default.setIMU_isHeadingCorrectionFromExtUDP;
            if (!rbtnHeadingCorrAutoSteer.Checked && !rbtnHeadingCorrBrick.Checked && !rbtnHeadingCorrUDP.Checked) rbtnHeadingCorrNone.Checked = true;

            //Roll
            rbtnRollAutoSteer.Checked = Properties.Settings.Default.setIMU_isRollFromAutoSteer;
            rbtnRollGPS.Checked = Properties.Settings.Default.setIMU_isRollFromGPS;
            rbtnRollUDP.Checked = Properties.Settings.Default.setIMU_isRollFromExtUDP;
            if (!rbtnRollAutoSteer.Checked && !rbtnRollGPS.Checked && !rbtnRollUDP.Checked) rbtnRollNone.Checked = true;

            lblRollZeroOffset.Text = ((double)Properties.Settings.Default.setIMU_rollZeroX16 / 16).ToString("N2");


            //Fix
            if (Properties.Settings.Default.setGPS_fixFromWhichSentence == "GGA") rbtnGGA.Checked = true;
            else if (Properties.Settings.Default.setGPS_fixFromWhichSentence == "RMC") rbtnRMC.Checked = true;
            else if (Properties.Settings.Default.setGPS_fixFromWhichSentence == "OGI") rbtnOGI.Checked = true;

            //heading
            headingFromWhichSource = Properties.Settings.Default.setGPS_headingFromWhichSource;
            if (headingFromWhichSource == "Fix") rbtnHeadingFix.Checked = true;
            else if (headingFromWhichSource == "GPS") rbtnHeadingGPS.Checked = true;
            else if (headingFromWhichSource == "HDT") rbtnHeadingHDT.Checked = true;

            cboxIsRTK.Checked = Properties.Settings.Default.setGPS_isRTK;
        }

        #endregion EntryExit

        private void btnRemoveZeroOffset_Click(object sender, EventArgs e)
        {
            mf.ahrs.rollZeroX16 = 0;
            lblRollZeroOffset.Text = "0.00";
            Properties.Settings.Default.setIMU_rollZeroX16 = 0;
            Properties.Settings.Default.Save();
        }

        private void btnRemoveZeroOffsetPitch_Click(object sender, EventArgs e)
        {
        }

        private void btnZeroPitch_Click(object sender, EventArgs e)
        {
        }

        private void btnZeroRoll_Click(object sender, EventArgs e)
        {
            if (mf.ahrs.rollX16 == 9999)
            {
                lblRollZeroOffset.Text = "***";
            }
            else
            {
                mf.ahrs.rollZeroX16 = mf.ahrs.rollX16;
                lblRollZeroOffset.Text = ((double)mf.ahrs.rollZeroX16 / 16).ToString("N2");
                Properties.Settings.Default.setIMU_rollZeroX16 = mf.ahrs.rollX16;
                Properties.Settings.Default.Save();
            }
        }

        private void NudMinFixStepDistance_ValueChanged(object sender, EventArgs e)
        {
            minFixStepDistance = nudMinFixStepDistance.Value;
        }

        private void NudMinFixStepDistance_Enter(object sender, EventArgs e)
        {
            mf.KeypadToNUD((NumericUpDown)sender);
            btnCancel.Focus();
        }

        private void rbtnGGA_CheckedChanged(object sender, EventArgs e)
        {
            var checkedButton = groupBox4.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            Properties.Settings.Default.setGPS_fixFromWhichSentence = checkedButton.Text;
            Properties.Settings.Default.Save();
            mf.pn.fixFrom = checkedButton.Text;
        }


        private void rbtnHeadingFix_CheckedChanged(object sender, EventArgs e)
        {
            var checkedButton = headingGroupBox.Controls.OfType<RadioButton>()
                          .FirstOrDefault(r => r.Checked);
            headingFromWhichSource = checkedButton.Text;
        }

        private void cboxNMEAHz_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.setPort_NMEAHz = Convert.ToInt32(cboxNMEAHz.SelectedItem);
            Properties.Settings.Default.Save();
            mf.fixUpdateHz = Properties.Settings.Default.setPort_NMEAHz;

            mf.timerSim.Interval = (int)((1.0 / (double)mf.fixUpdateHz) * 1000.0);
        }
    }
}
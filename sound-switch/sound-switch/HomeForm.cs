﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace sound_switch
{
    public partial class HomeForm : Form
    {
        //Instance of the binding manager
        BindingManager bm = new BindingManager();
        RecorderManager rm = new RecorderManager();

        public HomeForm()
        {
            InitializeComponent();
        }

        private void btnBackBind_Click(object sender, EventArgs e)
        {
            panelBindings.Hide();
            panelMain.Show();
        }

        private void btnBindings_Click(object sender, EventArgs e)
        {
            panelBindings.Show();
            panelMain.Hide();

            //Update the display on the datagridview that holds the binding information.
            bm.update(dgvBind);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Show();
            panelMain.Hide();

            //Autopop source list on tab change
            rm.SetSourceListView(lvSource);
        }

        private void btnBackSet_Click(object sender, EventArgs e)
        {
            panelSettings.Hide();
            panelMain.Show();
        }

        private void btnToggleListen_Click(object sender, EventArgs e)
        {
            //Instruct rm to begin polling mic data
            rm.StartListening(lvSource, rtbSoundLevel);

            timer1.Enabled = true;
                
        }

        private void btnNewBind_Click(object sender, EventArgs e)
        {
            //Intermediary vals for creation of a new bind
            string bNameTemp = "";
            string bBindTemp = "";

            //Open dialog prompts for the new binding
            BindDialog dialog = new BindDialog(lvSource);

            //Open the dialogue and wait a resolution flag
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                //Get value out of description box from the dialog.
                bNameTemp = dialog.txtDescription.Text;
                bBindTemp = dialog.txtBindCode.Text;

                //Build the binding instance and add it to the binding list in the manager.
                Binding newBinding = new Binding(bNameTemp, bBindTemp, false);
                bm.addBinding(newBinding);

                //Update the binding display
                bm.update(dgvBind);
            }
            else
            {
                MessageBox.Show("Binding not created, dialog was exited prematurely.");
            }
        }

        private void btnRemoveBind_Click(object sender, EventArgs e)
        {
            int rowToDelete = dgvBind.CurrentCell.RowIndex;

            bm.removeBindingAtIndex(rowToDelete);

            bm.update(dgvBind);
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            //DEBUG, check the current unprocessed wav against the current bindings.
            Binding outputDebug = bm.compareUnprocessed();

            MessageBox.Show(outputDebug.bindCode + " was found as the best match.");
        }

        private void btnGit_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/OtagoPolytechnic/SoundSwitch");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            rm.SetSourceListView(lvSource);
        }

        private void btnSetValues_Click(object sender, EventArgs e)
        {
            rm.SetValues(tbThreshold);
        }

        private void lvSource_ItemActivate(object sender, EventArgs e)
        {
            tbDeviceName.Text = rm.DisplaySelectedDeviceName(lvSource);
        }

        private void tbThreshold_Leave(object sender, EventArgs e)
        {
            rm.ThresFlag = rm.CheckRegex(tbThreshold);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            rm.Stop();
        }

        private void rtbSoundLevel_TextChanged(object sender, EventArgs e)
        {
            rtbSoundLevel.SelectionStart = rtbSoundLevel.Text.Length;
            rtbSoundLevel.ScrollToCaret();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (rm.readyForProcessing)
            {
                rm.readyForProcessing = false;
                MessageBox.Show("eat my piss");
            }
        }
    }
}

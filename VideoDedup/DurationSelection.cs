﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoDedup
{
    public partial class DurationSelection : Form
    {
        private readonly static string TimeSpanLongFormat = @"dd\.hh\:mm\:ss";
        private readonly static string TimeSpanShortFormat = @"hh\:mm\:ss";

        private static string ToString(TimeSpan ts)
        {
            string format = ts.Days >= 1 ? TimeSpanLongFormat : TimeSpanShortFormat;
            return ts.ToString(format);
        }
        private static string ToString(TimeSpan? ts)
        {
            return ToString(ts.Value);
        }

        public TimeSpan AbsolutMaximumDuration { get; set; }
        public TimeSpan AbsolutMinimumDuration { get; set; }

        public TimeSpan? SelectedMaximumDuration { get; set; }
        public TimeSpan? SelectedMinimumDuration { get; set; }

        private bool ProgrammaticUpdate { get; set; } = false;

        public DurationSelection()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            ProgrammaticUpdate = true;

            LblDurationInfo.Text = $"Videos are between " +
                $"{ToString(AbsolutMinimumDuration)} and " +
                $"{ToString(AbsolutMaximumDuration)} long.";

            if (SelectedMinimumDuration == null)
            {
                SelectedMinimumDuration = AbsolutMinimumDuration;
            }
            TxtMinimumDuration.Text = ToString(SelectedMinimumDuration);

            if (SelectedMaximumDuration == null)
            {
                SelectedMaximumDuration = AbsolutMaximumDuration;
            }
            TxtMaximumDuration.Text = ToString(SelectedMaximumDuration);

            MinimumSlider.Minimum = (int)AbsolutMinimumDuration.TotalSeconds;
            MinimumSlider.Maximum = (int)AbsolutMaximumDuration.TotalSeconds;
            MinimumSlider.Value = (int)SelectedMinimumDuration.Value.TotalSeconds;

            MaximumSlider.Minimum = (int)AbsolutMinimumDuration.TotalSeconds;
            MaximumSlider.Maximum = (int)AbsolutMaximumDuration.TotalSeconds;
            MaximumSlider.Value = (int)SelectedMaximumDuration.Value.TotalSeconds;

            ProgrammaticUpdate = false;

            base.OnLoad(e);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!TimeSpan.TryParse(TxtMinimumDuration.Text, out TimeSpan duration))
            {
                MessageBox.Show("Minimum duration is not valid.");
                return;
            }
            SelectedMinimumDuration = duration;

            if (!TimeSpan.TryParse(TxtMaximumDuration.Text, out duration))
            {
                MessageBox.Show("Maximum duration is not valid.");
                return;
            }
            SelectedMaximumDuration = duration;

            if (SelectedMaximumDuration <= SelectedMinimumDuration)
            {
                var temp = SelectedMaximumDuration;
                SelectedMaximumDuration = SelectedMinimumDuration;
                SelectedMinimumDuration = temp;
            }

            DialogResult = DialogResult.OK;
        }

        private void MinimumSlider_Scroll(object sender, EventArgs e)
        {
            if (ProgrammaticUpdate)
            {
                return;
            }
            ProgrammaticUpdate = true;

            SelectedMinimumDuration = new TimeSpan(0, 0, MinimumSlider.Value);
            TxtMinimumDuration.Text = ToString(SelectedMinimumDuration);

            ProgrammaticUpdate = false;
        }

        private void MaximumSlider_Scroll(object sender, EventArgs e)
        {
            if (ProgrammaticUpdate)
            {
                return;
            }
            ProgrammaticUpdate = true;

            SelectedMaximumDuration = new TimeSpan(0, 0, MaximumSlider.Value);
            TxtMaximumDuration.Text = ToString(SelectedMaximumDuration);

            ProgrammaticUpdate = false;
        }

        private void TxtMinimumDuration_TextChanged(object sender, EventArgs e)
        {
            if (ProgrammaticUpdate)
            {
                return;
            }

            try
            {
                ProgrammaticUpdate = true;

                if (!TimeSpan.TryParse(TxtMinimumDuration.Text, out TimeSpan duration))
                {
                    Debug.Print("Can't parse");
                    return;
                }
                var secs = (int)duration.TotalSeconds;
                if (secs > MinimumSlider.Maximum || secs < MinimumSlider.Minimum)
                {
                    Debug.Print("Too large or too small");
                    return;
                }
                MinimumSlider.Value = secs;

            }
            finally
            {
                ProgrammaticUpdate = false;
            }
        }

        private void TxtMaximumDuration_TextChanged(object sender, EventArgs e)
        {
            if (ProgrammaticUpdate)
            {
                return;
            }

            try
            {
                ProgrammaticUpdate = true;

                if (!TimeSpan.TryParse(TxtMaximumDuration.Text, out TimeSpan duration))
                {
                    return;
                }
                var secs = (int)duration.TotalSeconds;
                if (secs > MaximumSlider.Maximum || secs < MaximumSlider.Minimum)
                {
                    return;
                }
                MaximumSlider.Value = secs;
            }
            finally
            {
                ProgrammaticUpdate = false;
            }
        }
    }
}